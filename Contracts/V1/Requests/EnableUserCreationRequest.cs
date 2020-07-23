using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class EnableUserCreationRequest
    {
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
    }
}
