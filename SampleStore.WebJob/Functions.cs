using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Table;
using NAudio.Wave;
using NLayer.NAudioSupport;
using SampleStore.Models;
using System;
using System.IO;

namespace SampleStore.WebJob
{
    public class Functions
    {
        private const int SampleLengthSeconds = 20;
        private static readonly BlobRepository blobRepository = new BlobRepository();

        public static void GenerateSample(
            [QueueTrigger("soundqueue")] SampleEntity sampleInQueue,
            [Table("Samples", "{PartitionKey}", "{RowKey}")] SampleEntity sampleInTable,
            [Table("Samples")] CloudTable tableBinding, TextWriter logger)
        {
            logger.Write($"Generating sample for '{sampleInQueue.Title}' ({sampleInQueue.SampleId})... ");

            // Get sound and sample blobs from Azure storage.
            var soundBlob = blobRepository.GetSoundBlob(sampleInQueue.SampleId);
            var sampleBlob = blobRepository.GetSampleBlob(sampleInQueue.SampleId);

            // Make sure source blob exists for sample.
            if (!soundBlob.Exists())
            {
                logger.WriteLine($"blob not found for ID: '{sampleInQueue.SampleId}'");
                return;
            }

            // Re-sample sound blob into sample blob
            using (var input = soundBlob.OpenRead())
            using (var output = sampleBlob.OpenWrite())
            {
                CreateSample(input, output, SampleLengthSeconds);
            }

            // Update sample in table with new sample data
            sampleInQueue.SampleDate = DateTime.Now;
            sampleInQueue.SampleMp3Url = sampleBlob.Uri.ToString();
            sampleInQueue.SampleMp3Blob = sampleBlob.Name;
            tableBinding.Execute(TableOperation.Replace(sampleInQueue));

            // yay
            logger.WriteLine("done!");
        }

        private static void CreateSample(Stream input, Stream output, int duration)
        {
            using (var reader = new Mp3FileReader(input, wave => new Mp3FrameDecompressor(wave)))
            {
                var frame = reader.ReadNextFrame();
                int frameTimeLength = (int)(frame.SampleCount / (double)frame.SampleRate * 1000.0);
                int framesRequired = (int)(duration / (double)frameTimeLength * 1000.0);

                int frameNumber = 0;
                while ((frame = reader.ReadNextFrame()) != null)
                {
                    frameNumber++;

                    if (frameNumber <= framesRequired)
                    {
                        output.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
