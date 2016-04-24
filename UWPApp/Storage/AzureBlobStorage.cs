using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Windows.Storage;

namespace UWPApp.Storage
{
    public class AzureBlobStorage
    {
        // TODO: Your storage account name goes here.
        private static readonly string _storageAccountString = "your_storage_account_name";
        // TODO: Your storage container name goes here.
        private static readonly string _storageContainerString = "your_storage_container_name";

        private static CloudBlobContainer _storageContainer;

        private static AzureBlobStorage _instance;
        public static AzureBlobStorage Instance
        {
            get { return _instance ?? (_instance = new AzureBlobStorage()); }
        }

        private AzureBlobStorage()
        {
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            try
            {
                var storageCredentials = new StorageCredentials(_storageAccountString, MyStorageAccountPrimaryKey.String);

                var storageAccount = new CloudStorageAccount(storageCredentials, true);

                var blobClient = storageAccount.CreateCloudBlobClient();

                _storageContainer = blobClient.GetContainerReference(_storageContainerString);
                var ignore = _storageContainer.CreateIfNotExistsAsync();
            }
            catch (Exception e)
            {
                _instance = null;
                Debug.WriteLine("InitializeStorage(): " + e);
            }
        }

        public async Task Upload(StorageFile file)
        {
            try
            {
                var blob = _storageContainer.GetBlockBlobReference(file.Name);

                await blob.UploadFromFileAsync(file);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Upload(file): " + e);
            }
        }

        public async Task<IEnumerable<CloudBlob>> GetBlobs()
        {
            var results = new List<CloudBlob>();

            try
            {
                BlobContinuationToken continuationToken = null;
                var blobs = new List<IListBlobItem>();
                do
                {
                    var listing = await _storageContainer.ListBlobsSegmentedAsync(continuationToken);

                    blobs.AddRange(listing.Results);

                    continuationToken = listing.ContinuationToken;
                }
                while (continuationToken != null);

                foreach (var blob in blobs)
                {
                    if (blob is CloudBlob)
                    {
                        results.Add(blob as CloudBlob);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Download(): " + e);
            }

            return results;
        }

        public async Task<MemoryStream> Download(CloudBlob blob)
        {
            var stream = new MemoryStream();

            await blob.DownloadToStreamAsync(stream);

            // Reset stream postition to beginning.
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
