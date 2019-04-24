using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ComponentModel.DataAnnotations;

namespace SampleStore.Models
{
    /// <summary>
    /// Sample entity object
    /// </summary>
    public class SampleEntity : TableEntity
    {
        [Key]
        public string SampleId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Mp3Blob { get; set; }
        public string SampleMp3Blob { get; set; }
        public string SampleMp3Url { get; set; }
        public DateTime? SampleDate { get; set; }

        public SampleEntity()
        {
            // Left blank
        }

        /// <summary>
        /// Creates a new SampleEntity object.
        /// </summary>
        /// <param name="partitionKey">The partion key for azure storage</param>
        /// <param name="sampleId">The ID to give the RowKey and SampleID</param>
        public SampleEntity(string partitionKey, string sampleId)
        {
            PartitionKey = partitionKey;
            SampleId = RowKey = sampleId;
        }
    }
}
