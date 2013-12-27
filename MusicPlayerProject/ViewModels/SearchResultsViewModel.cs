using MusicPlayerProject.Commands;
using MusicPlayerProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicPlayerProject.ViewModels
{
    public class SearchResultsViewModel : Common.BindableBase
    {
        private string queryText;

        public string QueryText 
        {
            get
            {
                return this.queryText;
            }
            set
            {
                this.queryText = value;
                this.OnPropertyChanged("QueryText");
                LoadResults();
            }
        }

        public SearchResultsViewModel()
        {
            this.selectedSongs = new ObservableCollection<Song>();
        }

        private ObservableCollection<Song> songs;
        private ObservableCollection<Song> selectedSongs;

        public IEnumerable<Song> Songs
        {
            get
            {
                if (this.songs == null)
                {
                    this.songs = new ObservableCollection<Song>();
                }
                return this.songs;
            }
            set
            {
                if (this.songs == null)
                {
                    this.songs = new ObservableCollection<Song>();
                }
                this.SetObservableValues(this.songs, value);
            }
        }

        public IEnumerable<Song> SelectedSongs
        {
            get
            {
                if (this.selectedSongs == null)
                {
                    this.selectedSongs = new ObservableCollection<Song>();
                }
                return this.selectedSongs;
            }
            set
            {
                if (this.selectedSongs == null)
                {
                    this.selectedSongs = new ObservableCollection<Song>();
                }
                this.SetObservableValues(this.selectedSongs, value);
            }
        }

        private void SetObservableValues<T>(ObservableCollection<T> observableCollection, IEnumerable<T> values)
        {
            if (observableCollection != values)
            {
                observableCollection.Clear();
                foreach (var item in values)
                {
                    observableCollection.Add(item);
                }
            }
        }

        private async void LoadResults()
        {
            var accessListEntries =
                StorageApplicationPermissions.FutureAccessList.Entries;
            foreach (var entry in accessListEntries)
            {
                var file = await
                    StorageApplicationPermissions.FutureAccessList.GetFileAsync(entry.Token);

                GetFileInfo(file, entry.Token);
            }
        }

        private async void GetFileInfo(StorageFile file, string token)
        {
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            if (null != file)
            {
                if (String.IsNullOrEmpty(token))
                {
                    token = StorageApplicationPermissions.FutureAccessList.Add(file);
                }

                MusicProperties musicInfo = await file.Properties.GetMusicPropertiesAsync();
                string genre = musicInfo.Genre.FirstOrDefault();

                StorageItemThumbnail imageThumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem);
                BitmapImage albumCover = new BitmapImage();
                albumCover.SetSource(imageThumbnail);

                //check if there is actual metadata
                if (string.IsNullOrEmpty(musicInfo.Title))
                {
                    musicInfo.Title = file.DisplayName;
                }

                if (musicInfo.Title.ToLower().Contains(this.queryText.ToLower())
                    || musicInfo.Artist.ToLower().Contains(this.queryText.ToLower())
                    || musicInfo.Album.ToLower().Contains(this.queryText.ToLower()))
                {
                    AddSong
                    (musicInfo.Title,
                    musicInfo.Artist,
                    musicInfo.TrackNumber,
                    musicInfo.Duration,
                    musicInfo.Rating,
                    new Album(musicInfo.Album, musicInfo.AlbumArtist, albumCover, musicInfo.Year, genre),
                    token);
                }
            }
        }

        internal async void AddSong(string songName, string artist, uint trackNumber, TimeSpan length, uint rating, Album album, string token)
        {
            var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
            this.songs.Add(new Song(songName, artist, trackNumber, length, rating, album, file, ""));
        }

        private ICommand songsSelectionChanged;

        public ICommand SongsSelectionChanged
        {
            get
            {
                if (this.songsSelectionChanged == null)
                {
                    this.songsSelectionChanged = new RelayCommand(this.SendFiles);
                }
                return this.songsSelectionChanged;
            }
        }

        private void SendFiles(object obj)
        {
            if (selectedSongs.Contains(obj as Song))
            {
                selectedSongs.Remove(obj as Song);
            }
            else
            {
                selectedSongs.Add(obj as Song);
            }
        }
    }
}
