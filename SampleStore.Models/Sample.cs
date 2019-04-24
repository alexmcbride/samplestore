using System;

namespace SampleStore.Models
{
    /// <summary>
    /// Sample DTO
    /// </summary>
    public class Sample
    {
        public string SampleId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Mp3Blob { get; set; }
        public string SampleMp3Blob { get; set; }
        public string SampleMp3Url { get; set; }
        public DateTime? SampleDate { get; set; }
    }
}