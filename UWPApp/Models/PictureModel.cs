using System.Threading.Tasks;
using UWPApp.Extensions;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPApp.Models
{
    public class PictureModel
    {
        public string Name { get; set; }
        public BitmapImage Thumbnail { get; set; }

        public static async Task<PictureModel> Parse(StorageFile picture) =>
            picture == null ? null : new PictureModel()
                                     {
                                         Name = picture.Name,
                                         Thumbnail = await picture.GetThumbnailAsBitmapImageAsync()
                                     };

    }
}
