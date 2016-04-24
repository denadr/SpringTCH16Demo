using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPApp.Views
{
    public sealed partial class SnapPage : Page
    {
        private bool _isBackButtonPresent;
        private bool _isCameraButtonPresent;

        public SnapPage()
        {
            InitializeComponent();

            // Adaptive Code: Ask explicitly for special hardware button events (back and camera buttons).
            _isBackButtonPresent = ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "BackPressed");
            _isCameraButtonPresent = ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "CameraPressed");
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (Frame.CanGoBack)
            {
                var navigationManager = SystemNavigationManager.GetForCurrentView();
                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                navigationManager.BackRequested += NavigationManager_BackRequested;

                if (_isBackButtonPresent)
                {
                    HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                }

                if (_isCameraButtonPresent)
                {
                    HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);

            if (Frame.CanGoBack)
            {
                var navigationManager = SystemNavigationManager.GetForCurrentView();
                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                navigationManager.BackRequested -= NavigationManager_BackRequested;

                if (_isBackButtonPresent)
                {
                    HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                }

                if (_isCameraButtonPresent)
                {
                    HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
                }
            }
        }

        private void NavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            NavigateBack();
            e.Handled = true;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            NavigateBack();
            e.Handled = true;
        }

        private async void HardwareButtons_CameraPressed(object sender, CameraEventArgs e) =>
            await _cameraCapture.CapturePhotoAsync();

        private void NavigateBack()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
