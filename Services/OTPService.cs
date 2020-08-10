using Microsoft.EntityFrameworkCore;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Data;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
using OK_OnBoarding.Helpers;
using OK_OnBoarding.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class OTPService : IOTPService
    {
        private readonly AppSettings _appSettings;
        private readonly DataContext _dataContext;
        private readonly TermiiAuthSettings _termiiAuthSettings;

        public OTPService(AppSettings appSettings, DataContext dataContext, TermiiAuthSettings termiiAuthSettings)
        {
            _appSettings = appSettings;
            _dataContext = dataContext;
            _termiiAuthSettings = termiiAuthSettings;
        }

        public async Task<GenericResponse> VerifyOTPForDeliveryman(string OTP, string PhoneNumber)
        {
            GenericResponse genericResponse = new GenericResponse();

            var deliverymanExist = await _dataContext.DeliveryMen.FirstOrDefaultAsync(d => d.PhoneNumber == PhoneNumber.Trim());

            if(deliverymanExist == null)
            {
                genericResponse.Status = false;
                genericResponse.Message = "Invalid phonenumber";
            }
            else
            {
                var tokenExist = await _dataContext.DeliverymenTokens.Where(d => d.DeliverymanId == deliverymanExist.Id && d.TheToken == OTP).OrderByDescending(d => d.DateCreated).FirstOrDefaultAsync();
                if(tokenExist == null)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Invalid token";
                    return genericResponse;
                }
                if(tokenExist.IsUsed == true)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Used Token";
                    return genericResponse;
                }
                if (tokenExist.ExpiryDate <= DateTime.Now)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Expired Token";
                    return genericResponse;
                }
                tokenExist.IsUsed = true;
                _dataContext.Entry(tokenExist).State = EntityState.Modified;
                var updated = await _dataContext.SaveChangesAsync();
                if (updated <= 0)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Error Occurred.";
                }
                else
                {
                    deliverymanExist.IsVerified = true;
                    _dataContext.Entry(deliverymanExist).State = EntityState.Modified;
                    var isVerified = await _dataContext.SaveChangesAsync();
                    if (isVerified <= 0)
                    {
                        genericResponse.Status = false;
                        genericResponse.Message = "Couldn't verify Deliveryman";
                    }
                    else
                    {
                        genericResponse.Status = true;
                        genericResponse.Message = "Valid token";
                    }

                }
            }
            return genericResponse;
        }

        public async Task<GenericResponse> VerifyOTPForStoreOwner(string OTP, string PhoneNumber)
        {
            GenericResponse genericResponse = new GenericResponse();

            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.PhoneNumber == PhoneNumber.Trim());

            if(storeOwnerExist == null)
            {
                genericResponse.Status = false;
                genericResponse.Message = "Invalid phonenumber";
            }
            else
            {
                var tokenExist = await _dataContext.StoreOwnerTokens.Where(s => s.StoreOwnerId == storeOwnerExist.Id && s.TheToken == OTP).OrderByDescending(s => s.DateCreated).FirstOrDefaultAsync();
                if(tokenExist == null)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Invalid token";
                    return genericResponse;
                }
                if(tokenExist.IsUsed == true)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Used Token";
                    return genericResponse;
                }
                if(tokenExist.ExpiryDate <= DateTime.Now)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Expired Token";
                    return genericResponse;
                }

                tokenExist.IsUsed = true;
                _dataContext.Entry(tokenExist).State = EntityState.Modified;
                var updated = await _dataContext.SaveChangesAsync();
                if(updated <= 0)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Error Occurred.";
                }
                else
                {
                    storeOwnerExist.IsVerified = true;
                    _dataContext.Entry(storeOwnerExist).State = EntityState.Modified;
                    var isVerified = await _dataContext.SaveChangesAsync();
                    if(isVerified <= 0)
                    {
                        genericResponse.Status = false;
                        genericResponse.Message = "Couldn't verify StoreOwner";
                    }
                    else
                    {
                        genericResponse.Status = true;
                        genericResponse.Message = "Valid token";
                    }
                    
                }
            }
            return genericResponse;
        }

        public async Task<GenericResponse> VerifyOTPForCustomer(string OTP, string PhoneNumber)
        {
            GenericResponse genericResponse = new GenericResponse();

            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(s => s.PhoneNumber == PhoneNumber.Trim());

            if (customerExist == null)
            {
                genericResponse.Status = false;
                genericResponse.Message = "Invalid phonenumber";
            }
            else
            {
                var tokenExist = await _dataContext.CustomerTokens.Where(c => c.CustomerId == customerExist.CustomerId && c.TheToken == OTP).OrderByDescending(c => c.DateCreated).FirstOrDefaultAsync();
                if (tokenExist == null)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Invalid token";
                    return genericResponse;
                }
                if (tokenExist.IsUsed == true)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Used Token";
                    return genericResponse;
                }
                if (tokenExist.ExpiryDate <= DateTime.Now)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Expired Token";
                    return genericResponse;
                }

                tokenExist.IsUsed = true;
                _dataContext.Entry(tokenExist).State = EntityState.Modified;
                var updated = await _dataContext.SaveChangesAsync();
                if (updated <= 0)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Error Occurred.";
                }
                else
                {
                    customerExist.IsVerified = true;
                    _dataContext.Entry(customerExist).State = EntityState.Modified;
                    var isVerified = await _dataContext.SaveChangesAsync();
                    if (isVerified <= 0)
                    {
                        genericResponse.Status = false;
                        genericResponse.Message = "Couldn't verify Customer";
                    }
                    else
                    {
                        genericResponse.Status = true;
                        genericResponse.Message = "Valid token";
                    }

                }
            }
            return genericResponse;
        }

        public async Task<GenericResponse> GenerateOTPForStoreOwner(string tokenGenerationReason, string phoneNumber, string email)
        {
            GenericResponse genericResponse = new GenericResponse();

            var storeOwnerExist = await _dataContext.StoreOwners.Where(s => s.Email == email.Trim() && s.PhoneNumber == phoneNumber.Trim()).Select(s => new StoreOwner { Id = s.Id }).FirstOrDefaultAsync();

            if (storeOwnerExist == null)
            {
                genericResponse.Status = false;
                genericResponse.Message = "Invalid Phonenumber/Email";
            }
            else
            {
                var token = new StoreOwnerToken()
                {
                    StoreOwnerId = storeOwnerExist.Id,
                    DateCreated = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(_appSettings.ExpireInDays),
                    IsUsed = false,
                    TheToken = GenerateOTP(),
                    StatusOperation = tokenGenerationReason
                };
                await _dataContext.StoreOwnerTokens.AddAsync(token);
                var created = 0;
                try
                {
                    created = await _dataContext.SaveChangesAsync();
                }
                catch(Exception)
                {
                    return new GenericResponse { Status = false, Message = "Error Occurred." };
                }
                if(created <= 0)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Couldn't generate OTP";
                }
                else
                {
                    genericResponse.Status = true;
                    genericResponse.Message = token.TheToken;
                }
            }
            return genericResponse;
        }

        public async Task<GenericResponse> GenerateOTPForDeliveryman(string tokenGenerationReason, string phoneNumber, string email)
        {
            GenericResponse genericResponse = new GenericResponse();

            var deliverymanExist = await _dataContext.DeliveryMen.Where(d => d.Email == email && d.PhoneNumber == phoneNumber).Select(d => new Deliveryman { Id = d.Id }).FirstOrDefaultAsync();

            if (deliverymanExist == null)
            {
                genericResponse.Status = false;
                genericResponse.Message = "Invalid Phonenumber/Email";
            }
            else
            {
                var token = new DeliverymanToken()
                {
                    DeliverymanId = deliverymanExist.Id,
                    DateCreated = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(_appSettings.ExpireInDays),
                    IsUsed = false,
                    TheToken = GenerateOTP(),
                    StatusOperation = tokenGenerationReason
                };
                await _dataContext.DeliverymenTokens.AddAsync(token);
                var created = 0;
                try
                {
                    created = await _dataContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return new GenericResponse { Status = false, Message = "Error Occurred." };
                }
                if (created <= 0)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Couldn't generate OTP";
                }
                else
                {
                    genericResponse.Status = true;
                    genericResponse.Message = token.TheToken;
                }
            }
            return genericResponse;
        }

        public async Task<GenericResponse> GenerateOTPForCustomer(string tokenGenerationReason, string phoneNumber, string email)
        {
            GenericResponse genericResponse = new GenericResponse();

            var customerExist = await _dataContext.Customers.Where(c => c.Email == email && c.PhoneNumber == phoneNumber).Select(c => new Customer { CustomerId = c.CustomerId }).FirstOrDefaultAsync();
            
            if(customerExist == null)
            {
                genericResponse.Status = false;
                genericResponse.Message = "Invalid Phonenumber/Email";
            }
            else
            {
                var token = new CustomerToken()
                {
                    CustomerId = customerExist.CustomerId,
                    DateCreated = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(_appSettings.ExpireInDays),
                    IsUsed = false,
                    TheToken = GenerateOTP(),
                    StatusOperation = tokenGenerationReason
                };
                await _dataContext.CustomerTokens.AddAsync(token);
                var created = 0;
                try
                {
                    created = await _dataContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return new GenericResponse { Status = false, Message = "Error Occurred." };
                }
                if(created <= 0)
                {
                    genericResponse.Status = false;
                    genericResponse.Message = "Couldn't generate OTP";
                }
                else
                {
                    genericResponse.Status = true;
                    genericResponse.Message = token.TheToken;
                }
            }
            return genericResponse;
        }

        public async Task<GenericResponse> ResendOTPForStoreOwner(string tokenGenerationReason, string phoneNumber, string email)
        {
            var storeOwnerExist = await _dataContext.StoreOwners.Where(s => s.Email == email.Trim() && s.PhoneNumber == phoneNumber.Trim()).Select(s => new StoreOwner { Id = s.Id, IsVerified = s.IsVerified }).FirstOrDefaultAsync();

            if (storeOwnerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid PhoneNumber/Email" };
            if (storeOwnerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Phone number already verified." };

            var token = new StoreOwnerToken()
            {
                StoreOwnerId = storeOwnerExist.Id,
                DateCreated = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(_appSettings.ExpireInDays),
                IsUsed = false,
                TheToken = GenerateOTP(),
                StatusOperation = tokenGenerationReason
            };
            await _dataContext.StoreOwnerTokens.AddAsync(token);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't generate OTP" };

            var termiiResponse = await SendSms(phoneNumber, token.TheToken);

            if (termiiResponse.Code.ToLower() == "ok")
                return new GenericResponse { Status = true, Message = "OTP has been sent to your phone number." };

            return new GenericResponse { Status = false, Message = "Couldn't resend OTP." };

        }

        public async Task<GenericResponse> ResendOTPForDeliveryman(string tokenGenerationReason, string phoneNumber, string email)
        {
            var deliverymanExist = await _dataContext.DeliveryMen.Where(d => d.Email == email && d.PhoneNumber == phoneNumber).Select(d => new Deliveryman { Id = d.Id, IsVerified = d.IsVerified }).FirstOrDefaultAsync();

            if (deliverymanExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Phonenumber/Email" };
            if (deliverymanExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Phone number already verified." };

            var token = new DeliverymanToken()
            {
                DeliverymanId = deliverymanExist.Id,
                DateCreated = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(_appSettings.ExpireInDays),
                IsUsed = false,
                TheToken = GenerateOTP(),
                StatusOperation = tokenGenerationReason
            };
            await _dataContext.DeliverymenTokens.AddAsync(token);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't generate OTP" };

            var termiiResponse = await SendSms(phoneNumber, token.TheToken);

            if (termiiResponse.Code.ToLower() == "ok")
                return new GenericResponse { Status = true, Message = "OTP has been sent to your phone number." };

            return new GenericResponse { Status = false, Message = "Couldn't resend OTP." };

        }

        public async Task<GenericResponse> ResendOTPForCustomer(string tokenGenerationReason, string phoneNumber, string email)
        {
            var customerExist = await _dataContext.Customers.Where(c => c.Email == email && c.PhoneNumber == phoneNumber).Select(c => new Customer { CustomerId = c.CustomerId, IsVerified = c.IsVerified }).FirstOrDefaultAsync();

            if (customerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Phonenumber/Email" };
            if (customerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Phone number already verified." };

            var token = new CustomerToken()
            {
                CustomerId = customerExist.CustomerId,
                DateCreated = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(_appSettings.ExpireInDays),
                IsUsed = false,
                TheToken = GenerateOTP(),
                StatusOperation = tokenGenerationReason
            };
            await _dataContext.CustomerTokens.AddAsync(token);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't generate OTP" };

            var termiiResponse = await SendSms(phoneNumber, token.TheToken);

            if (termiiResponse.Code.ToLower() == "ok")
                return new GenericResponse { Status = true, Message = "OTP has been sent to your phone number." };

            return new GenericResponse { Status = false, Message = "Couldn't resend OTP." };
        }

        public string GenerateOTP()
        {
            var otp = string.Empty;
            int lengthOfOTP = _appSettings.LengthOfOTP;
            var charArray =  Guid.NewGuid().ToString().ToCharArray();
            foreach(var item in charArray)
            {
                int n;
                if(int.TryParse(item.ToString(), out n))
                {
                    otp += item.ToString();
                }
            }
            otp = otp.Substring(0, lengthOfOTP);
            return otp;
        }

        public async Task<TermiiResponse> SendSms(string phoneNumber, string theToken)
        {
            // Send Sms 
            var termiiRequest = new TermiiRequest()
            {
                To = phoneNumber,
                From = _termiiAuthSettings.SenderId,
                Sms = _appSettings.AccountCreationOTPMsg.Replace("{TOKEN}", theToken),
                Type = _termiiAuthSettings.Type,
                Channel = _termiiAuthSettings.Channel,
                ApiKey = _termiiAuthSettings.ApiKey
            };
            var termiiResponse = await ApiHelper.DoWebRequestAsync<TermiiResponse>(_termiiAuthSettings.Url, termiiRequest, "post");
            return termiiResponse;
        }
    }
}
