using AutoMapper;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Mappers
{
    public class MapperConfig
    {

        public MapperConfiguration mapperConfig;

        public MapperConfig(BlobContainerClient containerClient)
        {
            mapperConfig = new MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile(new DataverseToBusinessProfile());
                        cfg.AddProfile(new BusinessToDataverseProfile());
                        cfg.AddProfile(new AppToBusinessProfile(containerClient));
                    });
        }
    }
}
