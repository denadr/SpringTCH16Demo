using UWPApp.ViewModels;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPApp.Views
{
    public sealed partial class PicturesPage : Page
    {
        private PicturesViewModel _viewModel = new PicturesViewModel();

        private bool _isCameraButtonPresent;

        public PicturesPage()
        {
            InitializeComponent();

            DataContext = _viewModel;

            // Adaptive Code: Ask explicitly for special hardware button event (camera button).
            _isCameraButtonPresent = ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "CameraPressed");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var ignore = _viewModel.LoadPicturesAsync();

            if (_isCameraButtonPresent)
            {
                HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (_isCameraButtonPresent)
            {
                HardwareButtons.CameraPressed -= HardwareButtons_CameraPressed;
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e) =>
            _splitView.IsPaneOpen = !_splitView.IsPaneOpen;

        private void HardwareButtons_CameraPressed(object sender, CameraEventArgs e) =>
            Frame.Navigate(typeof(SnapPage));

        private void CameraButton_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(SnapPage));
    }
}
