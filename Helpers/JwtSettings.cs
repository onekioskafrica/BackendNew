using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
    }
}
