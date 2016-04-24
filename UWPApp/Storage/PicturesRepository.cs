using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using UWPApp.Extensions;
using UWPApp.Models;
using Windows.Storage;
using Windows.Storage.Streams;

namespace UWPApp.Storage
{
    public static class PicturesRepository
    {
        // TODO: Your storage folder name goes here.
        private static readonly string _storageFolderString = "your_storage_folder_name";
        private static StorageFolder _storageFolder;
        private static async Task<StorageFolder> GetStorageFolder() => _storageFolder ??
                (_storageFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync(
                    _storageFolderString, CreationCollisionOption.OpenIfExists));

        private static ICollection<StorageFile> _loadedPictures = new List<StorageFile>();

        public static async Task SaveNewCaptureAsync(IRandomAccessStream stream)
        {
            var bytes = await stream.AsByteArray();

            var storageFile = await CreateStorageFileAsync(bytes);
            
            var ignore = AzureBlobStorage.Instance?.Upload(storageFile);

            _loadedPictures.Add(storageFile);
        }

        private static async Task SaveNewBlobAsync(CloudBlob blob)
        {
            var storageFile = await CreateStorageFileAsync(blob.Name);

            if (storageFile == null)
            { // File exists in local storage
                var folder = await GetStorageFolder();
                storageFile = await folder.GetFileAsync(blob.Name);
            }
            else
            { // Download the new picture
                using (var stream = await AzureBlobStorage.Instance?.Download(blob))
                {
                    await storageFile.WriteAsync(stream.ToArray());
                }
            }

            _loadedPictures.Add(storageFile);
        }

        private static async Task<StorageFile> CreateStorageFileAsync(byte[] buffer)
        {
            var fileName = string.Format("pic_{0}.jpg", Guid.NewGuid());
            var folder = await GetStorageFolder();

            var storageFile = await folder.CreateFileAsync(fileName);
            await storageFile.WriteAsync(buffer);

            return storageFile;
        }

        private static async Task<StorageFile> CreateStorageFileAsync(string fileName)
        {
            var folder = await GetStorageFolder();
            StorageFile storageFile = null;
            try
            {
                storageFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.FailIfExists);
            }
            catch (Exception)
            {
                // File already exists -> do nothing
            }
            return storageFile;
        }

        public static async Task<IEnumerable<PictureModel>> GetPictures()
        {
            // Check for new pictures in the blob storage first
            var allBlobs = await AzureBlobStorage.Instance?.GetBlobs();
            foreach (var blob in allBlobs)
            {
                if (GetFile(blob.Name) == null)
                {
                    await SaveNewBlobAsync(blob);
                }
            }

            // Return all pictures, now including stored and new ones
            // TODO: yield iterator
            var pictures = new List<PictureModel>();
            foreach (var picture in _loadedPictures)
            {
                pictures.Add(await PictureModel.Parse(picture));
            }
            return pictures;
        }

        public static async Task<PictureModel> GetPicture(string name) =>
            await PictureModel.Parse(GetFile(name));

        public static StorageFile GetFile(string name) =>
            _loadedPictures.FirstOrDefault(p => p.Name == name);
    }
}
