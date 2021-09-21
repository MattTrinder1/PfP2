using Azure.Storage.Blobs;

namespace API.Mappers
{
    public class QueueMapperConfig
    {

        public AutoMapper.MapperConfiguration mapperConfig;

        public QueueMapperConfig(BlobContainerClientFactory containerClientFactory)
        {
            mapperConfig = new AutoMapper.MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile(new AppToBusinessProfile(containerClientFactory));
                    });
        }
    }
}
