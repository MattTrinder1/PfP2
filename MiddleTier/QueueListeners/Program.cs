using API.Mappers;
using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace QueueListeners
{
    public class Program
    {
        public static void Main()
        {

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                //.ConfigureLogging()
                .ConfigureServices(
                                   services => services
                                        .AddSingleton(new RestClient(Environment.GetEnvironmentVariable("APIUrl")))
                                        .AddSingleton(new BlobContainerClient(Environment.GetEnvironmentVariable("BlobStorageConnectionString"), "pnb"))
                                        .AddSingleton<MapperConfig>()
                                   )
                .Build();
            host.Run();
        }

        
    }
}