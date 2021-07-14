using API;
using API.DataverseAccess;
using EmergeAPI;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public static DynamicDataAccess da;

        [AssemblyInitialize]
        public static void Initialise(TestContext testContext)
        {
            testServer = new TestServer(
                WebHost.CreateDefaultBuilder()
                .UseContentRoot(Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "..", "..", "API")))
                .UseEnvironment("Development")
                .UseStartup<Startup>()
                );

            da = new DynamicDataAccess("https://cumbriapoc.api.crm4.dynamics.com/api/data/v9.2/",
                          "https://cumbriapoc.crm4.dynamics.com",
                          "9b0a7eea-a4e3-4d8c-b941-5c0fb9046102",
                          "5C_O93sL6ESBH.TYr6Q.ZpbWf-IM2o5BSR",
                          "https://login.microsoftonline.com/66da2a16-0c51-49c1-ab7e-c5fe8eb76946");



        }

        public static HttpClient GetClient()
        {
            var client = testServer.CreateClient();
            client.DefaultRequestHeaders.Add("UserEmail", "matt.trinder@tisski.com");
            return client;
        }
    }
}
