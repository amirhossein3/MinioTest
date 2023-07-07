using Microsoft.AspNetCore.Http;
using Minio;
using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;

namespace MinioTest.Models
{
    public class MinioService
    {
        private readonly MinioClient _minio;


        public MinioService()
        {
            _minio = new MinioClient()
            .WithEndpoint("127.0.0.1:9000")
            .WithCredentials("vC6eSBwI0XVrJPOxIiov",
            "81Wy9o8SsbRf9A7gfgAZ9Vjfyw1r9cq8y0rNlJIq")
            .Build();


        }


        public async Task<string> PutObj(PutObjectRequest request)
        {
            var bucketName = request.bucket;
            // Check Exists bucket
            bool found = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                // if bucket not Exists,make bucket
                await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }
            var filename = Guid.NewGuid();

            var test = request.File.FileName.Substring(request.File.FileName.IndexOf("."));


            using (var ms = new MemoryStream())
            {
                request.File.CopyTo(ms);


                ms.Position = 0;
                // upload object
                await _minio.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filename.ToString() + ".jpg")
                    .WithObjectSize(ms.Length)

                    .WithStreamData(ms)
                    );

            }
            return await Task.FromResult<string>(filename.ToString());
        }


        public async Task<GetObjectReply> GetObject(string bucket, string objectname)
        {
            var destination = new MemoryStream();


                var objstatreply = await _minio.StatObjectAsync(new StatObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(objectname)
                    );



            if (objstatreply == null || objstatreply.DeleteMarker)
                throw new Exception("");



            // Get object
            await _minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectname)
            .WithCallbackStream((stream) =>
            {
                stream.CopyTo(destination);
            }
            )
            );

            destination.Position = 0;

            return await Task.FromResult<GetObjectReply>(new GetObjectReply()
            {
                data = destination,
                objectstat = objstatreply
            });

        }

    }
}
