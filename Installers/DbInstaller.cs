using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OK_OnBoarding.Data;
using OK_OnBoarding.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<ICustomersService, CustomersService>();
            services.AddScoped<IStoreOwnerService, StoreOwnerService>();
            services.AddScoped<IDelivermanService, DeliverymanService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<ISuperAdminService, SuperAdminService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAwsS3UploadService, AwsS3UploadService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IProductService, ProductService>();
        }
    }
}
