using Azure.Storage.Blobs;

namespace API.Mappers
{
    public class MapperConfiguration
    {

        public AutoMapper.MapperConfiguration mapperConfig;

        public MapperConfiguration(BlobContainerClient containerClient)
        {
            mapperConfig = new AutoMapper.MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile(new DataverseToBusinessProfile());
                        cfg.AddProfile(new BusinessToDataverseProfile());
                        cfg.AddProfile(new AppToBusinessProfile(containerClient));
                    });
        }
    }
}
