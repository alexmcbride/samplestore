using SampleStore.Models;
using System.Web.Http;

namespace SampleStore.WebApi.Controllers
{
    /// <summary>
    /// Samples RESTful API controller.
    /// </summary>
    public class SamplesController : ApiController
    {
        private readonly SamplesRepository samplesRepository = new SamplesRepository();
        private readonly BlobRepository blobRepository = new BlobRepository();

        // GET: api/samples
        public IHttpActionResult Get()
        {
            return Ok(samplesRepository.GetAll());
        }

        // GET: api/samples/5
        public IHttpActionResult Get(string id)
        {
            var sample = samplesRepository.Get(id);
            if (sample != null)
            {
                return Ok(sample);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/samples
        public IHttpActionResult Post([FromBody]Sample sample)
        {
            // Add sample to database.
            samplesRepository.Add(sample);

            // Return location and json
            var url = Url.Link("DefaultApi", new { Id = sample.SampleId });
            return Created(url, sample);
        }

        // PUT: api/samples/5
        public IHttpActionResult Put(string id, [FromBody]Sample sample)
        {
            sample = samplesRepository.Update(id, sample);
            if (sample != null)
            {
                return Ok(sample);
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE: api/samples/5
        public IHttpActionResult Delete(string id)
        {
            if (samplesRepository.Delete(id))
            {
                // After deleteing sample delete blobs in storage.
                blobRepository.DeleteBlobs(id);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // OPTIONS: api/samples
        [AcceptVerbs("OPTIONS")]
        public IHttpActionResult Options()
        {
            var result = Ok();
            result.Request.Headers.Add("Access-Control-Allow-Origin", "*");
            result.Request.Headers.Add("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE");
            return result;
        }
    }
}
