using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MusicPlayerProject.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        //private INavigationService navigationService;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()//INavigationService navigationService)
        {
            //this.navigationService = navigationService;

            //this.NavigateCommand = new RelayCommand(() =>
            //{
            //    this.navigationService.Navigate(MusicPlayerProject.Services.Views.Library);
            //});
        }

        public RelayCommand NavigateCommand { get; private set; }
    }
}
