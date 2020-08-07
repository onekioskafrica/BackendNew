using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OK_OnBoarding.Data;
using OK_OnBoarding.Installers;
using OK_OnBoarding.Options;

namespace OK_OnBoarding
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("AllowAll", options => {
                    options.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                 });
            });
            services.InstallServicesInAssembly(Configuration);
            services.AddAutoMapper(typeof(Startup));
            services.AddControllersWithViews();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.Use((context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)context.Request.Headers["Origin"] });
                    context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type,contentType, Accept, Authorization" });
                    context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                }
                return next.Invoke();
            });
            app.UseCors(options => options.WithOrigins("*"));
            app.UseStaticFiles();

            app.UseAuthentication();

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => option.RouteTemplate = swaggerOptions.JsonRoute);
            app.UseSwaggerUI(option => option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
