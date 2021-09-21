using API.Mappers;
using Azure.Storage.Blobs;
using Common.Models.Business;
using Common.Models.Queue;
using FunctionApps;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using QueueListeners.Listeners;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QueueListeners
{
    public class SuddenDeathQueueListener : QueueListenerBase
    {

        public SuddenDeathQueueListener(QueueMapperConfig mapperconfig, RestClient restClient, QueueClientFactory queueClientFactory) : base(mapperconfig, restClient, queueClientFactory)
        {
        }

        [Function("SuddenDeathQueueListener")]
        public void Run([QueueTrigger("suddendeathqueue")] string myQueueItem, FunctionContext context)
        {
            CreateLogger(context);
            LogInfo($"C# Queue trigger function processed: {myQueueItem}");

            string newQueueItem = Helpers.ParseQueueMessage(myQueueItem);

            //archive queue message
            ArchiveQueueMessage(myQueueItem);

            var docRoot = JsonDocument.Parse(newQueueItem).RootElement;
            var queueSD = JsonSerializer.Deserialize<QueueSuddenDeath>(docRoot.ToString());

            SuddenDeath suddenDeath = _mapper.Map<SuddenDeath>(queueSD);
            //suddenDeath.Photos = _mapper.Map<List<Photo>>(docRoot.GetProperty("photos").EnumerateArray());


            RestRequest req = Helpers.GetRestRequest("/api/suddendeath", Method.POST, suddenDeath.EnteredBy );
            req.AddJsonBody(suddenDeath, "application/json");
            var resp = _restClient.Execute(req);

            if (!resp.IsSuccessful)
            {
                string errorMessage = "Error calling API" + 
                    (resp.ErrorMessage != null ? $": {resp.ErrorMessage}" : "");
                throw new Exception(errorMessage, resp.ErrorException);
            }

            LogInfo(resp.Content);

            

        }




        
    }
}
