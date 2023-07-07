namespace MinioTest.Models
{
    public class PutObjectRequest
    {
        public string bucket { get; set; }

        public IFormFile File { get; set; }
    }
}
