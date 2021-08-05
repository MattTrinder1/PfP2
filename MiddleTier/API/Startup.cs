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
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergeAPI
{
    public class CustomHeaderSwaggerAttribute : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            //Add UserEmail header parameter to swagger definition for all operations.
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "UserEmail",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PoliceAPI", Version = "v1" });
                c.OperationFilter<CustomHeaderSwaggerAttribute>();
            });

            services.AddSingleton(new BlobContainerClient(Configuration.GetValue<string>("AzureWebJobsStorage"), "pnb"));
            services.AddSingleton<MapperConfig>();

            ConnectionConfiguration connectionConfiguration = new ConnectionConfiguration(Configuration.GetValue<string>("Connection:DataverseUrl"),
                                                                 Configuration.GetValue<string>("Connection:ServiceUrl"),
                                                                 Configuration.GetValue<string>("Connection:ClientId"),
                                                                 Configuration.GetValue<string>("Connection:ClientSecret"),
                                                                 Configuration.GetValue<string>("Connection:Authority"));
            services.AddSingleton(connectionConfiguration);
            services.AddMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<DVDataAccessFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger(c=>c.SerializeAsV2 = true);
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PoliceAPI v1")) ;

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
