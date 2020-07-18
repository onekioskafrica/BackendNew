using AutoMapper;
using OK_OnBoarding.Contracts.V1;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<CustomerSignupRequest, Customer>();
            CreateMap<StoreOwnerSignUpRequest, StoreOwner>();
        }
    }
}
