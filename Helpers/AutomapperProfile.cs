using AutoMapper;
using OK_OnBoarding.Contracts.V1;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
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
            CreateMap<DeliverymanSignUpRequest, Deliveryman>();
            CreateMap<StoreOwner, UserDataResponse>();
            CreateMap<Customer, CustomerUserDataResponse>();
            CreateMap<Deliveryman, UserDataResponse>();
            CreateMap<CreateSuperAdminRequest, SuperAdmin>();
            CreateMap<SuperAdmin, SuperAdminUserDataResponse>();
            CreateMap<CreateAdminRequest, Admin>();
            CreateMap<Admin, AdminUserDataResponse>();
            CreateMap<CreateStoreBasicRequest, Store>();
            CreateMap<CreateStoreBusinessInfoRequest, StoresBusinessInformation>();
            CreateMap<CreateStoreBankAccountInformationRequest, StoresBankAccount>();
            CreateMap<Store, StoreCreationDataResponse>();
            CreateMap<StoresBusinessInformation, StoresBusinessInfoDataResponse>();
            CreateMap<StoresBankAccount, StoresBankInformationDataResponse>();
        }
    }
}
