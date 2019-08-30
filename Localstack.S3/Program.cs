using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Localstack.S3
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Start S3 test");
            AmazonS3Config config = new AmazonS3Config()
            {
                //uncomment to use localstack
                ServiceURL = "http://localhost:4572",
                ForcePathStyle = true,
            };
            AmazonS3Client client = new AmazonS3Client(config);

            S3TestClient testClient = new S3TestClient(client);

            await testClient.CreateBucketIfNotExistsAsync();
            string objectKey = await testClient.PutObjectAsync();
            string url = testClient.GetObjectUrl(objectKey);

            Console.WriteLine($"Uploaded file url: {url}");
        }
    }
}