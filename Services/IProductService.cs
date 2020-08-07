using OK_OnBoarding.Contracts.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IProductService
    {
        Task<GenericResponse> GetAllCategoriesAsync();
    }
}
