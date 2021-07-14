using API.DataverseAccess;
using API.Mappers;
using AutoMapper;
using Azure.Storage.Blobs;
using DataverseRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergeAPI
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmergeAPI", Version = "v1" });
            });

            services.AddSingleton(new BlobContainerClient(Configuration.GetValue<string>("AzureWebJobsStorage"), "pnb"));
            services.AddSingleton<MapperConfig>();
            services.AddSingleton(new ConnectionConfiguration(Configuration.GetValue<string>("Connection:DataverseUrl"),
                                                                 Configuration.GetValue<string>("Connection:ServiceUrl"),
                                                                 Configuration.GetValue<string>("Connection:ClientId"),
                                                                 Configuration.GetValue<string>("Connection:ClientSecret"),
                                                                 Configuration.GetValue<string>("Connection:Authority")));
            services.AddMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<Func<string,DVDataAccess>>(serviceProvider => key => {
                switch (key)
                {
                    case "Admin":
                        return new DVDataAccess(serviceProvider.GetService<ConnectionConfiguration>(), 
                                                serviceProvider.GetService<IMemoryCache>());
                    case "User":
                        return new DVDataAccess(serviceProvider.GetService<ConnectionConfiguration>(),
                                                serviceProvider.GetService<IMemoryCache>(),
                                                new DVDataAccess(
                                                        serviceProvider.GetService<ConnectionConfiguration>(),
                                                        serviceProvider.GetService<IMemoryCache>())
                                                            .GetUserADObjectId(serviceProvider.GetService<IHttpContextAccessor>().HttpContext.Request.Headers["UserEmail"]));
                    default:
                        return null;
                }
            });

            services.AddSingleton<DVDataAccess>();




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmergeAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
