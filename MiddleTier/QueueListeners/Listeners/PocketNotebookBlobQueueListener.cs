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
    public class PocketNotebookBlobQueueListener : QueueListenerBase
    {
        private BlobContainerClient _containerClient;
        public PocketNotebookBlobQueueListener(QueueMapperConfig mapperconfig, RestClient restClient, QueueClientFactory queueClientFactory, BlobContainerClientFactory containerClientFactory) : base(mapperconfig, restClient, queueClientFactory)
        {
            _containerClient = containerClientFactory.GetBlobContainerClient("pnb");
        }

        [Function("PocketNotebookBlobQueueListener")]
        public void Run([QueueTrigger("pocketnotebookblobqueue")] string myQueueItem, FunctionContext context)
        {
            CreateLogger(context);
            LogInfo($"C# Queue trigger function processed: {myQueueItem}");

            string newQueueItem = Helpers.ParseQueueMessage(myQueueItem);

            ArchiveQueueMessage(myQueueItem);



            var queueItemParts = newQueueItem.Split("*&^%$");
            var queueItemParts2 = queueItemParts[0].Split(":");
            var deleted = bool.Parse(queueItemParts[2]);

            var blobId = queueItemParts2[2];

            var blobIdParts = blobId.Split(new char[] { '-' });

            var imageType = blobIdParts[10];

            var photoId = Guid.Parse(blobIdParts[5] + "-" + blobIdParts[6] + "-" + blobIdParts[7] + "-" + blobIdParts[8] + "-" + blobIdParts[9]);

            var userEmail = queueItemParts2[0];
            var pocketNotebookId = queueItemParts2[1];
            var caption = queueItemParts[1];
            //var parentId = queueItemParts[3];

            var blobClient = _containerClient.GetBlobClient(blobId);
            string blob;
            using (var memorystream = new MemoryStream())
            {
                blobClient.DownloadTo(memorystream);

                blob = Convert.ToBase64String(memorystream.ToArray());
            }

            IRestRequest req;
            if (!string.IsNullOrEmpty(caption))
            {
                req = Helpers.GetRestRequest("/api/pnb/{pocketNotebookId}/{photoId}/{imageType}/{deleted}/{caption}", Method.PATCH, userEmail)
                                           .AddUrlSegment("pocketNotebookId", pocketNotebookId)
                                           .AddUrlSegment("photoId", photoId)
                                           .AddUrlSegment("imageType", imageType)
                                           .AddUrlSegment("deleted", deleted)
                                           .AddUrlSegment("caption", caption);
            }
            else
            {
                req = Helpers.GetRestRequest("/api/pnb/{pocketNotebookId}/{photoId}/{imageType}/{deleted}", Method.PATCH, userEmail)
                                           .AddUrlSegment("pocketNotebookId", pocketNotebookId)
                                           .AddUrlSegment("photoId", photoId)
                                           .AddUrlSegment("imageType", imageType)
                                           .AddUrlSegment("deleted", deleted);
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
