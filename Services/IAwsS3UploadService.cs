using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IAwsS3UploadService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string directory);
    }
}
