using MusicPlayerProject.ViewModels;
using System;
//using System.Collections;
using System.Collections.Generic;
//using System.IO;
using System.Linq;
//using Windows.ApplicationModel.Activation;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace MusicPlayerProject.Views
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsView : MusicPlayerProject.Common.LayoutAwarePage
    {

        ViewModels.SearchResultsViewModel currentViewModel = null;

        public SearchResultsView()
        {
            this.InitializeComponent();

            this.currentViewModel =  new ViewModels.SearchResultsViewModel();

            this.DataContext = this.currentViewModel;
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var queryText = navigationParameter as String;
            this.currentViewModel.QueryText = queryText;
        }

        private void GridViewItemClick(object sender, RoutedEventArgs e)
        {
            SearchResultsViewModel model = pageRoot.DataContext as SearchResultsViewModel;
            this.Frame.Navigate(typeof(MusicPlayerView), model.SelectedSongs);
        }
    }
}
