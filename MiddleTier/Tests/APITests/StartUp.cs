using API;
using API.DataverseAccess;
using DataverseRepository;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APITests
{
    [TestClass]
    static class StartUp
    {

        private static TestServer testServer;
        public static IOrganizationService adminService;

        [AssemblyInitialize]
        public static void Initialise(TestContext testContext)
        {

            var connConfig = new ConnectionConfiguration("https://policeproductuat.crm11.dynamics.com/appportal/sg/notification.aspx", "83af4fce-10c3-4409-a43d-d67e300424aa", "H.ARok4~mmJl_lfU~nfE9JqE.7pUE26u.t");
            var daFactory = new DVDataAccessFactory(connConfig, new MemoryCache(new MemoryCacheOptions()), "matt.trinder@tisski.com");
            adminService = daFactory.AdminDvService;

            testServer = new TestServer(
                WebHost.CreateDefaultBuilder()
                .UseContentRoot(Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "..", "..", "API")))
                .UseEnvironment("Development")
                .UseStartup<Startup>()
                );

            
        }

        public static HttpClient GetClient()
        {
            var client = testServer.CreateClient();
            client.DefaultRequestHeaders.Add("UserEmail", "cumbriatestuser3@tisski.com");
            return client;
        }
    }
}
