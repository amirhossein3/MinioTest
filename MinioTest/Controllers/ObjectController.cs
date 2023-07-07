using Microsoft.AspNetCore.Mvc;
using Minio.DataModel;
using MinioTest.Models;

namespace MinioTest.Controllers
{
    public class ObjectController : Controller
    {
        private readonly ILogger<ObjectController> _logger;
        private readonly MinioService _minio;
        public ObjectController(ILogger<ObjectController> logger, MinioService minio)
        {
            _logger = logger;
            _minio = minio;
        }


        [HttpGet]
        public async Task<ActionResult> Get(string objectname, string bucket)
        {
            var result = await _minio.GetObject(bucket.ToString(), objectname);
            return File(result.data, result.objectstat.ContentType);
        }



        public IActionResult upload()
        {
            return View();
        }

        public  async Task< IActionResult> GetImage()
        {

            var dd =await _minio.GetObject("output", "353fec96-5bab-4a92-99c4-80814313eace.jpg");


            return File(dd.data, "image/jpeg");
        }


        [HttpPost]
        public async Task<ActionResult> Post(IFormFile request)
        {

            using (var ms = new MemoryStream())
            {
                await request.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
            }

            var result = await _minio.PutObj(new PutObjectRequest()
            {
                bucket = "output",
                File = request
            });
            return Ok(new { filename = result });
        }
    }
}
