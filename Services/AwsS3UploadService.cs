using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using OK_OnBoarding.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class AwsS3UploadService : IAwsS3UploadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsS3BucketOptions _s3BucketOptions;
        private readonly ILogger<AwsS3UploadService> _logger;

        public AwsS3UploadService(IAmazonS3 s3Client, AwsS3BucketOptions s3BucketOptions, ILogger<AwsS3UploadService> logger)
        {
            _s3Client = s3Client;
            _s3BucketOptions = s3BucketOptions;
            _logger = logger;
        }
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string directory)
        {
            try
            {
                _logger.LogInformation($"About to upload {fileName} to AWS S3 Bucket.");
                var fileTransferUtility = new TransferUtility(_s3Client);
                var bucketPath = !string.IsNullOrWhiteSpace(directory) ? _s3BucketOptions.BucketName + @"/" + directory : _s3BucketOptions.BucketName;

                var _key = Guid.NewGuid().ToString() + "_" + fileName;

                var fileUploadRequest = new TransferUtilityUploadRequest() { 
                    CannedACL = S3CannedACL.BucketOwnerFullControl,
                    BucketName = bucketPath,
                    PartSize = 6291456, // 6 MB.
                    Key = _key,
                    InputStream = fileStream
                };
                _logger.LogInformation($"About to call AWSSDK - UploadAsync.");
                await fileTransferUtility.UploadAsync(fileUploadRequest);

                return $"https://{_s3BucketOptions.BucketName}.s3-{_s3BucketOptions.Region}.amazonaws.com/{directory}/{_key}";
            }
            catch(AmazonS3Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }
        }
    }
}
