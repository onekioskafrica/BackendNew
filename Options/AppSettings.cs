using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public class AppSettings
    {
        public int LengthOfStoreId { get; set; }
        public int LengthOfOTP { get; set; }
        public int ExpireInDays { get; set; }
        public string AccountCreationOTPMsg { get; set; }
    }
}
