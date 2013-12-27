//using MusicPlayerProject.Models;
using MusicPlayerProject.ViewModels;
using System;
using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
using System.Linq;
using Windows.UI.ApplicationSettings;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
//using Windows.Storage.FileProperties;
//using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Media.Imaging;
//using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MusicPlayerProject.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LibraryView : MusicPlayerProject.Common.LayoutAwarePage
    {
        public LibraryView()
        {
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += settingsPane_CommandsRequested;
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
            LibraryViewModel newModel = new LibraryViewModel();
            await newModel.LoadLibrary();
            pageRoot.DataContext = newModel;
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

        private void GridViewItemClick(object sender, RoutedEventArgs e)
        {
            LibraryViewModel model = pageRoot.DataContext as LibraryViewModel;
            this.Frame.Navigate(typeof(MusicPlayerView), model.SelectedSongs);
        }

        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var aboutCmd = new SettingsCommand("About", "About", (handler) =>
            {
                Popup popup = BuildSettingsItem(new AboutPage(), 646);
                popup.IsOpen = true;
            });
            args.Request.ApplicationCommands.Add(aboutCmd);
        }

        private Popup BuildSettingsItem(UserControl u, int w)
        {
            Popup p = new Popup();
            p.IsLightDismissEnabled = true;
            p.ChildTransitions = new TransitionCollection();
            p.ChildTransitions.Add(new PaneThemeTransition()
            {
                Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) ?
                        EdgeTransitionLocation.Right :
                        EdgeTransitionLocation.Left
            });

            u.Width = w;
            u.Height = Window.Current.Bounds.Height;
            p.Child = u;

            p.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (Window.Current.Bounds.Width - w) : 0);
            p.SetValue(Canvas.TopProperty, 0);

            return p;
        }
    }
}
