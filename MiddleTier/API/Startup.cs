using API.DataverseAccess;
using API.Mappers;
using Azure.Storage.Blobs;
using DataverseRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace API
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

            services.AddSingleton(Configuration);

            services.AddSingleton(new BlobContainerClient(Configuration.GetValue<string>("AzureWebJobsStorage"), "pnb"));

            services.AddSingleton(new ConnectionConfiguration(
                Configuration.GetValue<string>("Connection:DataverseUrl"),
                Configuration.GetValue<string>("Connection:ClientId"),
                Configuration.GetValue<string>("Connection:ClientSecret")));



            services.AddMemoryCache();
            services.AddSingleton<CacheOrchestrator>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<DVDataAccessFactory>();
            services.AddSingleton<MapperConfig>();
            services.AddSingleton<ApiConfiguration>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PoliceAPI", Version = "v1" });
                c.OperationFilter<CustomHeaderSwaggerAttributes>();
            });

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

    public class CustomHeaderSwaggerAttributes : IOperationFilter
    {
        private ApiConfiguration configuration = null;
        public CustomHeaderSwaggerAttributes(ApiConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            if (!configuration.SuppressIntegrationKeyVerification)
            {
                //Add IntegrationKey header parameter to swagger definition for all operations.
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "IntegrationKey",
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
            }

            if (context.MethodInfo.DeclaringType != typeof(API.Controllers.CacheController))
            {
                //Add UserEmail header parameter to swagger definition for all user level operations.
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
    }
}