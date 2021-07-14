using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApps
{
    static class Helpers
    {
        internal static RestRequest GetRestRequest(string url, Method method,string userEmailAddress)
        {
            var req = new RestRequest(url, method);
            req.RequestFormat = DataFormat.None;

            if (method == Method.POST || method == Method.PATCH) //adding or updating
            {
                req.RequestFormat = DataFormat.Json;
                req.AddHeader("Prefer", "return=representation");
                req.AddHeader("UserEmail", userEmailAddress);
                
            }

            return req;
        }

    }
}
