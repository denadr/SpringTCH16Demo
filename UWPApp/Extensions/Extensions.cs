using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace UWPApp.Extensions
{
    public static class IRandomAccessStreamExtensions
    {
        public static async Task<byte[]> AsByteArray(this IRandomAccessStream stream)
        {
            // Reset stream position
            stream.Seek(0);

            var bytes = new byte[stream.Size];
            await stream.ReadAsync(bytes.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);

            return bytes;
        }
    }

    public static class StorageFileExtensions
    {
        public static async Task WriteAsync(this StorageFile file, byte[] buffer)
        {
            await FileIO.WriteBytesAsync(file, buffer);
        }
        
        public static async Task<BitmapImage> GetThumbnailAsBitmapImageAsync(this StorageFile file)
        {
            var thumbnail = new BitmapImage();

            var itemThumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);

            thumbnail.SetSource(itemThumbnail);

            return thumbnail;
        }
    }
}
