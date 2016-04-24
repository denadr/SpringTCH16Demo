using UWPApp.EventArgs;
using UWPApp.Storage;

namespace UWPApp.ViewModels
{
    public class SnapViewModel
    {
        public async void OnPictureCaptured(object sender, PictureCapturedEventArgs e) =>
            await PicturesRepository.SaveNewCaptureAsync(e.Stream);
    }
}
