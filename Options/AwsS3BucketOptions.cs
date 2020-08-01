using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Options
{
    public class AwsS3BucketOptions
    {
        public string BucketName { get; set; }
        public string Region { get; set; }
        public string ProductFolderName { get; set; }
        public string StoreLogoFolderName { get; set; }
        public string CredentialsFolderName { get; set; }
    }
}
