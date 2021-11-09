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
using System.IO;
using System.Linq;
using System.Text.Json;

namespace QueueListeners
{
    public class SuddenDeathBlobQueueListener : QueueListenerBase
    {
        private BlobContainerClient _containerClient;
        public SuddenDeathBlobQueueListener(QueueMapperConfig mapperconfig, RestClient restClient, QueueClientFactory queueClientFactory, BlobContainerClientFactory containerClientFactory) : base(mapperconfig, restClient, queueClientFactory)
        {
            _containerClient = containerClientFactory.GetBlobContainerClient("suddendeath");
        }

        [Function("SuddenDeathBlobQueueListener")]
        public void Run([QueueTrigger("suddendeathblobqueue")] string myQueueItem, FunctionContext context)
        {
            CreateLogger(context);
            LogInfo($"C# Queue trigger function processed: {myQueueItem}");

            string newQueueItem = Helpers.ParseQueueMessage(myQueueItem);

            ArchiveQueueMessage(myQueueItem);


            var queueItemParts = newQueueItem.Split(new char[] { ':' });

            var blobId = queueItemParts[2];
            var imageType = blobId.Split(new char[] { '-' })[5];
            var userEmail = queueItemParts[0];
            var suddenDeathId = queueItemParts[1];
            var parentId = queueItemParts[3];

            var blobClient = _containerClient.GetBlobClient(blobId);
            string blob;
            using (var memorystream = new MemoryStream())
            {
                blobClient.DownloadTo(memorystream);

                blob = Convert.ToBase64String(memorystream.ToArray());
            }

            IRestRequest req;
            if (imageType == "property" || imageType == "propertysignature")
            {
                req = Helpers.GetRestRequest("/api/suddendeath/{suddenDeathId}/{propertyId}/{imageType}", Method.PATCH, userEmail)
                                            .AddUrlSegment("suddenDeathId", suddenDeathId)
                                            .AddUrlSegment("propertyId", parentId)
                                            .AddUrlSegment("imageType", imageType);
            }
            else
            {
                req = Helpers.GetRestRequest("/api/suddendeath/{suddenDeathId}/{imageType}", Method.PATCH, userEmail)
                                            .AddUrlSegment("suddenDeathId", suddenDeathId)
                                            .AddUrlSegment("imageType", imageType);
            }


            req.AddJsonBody(blob, "application/json");

            var resp = _restClient.Execute(req);

            if (!resp.IsSuccessful)
            {
                throw new Exception(resp.StatusCode.ToString());
            }
            


            //LogInfo(resp.Content);



        }





    }
}
