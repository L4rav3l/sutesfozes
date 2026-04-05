using Amazon.S3;
using Amazon.S3.Model;

namespace SutesFozes.Infrastructure;

public class Cloudflare
{
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;

        public Cloudflare(IConfiguration config)
        {
                var accountId = Environment.GetEnvironmentVariable("R2_ACCOUNTID");

                _bucket = Environment.GetEnvironmentVariable("R2_BUCKET");

                _s3 = new AmazonS3Client(
                        Environment.GetEnvironmentVariable("R2_ACCESS"),
                        Environment.GetEnvironmentVariable("R2_SECRET"),
                        new AmazonS3Config
                        {
                                ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com/",
                                ForcePathStyle = true
                        }
                ); 
        }

        public async Task<string> UploadFile(IFormFile file)
        {
                var key = Guid.NewGuid() + Path.GetExtension(file.FileName);

                using var stream = file.OpenReadStream();

                var request = new PutObjectRequest
                {
                        BucketName = _bucket,
                        Key = key,
                        InputStream = stream,
                        ContentType = file.ContentType,
                        DisablePayloadSigning = true
                };

                await _s3.PutObjectAsync(request);

                return $"https://pub-{_bucket}.r2.dev/{key}";
        }
}