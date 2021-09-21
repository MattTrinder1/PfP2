using API.DataverseAccess;
using Azure.Storage.Blobs;

namespace API.Mappers
{
    public class MapperConfig
    {

        public AutoMapper.MapperConfiguration mapperConfig;

        public MapperConfig(BlobContainerClient containerClient,DVDataAccessFactory daFactory)
        {
            mapperConfig = new AutoMapper.MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile(new DataverseToBusinessProfile());
                        cfg.AddProfile(new BusinessToDataverseProfile(daFactory.GetAdminDataAccess()));
                    });
        }
    }
}
