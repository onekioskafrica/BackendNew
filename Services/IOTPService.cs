﻿using OK_OnBoarding.Contracts.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IOTPService
    {
        Task<GenericResponse> GenerateOTPForCustomer(string tokenGenerationReason, string phoneNumber, string email);
        Task<GenericResponse> GenerateOTPForStoreOwner(string tokenGenerationReason, string phoneNumber, string email);
        Task<GenericResponse> GenerateOTPForDeliveryman(string tokenGenerationReason, string phoneNumber, string email);
        Task<GenericResponse> VerifyOTPForStoreOwner(string OTP, string PhoneNumber);
        Task<GenericResponse> VerifyOTPForDeliveryman(string OTP, string PhoneNumber);
        Task<GenericResponse> VerifyOTPForCustomer(string OTP, string PhoneNumber);
        string GenerateOTP();
    }
}
