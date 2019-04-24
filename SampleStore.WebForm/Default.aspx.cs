using Newtonsoft.Json;
using SampleStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SampleStore.WebForm
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly BlobRepository blobRepository = new BlobRepository();
        private readonly QueueHelper queueHelper = new QueueHelper();
        private readonly SamplesRepository samplesRepository = new SamplesRepository();

        /// <summary>
        /// Gets list of samples without sample MP3 URLs
        /// </summary>
        public IEnumerable<Sample> GetSamplesWithUploads()
        {
            return samplesRepository.GetAll()
                .Where(s => !string.IsNullOrEmpty(s.SampleMp3Blob));
        }

        /// <summary>
        /// Gets list of samples without uploaded MP3 URLs (for the sample dropdown)
        /// </summary>
        public IEnumerable<object> GetSamplesWithoutUploads()
        {
            return samplesRepository.GetAll()
                .Where(s => string.IsNullOrEmpty(s.Mp3Blob))
                .Select(s =>
                {
                    return new
                    {
                        Text = s.Title + " (" + s.SampleId + ")",
                        Value = s.SampleId,
                        s.Artist
                    };
                });
        }

        /// <summary>
        /// Check form state is valid then upload file
        /// </summary>
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (FileUpload.HasFile)
            {
                string id = IdDropDown.SelectedItem.Value;
                if (!string.IsNullOrEmpty(id))
                {
                    UploadFile(FileUpload.PostedFile, id);
                }
                else
                {
                    MessageLabel.Text = "Select sample from drop down";
                }
            }
            else
            {
                MessageLabel.Text = "No file uploaded";
            }
        }

        /// <summary>
        /// Upload file to blob storage, update sample with new blob name, then queue message for web job.
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="id">The id of the sample to upload the file for</param>
        private void UploadFile(HttpPostedFile file, string id)
        {
            // Add file to blob storage.
            var blobName = blobRepository.AddSoundBlob(file, id);

            // Update blob name in sample
            var sampleEntity = samplesRepository.UpdateBlobName(id, blobName);

            // Queue entity to trigger web job
            queueHelper.QueueMessage(JsonConvert.SerializeObject(sampleEntity));

            // Yay!
            MessageLabel.Text = "Uploaded successfully (now wait for Web Job to complete)";
            IdDropDown.Text = string.Empty;
        }

        /// <summary>
        /// Refresh the sample list
        /// </summary>
        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            // Cause page to refresh.
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }
    }
}