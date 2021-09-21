using Azure.Storage.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Mappers
{
    public class QueueClientFactory
    {
        private string _connectionString;
        public QueueClientFactory(string storageConnectionString)
        {
            _connectionString = storageConnectionString;
        }

        public QueueClient GetQueueClient(string queueName)
        {
            return new QueueClient(_connectionString, queueName,new QueueClientOptions() { MessageEncoding = QueueMessageEncoding.Base64 });
        }

    }
}
