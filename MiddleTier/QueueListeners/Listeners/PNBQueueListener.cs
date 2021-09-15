using API.Mappers;
using AutoMapper;
using Azure.Storage.Blobs;
using Common.Models.Business;
using FunctionApps;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QueueListeners
{
    public class PNBQueueListener
    {
        RestClient _restClient;
        Mapper _mapper;
        BlobContainerClient _containerClient;
        public PNBQueueListener(API.Mappers.MapperConfiguration mapperconfig, RestClient restClient, BlobContainerClient containerClient)
        {
            _mapper = new Mapper(mapperconfig.mapperConfig);
            _restClient = restClient;
            _containerClient = containerClient;
        }

        [Function("PNBQueueListener")]
        public void Run([QueueTrigger("pocketnotebookqueue")] string myQueueItem, FunctionContext context)
        {
            var log = context.GetLogger("PNBQueueListener");
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            string newQueueItem = Helpers.ParseQueueMessage(myQueueItem);
            var docRoot = JsonDocument.Parse(newQueueItem).RootElement;

            log.LogDebug($"\t mapping to business entities");
            PocketNotebook pocketNotebook = _mapper.Map<PocketNotebook>(docRoot);
            pocketNotebook.Photos = _mapper.Map<List<Photo>>(docRoot.GetProperty("photos").EnumerateArray());


            log.LogDebug($"\t calling API");
            RestRequest req = Helpers.GetRestRequest("/api/pnb", Method.POST, docRoot.GetProperty("enteredBy").GetString() );
            req.AddJsonBody(pocketNotebook, "application/json");
            var resp = _restClient.Execute(req);

            if (!resp.IsSuccessful)
            {
                string errorMessage = "Error calling API" + 
                    (resp.ErrorMessage != null ? $": {resp.ErrorMessage}" : "");
                throw new Exception(errorMessage, resp.ErrorException);
            }

            log.LogInformation(resp.Content);
        }




        
    }
}
