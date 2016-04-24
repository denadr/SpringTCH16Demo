using System;
using System.Linq;
using System.Threading.Tasks;
using UWPApp.EventArgs;
using UWPApp.Extensions;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPApp.Views
{
    public sealed partial class CameraCapture : UserControl
    {
        private DisplayInformation _displayInformation;
        private MediaCapture _mediaCapture;

        public event EventHandler<PictureCapturedEventArgs> PictureCaptured = delegate { };
        public void RaisePictureCaptured(IRandomAccessStream stream) =>
            PictureCaptured?.Invoke(this, new PictureCapturedEventArgs(stream));
        
        public CameraCapture()
        {
            InitializeComponent();

            var app = Application.Current;
            app.Suspending += async (s, e) => await DisposeAsync();
            app.Resuming += async (s, e) => await InitializeAsync();
        }

        private async void CameraCapture_Loaded(object sender, RoutedEventArgs e) =>
            await InitializeAsync();

        private async void CameraCapture_Unloaded(object sender, RoutedEventArgs e) =>
            await DisposeAsync();

        private async Task InitializeAsync()
        {
            var device = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)).FirstOrDefault();

            InitializeDisplayInformation();
            await InitializeMediaCaptureAsync(device.Id);
            InitializeCapturePreview(device.EnclosureLocation);
            
            await _mediaCapture.StartPreviewAsync();
        }

        private void InitializeDisplayInformation()
        {
            _displayInformation = DisplayInformation.GetForCurrentView();
            _displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
        }

        private async Task InitializeMediaCaptureAsync(string id)
        {
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings()
            {
                VideoDeviceId = id
            });

            var initialRotation = VideoRotationUtilities.FromDisplayOrientation(_displayInformation.CurrentOrientation, IsMirroredPreview());
            _mediaCapture.SetPreviewRotation(initialRotation);
        }

        private void InitializeCapturePreview(EnclosureLocation cameraLocation)
        {
            if (cameraLocation != null && cameraLocation.Panel == Windows.Devices.Enumeration.Panel.Front)
            {
                _capturePreview.FlowDirection = FlowDirection.RightToLeft;
            }

            _capturePreview.Source = _mediaCapture;
        }

        private async Task DisposeAsync()
        {
            DisposeCapturePreview();
            DisposeDisplayInformation();
            await DisposeMediaCaptureAsync();
        }

        private async Task DisposeMediaCaptureAsync()
        {
            if (_mediaCapture != null)
            {
                await _mediaCapture.StopPreviewAsync();
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }

        private void DisposeCapturePreview()
        {
            if (_capturePreview != null)
            {
                _capturePreview.Source = null;
            }
        }

        private void DisposeDisplayInformation()
        {
            if (_displayInformation != null)
            {
                _displayInformation.OrientationChanged -= DisplayInformation_OrientationChanged;
            }
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            if (_mediaCapture != null)
            {
                var newRotation = VideoRotationUtilities.FromDisplayOrientation(sender.CurrentOrientation, IsMirroredPreview());
                _mediaCapture.SetPreviewRotation(newRotation);
            }
        }

        private async void CapturePreview_Tapped(object sender, TappedRoutedEventArgs e) =>
            await CapturePhotoAsync();

        public async Task CapturePhotoAsync()
        {
            if (_mediaCapture != null)
            {
                using (var stream = new InMemoryRandomAccessStream())
                {
                    await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                    RaisePictureCaptured(stream);
                }
            }
        }

        private bool IsMirroredPreview() =>
            _capturePreview.FlowDirection == FlowDirection.RightToLeft;
    }
}
