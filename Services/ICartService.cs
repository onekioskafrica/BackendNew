using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface ICartService
    {
        Task<GenericResponse> CreateCartAsync(Guid CustomerId, String SessionId);
        Task<GenericResponse> AddCartItemAsync(AddCartItemRequest request);
        Task<GenericResponse> RemoveCartItemAsync(int cartItemId);
        Task<GenericResponse> GetCartAsync(int cartId);
        Task<GenericResponse> CheckoutAsync(CheckoutRequest request);
    }
}
