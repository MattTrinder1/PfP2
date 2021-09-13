using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionApps
{
    static class Helpers
    {
        internal static RestRequest GetRestRequest(string url, Method method, string userEmailAddress, string integrationKey = null)
        {
            var req = new RestRequest(url, method);
            req.RequestFormat = DataFormat.None;

            if (method == Method.POST || method == Method.PATCH) //adding or updating
            {
                req.RequestFormat = DataFormat.Json;
                req.AddHeader("Prefer", "return=representation");
                req.AddHeader("UserEmail", userEmailAddress);
                if (string.IsNullOrEmpty(integrationKey))
                {
                    integrationKey = Environment.GetEnvironmentVariable("APIIntegrationKey");
                }
                req.AddHeader("IntegrationKey", integrationKey);
            }

            return req;
        }

        internal static string ParseQueueMessage(string myQueueItem)
        {
            var parts = myQueueItem.Split("\\\\\\\"").ToList();
            var newParts = new List<string>();
            foreach (var part in parts)
            {
                string temp = part.Replace("\\n", Environment.NewLine).Replace("\\", "").Replace("\r", "\\r").Replace("\n", "\\n");
                newParts.Add(temp);
            }
            var newQueueItem = string.Join("\\\"", newParts);
            newQueueItem = newQueueItem.Trim(new Char[] { '\"' });
            return newQueueItem;
        }

    }
}
