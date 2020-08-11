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
    public interface IDelivermanService
    {
        Task<GenericResponse> ResetPassword(ForgotPasswordRequest request);

        Task<AuthenticationResponse> GoogleLoginDeliverymanAsync(GoogleAuthRequest request);
        Task<AuthenticationResponse> CreateDeliverymanAsync(Deliveryman deliveryman, string password);
        Task<AuthenticationResponse> LoginDeliverymanAsync(string email, string password);
        Task<AuthenticationResponse> FacebookLoginDeliverymanAsync(string accessToken);
        Task<GenericResponse> UpdateAddressAsync(UpdateAddressRequest request);
        Task<GenericResponse> UpdateGeneralInformationAsync(DeliverymanGeneralInfoRequest request);
        Task<GenericResponse> UploadDocumentsAsync(DeliverymanUploadDocumentsRequest request);
    }
}
