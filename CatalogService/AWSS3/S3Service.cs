using Amazon.S3;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace CatalogService.AWSS3
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _s3Settings;

        public S3Service(IAmazonS3 s3Client, IOptions<S3Settings> s3Settings)
        {
            _s3Client = s3Client;
            _s3Settings = s3Settings.Value;
        }

        public async Task<string> UploadFileAsync(Stream fileStream)
        {
            var key = Guid.NewGuid();
            var putRequest = new Amazon.S3.Model.PutObjectRequest
            {
                InputStream = fileStream,
                BucketName = _s3Settings.BucketName,
                Key = $"images/pies/{key}"
            };

            await _s3Client.PutObjectAsync(putRequest);
            return $"https://{_s3Settings.BucketName}.s3.amazonaws.com/{$"images/pies/{key}"}";
        }
    }
}
