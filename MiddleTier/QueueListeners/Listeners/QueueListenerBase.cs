using API.Mappers;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueListeners.Listeners
{
    public abstract class QueueListenerBase
    {
        protected RestClient _restClient;
        protected Mapper _mapper;
        protected QueueClient _archiveQueueClient;
        private ILogger _logger;
        
        public QueueListenerBase(QueueMapperConfig mapperconfig, RestClient restClient,QueueClientFactory queueClientFactory)
        {
            _mapper = new Mapper(mapperconfig.mapperConfig);
            _restClient = restClient;

            _archiveQueueClient = queueClientFactory.GetQueueClient(this.GetType().Name.Remove(this.GetType().Name.Length-8).ToLower() + "-archive");

        }

        protected void LogInfo(string message, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogInformation(message, args);
            }
        }
        protected void LogDebug(string message, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogDebug(message, args);
            }
        }

        protected void CreateLogger(FunctionContext context)
        {
            if (context != null)
            {
                _logger = context.GetLogger("SuddenDeathQueueListener");
            }
        }

        protected void ArchiveQueueMessage(string message)
        {
            _archiveQueueClient.SendMessage(message);
        }
    }
}
