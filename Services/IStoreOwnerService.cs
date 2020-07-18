using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IStoreOwnerService
    {
        Task<AuthenticationResponse> GoogleLoginStoreOwnerAsync(GoogleAuthRequest request);
        Task<AuthenticationResponse> CreateStoreOwnerAsync(StoreOwner storeOwner, string password);
        Task<AuthenticationResponse> LoginStoreOwnerAsync(string email, string password);
        Task<AuthenticationResponse> FacebookLoginStoreOwnerAsync(string accessToken);
    }
}
