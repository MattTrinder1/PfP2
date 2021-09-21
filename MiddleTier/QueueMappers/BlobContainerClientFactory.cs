using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Mappers
{
    public class BlobContainerClientFactory
    {
        private string _connectionString;
        public BlobContainerClientFactory(string storageConnectionString)
        {
            _connectionString = storageConnectionString;
        }

        public BlobContainerClient GetBlobContainerClient(string containerName)
        {
            return new BlobContainerClient(_connectionString, containerName);
        }

    }
}
