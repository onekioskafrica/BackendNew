using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Data;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public ProductService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<GenericResponse> GetAllCategoriesAsync()
        {
            List<Category> allCategories = null;
            try
            {
                allCategories = await _dataContext.Categories.ToListAsync<Category>();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            return new GenericResponse { Status = true, Message = "Success", Data = allCategories };
            
        }
    }
}
