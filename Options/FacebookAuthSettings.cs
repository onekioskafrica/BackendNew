using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Options
{
    public class FacebookAuthSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string UserInfoUrl { get; set; }
        public string ValidateAccessTokenUrl { get; set; }
    }
}
