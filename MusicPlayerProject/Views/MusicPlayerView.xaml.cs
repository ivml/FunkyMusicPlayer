using MusicPlayerProject.Commands;
using MusicPlayerProject.Models;
using MusicPlayerProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Input;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
using Windows.Media;
//using Windows.Storage;
//using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Media.Imaging;
//using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MusicPlayerProject.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MusicPlayerView : MusicPlayerProject.Common.LayoutAwarePage
    {
        MusicPlayerViewModel newModel = null;
        DateTime lyricsLastCheckTime;

        public MusicPlayerView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this.newModel = new MusicPlayerViewModel();
            this.newModel.Songs = navigationParameter as ObservableCollection<Song>;
            this.newModel.CurrentSong = newModel.Songs.FirstOrDefault();
            pageRoot.DataContext = this.newModel;
            myPlaylist.ItemsSource = this.newModel.Songs;

            this.lyricsLastCheckTime = DateTime.Now.AddSeconds(-20);

            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;

            timelineSlider.ValueChanged += timelineSlider_ValueChanged;

            PointerEventHandler pointerpressedhandler = new PointerEventHandler(slider_PointerEntered);
            timelineSlider.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);

            PointerEventHandler pointerreleasedhandler = new PointerEventHandler(slider_PointerCaptureLost);
            timelineSlider.AddHandler(Control.PointerCaptureLostEvent, pointerreleasedhandler, true);

            //Maybe remove this
            await SetMediaElementSourceAsync(this.newModel.CurrentSong);
            mediaElement.Play();
            MediaControl.IsPlaying = true;
            SetupTimer();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            double absvalue = (int)Math.Round(
                mediaElement.NaturalDuration.TimeSpan.TotalSeconds,
                MidpointRounding.AwayFromZero);

            timelineSlider.Maximum = absvalue;

            timelineSlider.StepFrequency =
                SliderFrequency(mediaElement.NaturalDuration.TimeSpan);
            mediaElement.Play();
            MediaControl.IsPlaying = true;
            SetupTimer();
        }

        private bool _sliderpressed = false;

        void slider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _sliderpressed = true;
        }

        void slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);
            _sliderpressed = false;
        }

        void timelineSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!_sliderpressed)
            {
                mediaElement.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                if (_sliderpressed)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer.Start();
                }
                MediaControl.IsPlaying = true;
            }

            if (mediaElement.CurrentState == MediaElementState.Paused)
            {
                _timer.Stop();
                MediaControl.IsPlaying = false;
            }

            if (mediaElement.CurrentState == MediaElementState.Stopped)
            {
                _timer.Stop();
                timelineSlider.Value = 0;
                MediaControl.IsPlaying = false;
            }
        }

        void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopTimer();
            timelineSlider.Value = 0.0;

            if (myPlaylist.SelectedIndex == myPlaylist.Items.Count - 1)
            {
                return;
            }
            else
            {
                if (myPlaylist.SelectedIndex + 1 < myPlaylist.Items.Count)
                {
                    myPlaylist.SelectedIndex++;
                }
            }
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // get HRESULT from event args 
            string hr = GetHresultFromErrorMessage(e);

            // Handle media failed event appropriately 
        }

        void MediaControl_StopPressed(object sender, object e)
        {
            mediaElement.Stop();
        }

        async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (mediaElement.CurrentState == MediaElementState.Playing)
                {
                    mediaElement.Pause();
                }
                else
                {
                    mediaElement.Play();
                }
            }

            );
        }

        void MediaControl_PlayPressed(object sender, object e)
        {
            mediaElement.Play();
        }

        void MediaControl_PausePressed(object sender, object e)
        {
            mediaElement.Pause();
        }

        private string GetHresultFromErrorMessage(ExceptionRoutedEventArgs e)
        {
            String hr = String.Empty;
            String token = "HRESULT - ";
            const int hrLength = 10;     // eg "0xFFFFFFFF"

            int tokenPos = e.ErrorMessage.IndexOf(token, StringComparison.Ordinal);
            if (tokenPos != -1)
            {
                hr = e.ErrorMessage.Substring(tokenPos + token.Length, hrLength);
            }

            return hr;
        }

        private DispatcherTimer _timer;

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(timelineSlider.StepFrequency);
            StartTimer();
        }

        private void _timer_Tick(object sender, object e)
        {
            if (!_sliderpressed)
            {
                timelineSlider.Value = mediaElement.Position.TotalSeconds;
            }
        }

        private void StartTimer()
        {
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
        }

        private double SliderFrequency(TimeSpan timevalue)
        {
            double stepfrequency = -1;

            double absvalue = (int)Math.Round(
                timevalue.TotalSeconds, MidpointRounding.AwayFromZero);

            stepfrequency = (int)(Math.Round(absvalue / 100));

            if (timevalue.TotalMinutes >= 10 && timevalue.TotalMinutes < 30)
            {
                stepfrequency = 10;
            }
            else if (timevalue.TotalMinutes >= 30 && timevalue.TotalMinutes < 60)
            {
                stepfrequency = 30;
            }
            else if (timevalue.TotalHours >= 1)
            {
                stepfrequency = 60;
            }

            if (stepfrequency == 0) stepfrequency += 1;

            if (stepfrequency == 1)
            {
                stepfrequency = absvalue / 100;
            }

            return stepfrequency;
        }

        async void MyPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //this.newModel.CurrentSong = myPlaylist.SelectedItem as Song;
                this.newModel.CurrentSong = e.AddedItems.FirstOrDefault() as Song;
                await SetMediaElementSourceAsync(this.newModel.CurrentSong);

                GetSongLyricsAsync();

                var trackName = this.newModel.CurrentSong.SongName;
                if (string.IsNullOrEmpty(trackName))
                {
                    trackName = this.newModel.CurrentSong.DisplayName;
                }

                ToastNotificator.ShowNotificationSongChange(trackName, this.newModel.CurrentSong.Author);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

        }

        private async void GetSongLyricsAsync()
        {
            string artist = this.newModel.CurrentSong.Author;
            string songName = this.newModel.CurrentSong.SongName;
            if (!String.IsNullOrEmpty(artist)
                && !String.IsNullOrEmpty(songName)
                && String.IsNullOrEmpty(this.newModel.CurrentSong.Lyrics))
            {
                var timeDifference = DateTime.Now.Subtract(this.lyricsLastCheckTime);
                if (timeDifference.TotalSeconds > 20)
                {
                    var lyrics = await HttpLyricRequester.GetLyrics(artist, songName);

                    if (lyrics.Equals("No internet connection"))
                    {
                        LyricsBoxInfo.Text = lyrics;
                    }
                    else
                    {
                        this.newModel.CurrentSong.Lyrics = lyrics;
                        Song newSongInfo = this.newModel.CurrentSong;
                        this.newModel.CurrentSong = newSongInfo;
                        this.lyricsLastCheckTime = DateTime.Now;
                        LyricsBoxInfo.Text = "";
                    }
                }
                else
                {
                    LyricsBoxInfo.Text = "You can check for lyrics only once every 20 seconds. Please wait and try again later.";
                }
            }
        }

        async Task SetMediaElementSourceAsync(Song newSong)
        {
            var stream = await newSong.File.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MediaControl.TrackName = newSong.SongName;
            mediaElement.SetSource(stream, newSong.File.ContentType);
            
            mediaElement.PosterSource = newSong.SongAlbum.AlbumCover;
        }

        private void NavigateToMetadata(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AudioMetadataView), this.newModel.CurrentSong);
        }
    }
}
