using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Web;

namespace SampleStore.Models
{
    /// <summary>
    /// Repository class to wrap the Azure blob file stuff
    /// </summary>
    public class BlobRepository
    {
        private readonly CloudBlobContainer blobContainer;

        /// <summary>
        /// Creates a new BlobRepository.
        /// </summary>
        public BlobRepository()
        {
            // Get cloud account from connection string.
            var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
            var cloudStorage = CloudStorageAccount.Parse(connectionString);

            // Get container from storage.
            var blobClient = cloudStorage.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference("soundblob");
            blobContainer.CreateIfNotExists();

            // Make sure we can read the URL of the blob once created.
            blobContainer.SetPermissions(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
        }

        /// <summary>
        /// Adds file to blob storage.
        /// </summary>
        /// <param name="file">File to add</param>
        /// <param name="id">SampleId</param>
        /// <returns>Blob name</returns>
        public string AddSoundBlob(HttpPostedFile file, string id)
        {
            string blobName = GetBlobSoundName(id);

            var blob = blobContainer.GetBlockBlobReference(blobName);
            blob.Properties.ContentType = file.ContentType;
            blob.UploadFromStream(file.InputStream);

            return blobName;
        }

        /// <summary>
        /// Delete sound and sample blobs with ID.
        /// </summary>
        /// <param name="id">SampleId to delete.</param>
        public void DeleteBlobs(string id)
        {
            var blobSoundName = GetBlobSoundName(id);
            DeleteBlobInternal(blobSoundName);
            var blobSampleName = GetBlobSampleName(id);
            DeleteBlobInternal(blobSampleName);
        }

        private void DeleteBlobInternal(string blobName)
        {
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);
            if (blockBlob != null && blockBlob.Exists())
            {
                blockBlob.Delete();
            }
        }

        /// <summary>
        /// Gets a sound blob for the ID.
        /// </summary>
        public CloudBlockBlob GetSoundBlob(string id)
        {
            var blobName = GetBlobSoundName(id);
            return blobContainer.GetBlockBlobReference(blobName);
        }

        /// <summary>
        /// Gets a sample blob for the ID.
        /// </summary>
        public CloudBlockBlob GetSampleBlob(string id)
        {
            var blobName = GetBlobSampleName(id);
            return blobContainer.GetBlockBlobReference(blobName);
        }

        private static string GetBlobSoundName(string id)
        {
            return "sounds/" + id + ".mp3";
        }

        private static string GetBlobSampleName(string id)
        {
            return "samples/" + id + ".mp3";
        }
    }
}