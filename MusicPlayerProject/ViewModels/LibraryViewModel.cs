using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Ioc;
using MusicPlayerProject.Commands;
using MusicPlayerProject.Models;
//using MusicPlayerProject.Views;
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
using Windows.Storage.Pickers;
//using Windows.Storage.Streams;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicPlayerProject.ViewModels
{
    public class LibraryViewModel : Common.BindableBase
    {
        private ObservableCollection<Song> songs;
        private ObservableCollection<Song> selectedSongs;

        public LibraryViewModel()
        {
            this.songs = new ObservableCollection<Song>();
            this.selectedSongs = new ObservableCollection<Song>();
        }

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
                this.OnPropertyChanged("Songs");
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

        private ICommand loadFilesCommand;

        public ICommand LoadFilesCommand
        {
            get
            {
                if (this.loadFilesCommand == null)
                {
                    this.loadFilesCommand = new RelayCommand(this.LoadMultipleFiles);
                }
                return this.loadFilesCommand;
            }
        }

        private ICommand loadFolderCommand;

        public ICommand LoadFolderCommand
        {
            get
            {
                if (this.loadFolderCommand == null)
                {
                    this.loadFolderCommand = new RelayCommand(this.LoadFolder);
                }
                return this.loadFolderCommand;
            }
        }

        internal async void LoadMultipleFiles(object obj)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;

            var files = await openPicker.PickMultipleFilesAsync();
            foreach (var file in files)
            {
                GetFileInfo(file, "");
            }
        }

        internal async void LoadFolder(object obj)
        {
            FolderPicker openPicker = new FolderPicker();
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;

            var folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                var files = await folder.GetFilesAsync();

                foreach (var file in files)
                {
                    if (file.FileType == ".mp3")
                    {
                        GetFileInfo(file, "");
                    }
                }
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
                if (imageThumbnail != null && imageThumbnail.Type == ThumbnailType.Image)
                {
                    albumCover.SetSource(imageThumbnail);
                }
                else
                {
                    var uri = new Uri("ms-appx:///Assets/NoArtwork.png");
                    var noArtworkFile = Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                    albumCover.UriSource = uri;
                }

                //check if there is actual metadata
                if (string.IsNullOrEmpty(musicInfo.Title))
                {
                    musicInfo.Title = file.DisplayName;
                }

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

        internal async void AddSong(string songName, string artist, uint trackNumber, TimeSpan length, uint rating, Album album, string token)
        {
            var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
            this.songs.Add(new Song(songName, artist, trackNumber, length, rating, album, file, ""));
        }

        internal void AddSelectedSong(Song selectedSong)
        {
            this.selectedSongs.Add(selectedSong);
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


        private ICommand clearLibraryCommand;

        public ICommand ClearLibraryCommand
        {
            get
            {
                if (this.clearLibraryCommand == null)
                {
                    this.clearLibraryCommand = new RelayCommand(this.ClearLibrary);
                }
                return this.clearLibraryCommand;
            }
        }

        private async void ClearLibrary(object obj)
        {
            StorageApplicationPermissions.FutureAccessList.Clear();
            LibraryViewModel newModel = new LibraryViewModel();
            await LoadLibrary();
        }
        //private ICommand playSelectedSongs;

        //public ICommand PlaySelectedSongs
        //{
        //    get
        //    {
        //        if (this.playSelectedSongs == null)
        //        {
        //            this.playSelectedSongs = new RelayCommand(this.SendSongsToPlayer);
        //        }
        //        return this.playSelectedSongs;
        //    }
        //}

        //internal void SendSongsToPlayer(object obj)
        //{
        //    var x = 4;
        //    //Navigate(typeof(MusicPlayerView), this.SelectedSongs);
        //}

        public async System.Threading.Tasks.Task LoadLibrary()
        {
            //StorageApplicationPermissions.FutureAccessList.Clear();
            var accessListEntries =
                StorageApplicationPermissions.FutureAccessList.Entries;
            foreach (var entry in accessListEntries)
            {
                var file = await
                    StorageApplicationPermissions.FutureAccessList.GetFileAsync(entry.Token);

                GetFileInfo(file, entry.Token);
            }
        }
    }
}
