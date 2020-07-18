using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OK_OnBoarding.Options;
using OK_OnBoarding.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Installers
{
    public class FacebookAuthInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            var faceAuthSettings = new FacebookAuthSettings();
            configuration.Bind(nameof(FacebookAuthSettings), faceAuthSettings);
            services.AddSingleton(faceAuthSettings);

            services.AddHttpClient();
            services.AddSingleton<IFacebookAuthService, FacebookAuthService>();
        }
    }
}
