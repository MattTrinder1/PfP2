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
    public class SuddenDeathQueueListener
    {
        RestClient _restClient;
        Mapper _mapper;
        BlobContainerClient _containerClient;
        public SuddenDeathQueueListener(API.Mappers.MapperConfiguration mapperconfig, RestClient restClient, BlobContainerClient containerClient)
        {
            _mapper = new Mapper(mapperconfig.mapperConfig);
            _restClient = restClient;
            _containerClient = containerClient;
        }

        [Function("SuddenDeathQueueListener")]
        public void Run([QueueTrigger("suddendeathqueue")] string myQueueItem, FunctionContext context)
        {
            var log = context.GetLogger("SuddenDeathQueueListener");
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            string newQueueItem = Helpers.ParseQueueMessage(myQueueItem);
            var docRoot = JsonDocument.Parse(newQueueItem).RootElement;

            SuddenDeath suddenDeath = _mapper.Map<SuddenDeath>(docRoot);
            //suddenDeath.Photos = _mapper.Map<List<Photo>>(docRoot.GetProperty("photos").EnumerateArray());


            RestRequest req = Helpers.GetRestRequest("/api/suddendeath", Method.POST, docRoot.GetProperty("enteredBy").GetString() );
            req.AddJsonBody(suddenDeath, "application/json");
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
