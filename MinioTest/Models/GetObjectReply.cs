using Minio.DataModel;

namespace MinioTest.Models
{
    public class GetObjectReply
    {
        public ObjectStat objectstat { get; set; }
        public MemoryStream data { get; set; }
    }
}
