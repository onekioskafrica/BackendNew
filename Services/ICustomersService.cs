using OK_OnBoarding.Contracts.V1;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface ICustomersService
    {
        Task<AuthenticationResponse> CreateCustomerAsync(Customer customer, string password);
        Task<AuthenticationResponse> LoginCustomerAsync(string email, string password);
    }
}
