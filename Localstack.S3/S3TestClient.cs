using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Localstack.S3
{
    public class S3TestClient
    {
        private readonly AmazonS3Client _client;
        private const string BucketName = "localstack-fancy-bucket";
        private const string FileName = "hac.png";

        public S3TestClient(AmazonS3Client client)
        {
            _client = client;
        }

        public async Task CreateBucketIfNotExistsAsync()
        {
            var listBucketsResponse = await _client.ListBucketsAsync();
            if (!listBucketsResponse.Buckets.Exists(_ => _.BucketName == BucketName))
            {
                await _client.PutBucketAsync(BucketName);
            }
        }

        public async Task<string> PutObjectAsync()
        {
            var objectKey = $"{Guid.NewGuid().ToString()}-{FileName}";
            var request = new PutObjectRequest()
            {
                BucketName = BucketName,
                FilePath = FileName,
                Key = objectKey,
            };

            await _client.PutObjectAsync(request);
            return objectKey;
        }

        public string GetObjectUrl(string objectKey)
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = BucketName,
                Expires = DateTime.Now.AddMinutes(5),
                Key = objectKey,
                Protocol = Protocol.HTTP,
            };

            return _client.GetPreSignedURL(request);
        }
    }
}