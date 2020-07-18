using Newtonsoft.Json;
using OK_OnBoarding.ExternalContract;
using OK_OnBoarding.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private readonly string UserInfoUrl;
        private readonly string ValidateAccessTokenUrl;

        private readonly FacebookAuthSettings _facebookAuthSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public FacebookAuthService(FacebookAuthSettings facebookAuthSettings, IHttpClientFactory httpClientFactory)
        {
            _facebookAuthSettings = facebookAuthSettings;
            _httpClientFactory = httpClientFactory;
            UserInfoUrl = _facebookAuthSettings.UserInfoUrl;
            ValidateAccessTokenUrl = _facebookAuthSettings.ValidateAccessTokenUrl;
        }

        public async Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            var formattedUrl = string.Format(UserInfoUrl, accessToken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            try
            {
                result.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                return new FacebookUserInfoResult() { Id = "Failed" };
            }
            
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken)
        {
            var formattedUrl = string.Format(ValidateAccessTokenUrl, accessToken, _facebookAuthSettings.AppId, _facebookAuthSettings.AppSecret);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            try
            {
                result.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException ex)
            {
                return new FacebookTokenValidationResult() { Data = null };
            }
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }
    }
}
