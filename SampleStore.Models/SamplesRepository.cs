using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleStore.Models
{
    /// <summary>
    /// Repository to wrap the azure sample tables.
    /// </summary>
    public class SamplesRepository
    {
        private const string PartitionKey = "samplePartition1";
        private const string TableName = "Samples";
        private const string ConnectionStringName = "AzureWebJobsStorage";
        private readonly CloudTable table;

        /// <summary>
        /// Creates a new SamplesRepository.
        /// </summary>
        public SamplesRepository()
        {
            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringName);
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(TableName);
            table.CreateIfNotExists();
        }

        /// <summary>
        /// Gets all samples from the table.
        /// </summary>
        public IEnumerable<Sample> GetAll()
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey);
            var query = new TableQuery<SampleEntity>().Where(filter);
            return table.ExecuteQuery(query).Select(CreateSample);
        }

        private SampleEntity GetEntity(string id)
        {
            var operation = TableOperation.Retrieve<SampleEntity>(PartitionKey, id);
            var result = table.Execute(operation);
            return result.Result as SampleEntity;
        }

        private void UpdateEntity(SampleEntity entity)
        {
            table.Execute(TableOperation.Replace(entity));
        }

        /// <summary>
        /// Gets a single sample from the table.
        /// </summary>
        /// <param name="id">The ID of the sample.</param>
        /// <returns>The sample.</returns>
        public Sample Get(string id)
        {
            var entity = GetEntity(id);
            return CreateSample(entity);
        }

        /// <summary>
        /// Adds a sample to the database. Once added object will be populated with SampleId and CreatedDate.
        /// </summary>
        /// <param name="sample">The sample to add.</param>
        public void Add(Sample sample)
        {
            sample.SampleId = sample.SampleId ?? Guid.NewGuid().ToString();
            sample.CreatedDate = DateTime.Now;
            var entity = CreateSampleEntity(sample);
            var operation = TableOperation.Insert(entity);
            table.Execute(operation);
        }

        /// <summary>
        /// Updates sample in the table.
        /// </summary>
        /// <param name="id">Id of sample to update.</param>
        /// <param name="sample">Sample to update table with.</param>
        /// <returns>The updated sample</returns>
        public Sample Update(string id, Sample sample)
        {
            var entity = GetEntity(id);
            if (entity != null)
            {
                entity.Title = sample.Title;
                entity.Artist = sample.Artist;
                UpdateEntity(entity);
                return CreateSample(entity);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates mp3 blob name of a sample.
        /// </summary>
        /// <param name="id">The ID of the sample to update</param>
        /// <param name="blobName">The new blob name</param>
        /// <returns>A sample entity object that can be queued.</returns>
        public SampleEntity UpdateBlobName(string id, string blobName)
        {
            var sampleEntity = GetEntity(id);
            sampleEntity.Mp3Blob = blobName;
            UpdateEntity(sampleEntity);
            return sampleEntity;
        }

        /// <summary>
        /// Deletes a sample from the table.
        /// </summary>
        /// <param name="id">The ID of the sample to delete.</param>
        /// <returns>True if the sample was found.</returns>
        public bool Delete(string id)
        {
            var entity = GetEntity(id);
            if (entity != null)
            {
                table.Execute(TableOperation.Delete(entity));
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Sample CreateSample(SampleEntity entity)
        {
            return new Sample
            {
                SampleId = entity.SampleId,
                Title = entity.Title,
                Artist = entity.Artist,
                CreatedDate = entity.CreatedDate,
                Mp3Blob = entity.Mp3Blob,
                SampleMp3Blob = entity.SampleMp3Blob,
                SampleMp3Url = entity.SampleMp3Url,
                SampleDate = entity.SampleDate
            };
        }

        private static SampleEntity CreateSampleEntity(Sample sample)
        {
            return new SampleEntity(PartitionKey, sample.SampleId)
            {
                SampleId = sample.SampleId,
                Title = sample.Title,
                Artist = sample.Artist,
                CreatedDate = sample.CreatedDate,
                Mp3Blob = sample.Mp3Blob,
                SampleMp3Blob = sample.SampleMp3Blob,
                SampleMp3Url = sample.SampleMp3Url,
                SampleDate = sample.SampleDate
            };
        }
    }
}