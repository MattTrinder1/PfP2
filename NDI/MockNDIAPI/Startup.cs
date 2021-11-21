using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Models;
using SoapCore;
using SoapCore.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MockNDIAPI
{
    public class MyServiceOperationTuner : IServiceOperationTuner
    {
        public void Tune(HttpContext httpContext, object serviceInstance, SoapCore.ServiceModel.OperationDescription operation)
        {
            if (operation.Name.Equals("HostConnect2"))
            {
                var service = serviceInstance as consolidataws;
                string result = string.Empty;

                StringValues paramValue;
                if (httpContext.Request.Headers.TryGetValue("some_parameter", out paramValue))
                {
                    result = paramValue[0];
                }

                service.SetParameterForSomeOperation(result);

                httpContext.Response.Headers.Add("content-type", "application/soap+xml");
            }
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MockNDIAPI", Version = "v1" });
            });
            services.AddSingleton<Iconsolidataws, consolidataws>();
            services.AddSoapServiceOperationTuner(new MyServiceOperationTuner());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MockNDIAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.Use(async (context, nextMiddleware) =>
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers["content-type"]= "application/soap+xml;charset=utf-8";
                    return Task.FromResult(0);
                });
                await nextMiddleware();
            });

            app.UseSoapEndpoint<Iconsolidataws>("/consolidataws.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer,false,null,null,false,false);


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
