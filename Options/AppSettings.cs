using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public class AppSettings
    {
        public int LengthOfStoreId { get; set; }
        public int LengthOfOTP { get; set; }
        public int LengthOfGeneratedPassword { get; set; }
        public int ExpireInDays { get; set; }
        public string AccountCreationOTPMsg { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ExtraShippingForBuyingFromMultipleStores { get; set; }

        public int LengthOfCouponCode { get; set; }
    }
}
