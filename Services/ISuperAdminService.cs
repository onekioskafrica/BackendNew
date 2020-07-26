using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface ISuperAdminService
    {
        Task<AuthenticationResponse> CreateSuperAdminAsync(SuperAdmin superAdmin, string password);
        Task<GenericResponse> SuperAdminChangePasswordAsync(int superAdminId, string oldPassword, string newPassword);
        Task<AuthenticationResponse> LoginSuperAdminAsync(string email, string password);
        Task<GenericResponse> CreateAdminAsync(Admin admin, string password);
        Task<GenericResponse> DeactivateAdminAsync(Guid adminId);
        Task<GenericResponse> GetAllPrivilegesAsync();
        Task<GenericResponse> GetAdminByIdAsync();
        Task<GenericResponse> GetAllAdminsAsync();
        Task<GenericResponse> GetAllAdminsActivityLogAsync();
        Task<GenericResponse> GetSingleAdminAcitvityLogAsync();
    }
}
