using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IAdminService
    {
        Task<AuthenticationResponse> LoginAdminAsync(string email, string password);
        Task<GenericResponse> CreateAdminAsync(Admin admin, string password, Guid callerId);
        Task<GenericResponse> ChangePassword(AdminChangePasswordRequest request);
    }
}
