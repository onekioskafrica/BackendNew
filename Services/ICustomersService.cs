using OK_OnBoarding.Contracts.V1;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface ICustomersService
    {
        Task<GenericResponse> ResetPassword(ForgotPasswordRequest request);
        Task<AuthenticationResponse> GoogleLoginCustomerAsync(GoogleAuthRequest request);
        Task<AuthenticationResponse> CreateCustomerAsync(Customer customer, string password);
        Task<AuthenticationResponse> LoginCustomerAsync(string email, string password);
        Task<AuthenticationResponse> FacebookLoginCustomerAsync(string accessToken);
        Task<GenericResponse> UpdateAddressAsync(UpdateAddressRequest request);
    }
}
