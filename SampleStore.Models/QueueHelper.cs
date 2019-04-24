using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Text;

namespace SampleStore.Models
{
    /// <summary>
    /// Helper for dealing with queues.
    /// </summary>
    public class QueueHelper
    {
        private readonly CloudQueue cloudQueue;

        /// <summary>
        /// Creates a new QueueHelper.
        /// </summary>
        public QueueHelper()
        {
            var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
            var cloudStorage = CloudStorageAccount.Parse(connectionString);
            var queueClient = cloudStorage.CreateCloudQueueClient();
            cloudQueue = queueClient.GetQueueReference("soundqueue");
            cloudQueue.CreateIfNotExists();
        }

        /// <summary>
        /// Queues a string message.
        /// </summary>
        /// <param name="payload"></param>
        public void QueueMessage(string payload)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            var message = new CloudQueueMessage(bytes);
            cloudQueue.AddMessage(message);
        }
    }
}