//using GalaSoft.MvvmLight;
using MusicPlayerProject.Commands;
using MusicPlayerProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media;
using Windows.UI.Xaml.Controls;

namespace MusicPlayerProject.ViewModels
{
    public class MusicPlayerViewModel : Common.BindableBase
    {
        private ObservableCollection<Song> songs;

        public MusicPlayerViewModel()
        {
            this.songs = new ObservableCollection<Song>();
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

        protected Song currentSong;
        public Song CurrentSong
        {
            get
            {
                return this.currentSong;
            }
            set
            {
                this.currentSong = value;
                this.OnPropertyChanged("CurrentSong");
            }
        }

        private ICommand playButtonCommand;

        public ICommand PlayButtonCommand
        {
            get
            {
                if (this.playButtonCommand == null)
                {
                    this.playButtonCommand = new RelayCommand(this.PlayButton);
                }
                return this.playButtonCommand;
            }
        }

        internal void PlayButton(object obj)
        {
            var mediaElement = obj as MediaElement;

            if (mediaElement.DefaultPlaybackRate != 1)
            {
                mediaElement.DefaultPlaybackRate = 1.0;
            }

            mediaElement.Play();
        }

        private ICommand pauseButtonCommand;

        public ICommand PauseButtonCommand
        {
            get
            {
                if (this.pauseButtonCommand == null)
                {
                    this.pauseButtonCommand = new RelayCommand(this.PauseButton);
                }
                return this.pauseButtonCommand;
            }
        }

        internal void PauseButton(object obj)
        {
            var mediaElement = obj as MediaElement;

            if (MediaControl.IsPlaying == true)
            {
                mediaElement.Pause();
            }
            else
            {
                mediaElement.Play();
            }
        }

        private ICommand stopButtonCommand;

        public ICommand StopButtonCommand
        {
            get
            {
                if (this.stopButtonCommand == null)
                {
                    this.stopButtonCommand = new RelayCommand(this.StopButton);
                }
                return this.stopButtonCommand;
            }
        }

        internal void StopButton(object obj)
        {
            var mediaElement = obj as MediaElement;

            mediaElement.Stop();
        }

        private ICommand muteButtonCommand;

        public ICommand MuteButtonCommand
        {
            get
            {
                if (this.muteButtonCommand == null)
                {
                    this.muteButtonCommand = new RelayCommand(this.MuteButton);
                }
                return this.muteButtonCommand;
            }
        }

        internal void MuteButton(object obj)
        {
            var mediaElement = obj as MediaElement;

            mediaElement.IsMuted = !mediaElement.IsMuted;
        }

        private ICommand volumeUpButtonCommand;

        public ICommand VolumeUpButtonCommand
        {
            get
            {
                if (this.volumeUpButtonCommand == null)
                {
                    this.volumeUpButtonCommand = new RelayCommand(this.VolumeUpButton);
                }
                return this.volumeUpButtonCommand;
            }
        }

        internal void VolumeUpButton(object obj)
        {
            var mediaElement = obj as MediaElement;


            if (mediaElement.IsMuted)
            {
                mediaElement.IsMuted = false;
            }

            if (mediaElement.Volume < 1)
            {
                mediaElement.Volume += .1d;
            }
        }

        private ICommand volumeDownButtonCommand;

        public ICommand VolumeDownButtonCommand
        {
            get
            {
                if (this.volumeDownButtonCommand == null)
                {
                    this.volumeDownButtonCommand = new RelayCommand(this.VolumeDownButton);
                }
                return this.volumeDownButtonCommand;
            }
        }

        internal void VolumeDownButton(object obj)
        {
            var mediaElement = obj as MediaElement;

            if (mediaElement.IsMuted)
            {
                mediaElement.IsMuted = false;
            }

            if (mediaElement.Volume > 0)
            {
                mediaElement.Volume -= .1d;
            }
        }
    }
}
