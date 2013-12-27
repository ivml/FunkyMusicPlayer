using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MusicPlayerProject.Models
{
    public class Album : Common.BindableBase
    {
        private string albumName;

        public string AlbumName 
        {
            get
            {
                return this.albumName;
            }
            set
            {
                this.albumName = value;
                this.OnPropertyChanged("AlbumName");
            }
        }

        private string albumArtist;

        public string AlbumArtist 
        {
            get
            {
                return this.albumArtist;
            }
            set
            {
                this.albumArtist = value;
                this.OnPropertyChanged("AlbumArtist");
            }
        }

        public ImageSource AlbumCover { get; set; }

        private uint albumYear;

        public uint AlbumYear 
        {
            get
            {
                return this.albumYear;
            }
            set
            {
                this.albumYear = value;
                this.OnPropertyChanged("AlbumYear");
            }
        }

        public IList<string> Genre { get; set; }

        public string GenreString { get { return this.Genre.FirstOrDefault(); } }

        public Album(string albumName, string albumArtist, ImageSource albumCover, uint albumYear, string genre)
        {
            this.AlbumName = albumName;
            this.AlbumArtist = albumArtist;
            this.AlbumCover = albumCover;
            this.AlbumYear = albumYear;
            this.Genre = new List<string>();
            if (!string.IsNullOrEmpty(genre))
            {
                this.Genre.Add(genre);
            }
        }
    }
}
