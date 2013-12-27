using MusicPlayerProject.Commands;
using MusicPlayerProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayerProject.ViewModels
{
    public class AudioMetadataViewModel : Common.BindableBase
    {
        protected Song song;
        public Song Song
        {
            get
            {
                return this.song;
            }
            set
            {
                this.song = value;
                this.OnPropertyChanged("Song");
            }
        }

        private ICommand setAttributesCommand;

        public ICommand SetAttributesCommand
        {
            get
            {
                if (this.setAttributesCommand == null)
                {
                    this.setAttributesCommand = new RelayCommand(this.SetAttributes);
                }
                return this.setAttributesCommand;
            }
        }

        internal async void SetAttributes(object obj)
        {
            var currentSong = obj as Song;
            var songName = currentSong.SongName;
            var artist = currentSong.Author;
            var album = currentSong.SongAlbum.AlbumName;
            var albumArtist = currentSong.SongAlbum.AlbumArtist;
            var albumYear = currentSong.SongAlbum.AlbumYear;
            var track = currentSong.TrackNumber;
            var rating = currentSong.Rating;

            try
            {
                List<string> propertiesName = new List<string>();
                propertiesName.Add("System.Title");
                propertiesName.Add("System.Music.Artist");
                propertiesName.Add("System.Music.AlbumTitle");
                propertiesName.Add("System.Music.AlbumArtist");
                propertiesName.Add("System.Media.Year");
                propertiesName.Add("System.Music.TrackNumber");
                propertiesName.Add("System.Rating");

                IDictionary<string, object> extraProperties = await this.song.File.Properties.RetrievePropertiesAsync(propertiesName);
                if (extraProperties.ContainsKey("System.Title"))
                {
                    extraProperties.Remove("System.Title");
                }
                if (extraProperties.ContainsKey("System.Music.Artist"))
                {
                    extraProperties.Remove("System.Music.Artist");
                }
                if (extraProperties.ContainsKey("System.Music.AlbumTitle"))
                {
                    extraProperties.Remove("System.Music.AlbumTitle");
                }
                if (extraProperties.ContainsKey("System.Music.AlbumArtist"))
                {
                    extraProperties.Remove("System.Music.AlbumArtist");
                }
                if (extraProperties.ContainsKey("System.Media.Year"))
                {
                    extraProperties.Remove("System.Media.Year");
                }
                if (extraProperties.ContainsKey("System.Music.TrackNumber"))
                {
                    extraProperties.Remove("System.Music.TrackNumber");
                }
                if (extraProperties.ContainsKey("System.Rating"))
                {
                    extraProperties.Remove("System.Rating");
                }

                extraProperties.Add("System.Title", songName); //musicInfo.Title);
                extraProperties.Add("System.Music.Artist", artist); //musicInfo.Artist
                extraProperties.Add("System.Music.AlbumTitle", album); //musicInfo.Album);
                extraProperties.Add("System.Music.AlbumArtist", albumArtist); //musicInfo.AlbumArtist);
                extraProperties.Add("System.Media.Year", albumYear); //musicInfo.Year);
                extraProperties.Add("System.Music.TrackNumber", track); //musicInfo.TrackNumber);
                extraProperties.Add("System.Rating", rating); //musicInfo.Rating);
                await this.song.File.Properties.SavePropertiesAsync(extraProperties);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
