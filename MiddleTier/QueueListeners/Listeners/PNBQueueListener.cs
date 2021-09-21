using API.Mappers;
using AutoMapper;
using Azure.Storage.Blobs;
using Common.Models.Business;
using FunctionApps;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using QueueListeners.Listeners;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QueueListeners
{
    public class PNBQueueListener : QueueListenerBase
    {
        public PNBQueueListener(QueueMapperConfig mapperconfig, RestClient restClient,QueueClientFactory queueClientFactory) : base(mapperconfig, restClient, queueClientFactory)
        {
        }

        [Function("PNBQueueListener")]
        public void Run([QueueTrigger("pocketnotebookqueue")] string myQueueItem, FunctionContext context)
        {
            CreateLogger(context);
            LogInfo($"C# Queue trigger function processed: {myQueueItem}");

            string newQueueItem = Helpers.ParseQueueMessage(myQueueItem);
            var docRoot = JsonDocument.Parse(newQueueItem).RootElement;

            //archive queue message
            ArchiveQueueMessage(myQueueItem);

            LogInfo($"\tmapping to business entities");
            PocketNotebook pocketNotebook = _mapper.Map<PocketNotebook>(docRoot);
            pocketNotebook.Photos = _mapper.Map<List<Photo>>(docRoot.GetProperty("photos").EnumerateArray());


            LogInfo($"\tcalling API");
            RestRequest req = Helpers.GetRestRequest("/api/pnb", Method.POST, docRoot.GetProperty("enteredBy").GetString() );
            req.AddJsonBody(pocketNotebook, "application/json");
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
