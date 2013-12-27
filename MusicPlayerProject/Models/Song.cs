using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MusicPlayerProject.Models
{
    public class Song : Common.BindableBase
    {
        private string songName;

        public string SongName
        {
            get
            {
                return this.songName;
            }
            set
            {
                this.songName = value;
                this.OnPropertyChanged("SongName");
            }
        }

        private string author;

        public string Author 
        {
            get
            {
                return this.author;
            }
            set
            {
                this.author = value;
                this.OnPropertyChanged("Author");
            }
        }

        public string DisplayName 
        {
            get
            {
                if (String.IsNullOrEmpty(this.Author))
                {
                    return this.SongName;
                }
                else
                {
                    return this.Author + " - " + this.SongName;
                }
            }
        }

        private Album songAlbum;

        public Album SongAlbum 
        {
            get
            {
                return this.songAlbum;
            }
            set
            {
                this.songAlbum = value;
                this.OnPropertyChanged("SongAlbum");
            }
        }

        private uint trackNumber;

        public uint TrackNumber 
        {
            get
            {
                return this.trackNumber;
            }
            set
            {
                this.trackNumber = value;
                this.OnPropertyChanged("TrackNumber");
            }
        }

        public TimeSpan Length { get; set; }

        public string LengthString { get { return this.Length.ToString(@"mm\:ss"); } }

        private uint rating;

        public uint Rating 
        {
            get
            {
                return this.rating;
            }
            set
            {
                this.rating = value;
                this.OnPropertyChanged("Rating");
            }
        }

        public StorageFile File { get; set; }

        public string ContentType { get; set; }

        public string Lyrics { get; set; }

        public Song(string songName, string artist, uint trackNumber, TimeSpan length, uint rating, Album album, StorageFile file, string lyrics)
        {
            this.SongName = songName;
            this.Author = artist;
            this.TrackNumber = trackNumber;
            this.Length = length;
            this.SongAlbum = album;
            this.Rating = rating;
            this.File = file;
            this.Lyrics = lyrics;
        }
    }
}
