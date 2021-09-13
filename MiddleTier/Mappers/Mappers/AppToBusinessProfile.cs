using API.Models.Business;
using API.Models.Dataverse;
using API.Models.IYC;
using API.Models.PNB;
using AutoMapper;
using Azure.Storage.Blobs;
using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Mappers
{
    public class AppToBusinessProfile : Profile
    {
        private BlobContainerClient _containerClient;

        public AppToBusinessProfile(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
            //SourceMemberNamingConvention = new ();
            RecognizePrefixes("cp_");


            //CreateMap<AppPocketNotebook, PocketNotebook>()
            // .ForMember(dest => dest.Sketch, map => map.MapFrom(src => GetBlob(src.SketchId)))
            // .ForMember(dest => dest.Signature, map => map.MapFrom(src => GetBlob(src.SignatureId)))

            ;
            


            CreateMap<JsonElement, Photo>()
                .ForMember(dest => dest.Caption, map => map.MapFrom(src => src.GetProperty("caption").GetString()))
                .ForMember(dest => dest.Blob, map => map.MapFrom(src => GetBlob(src.GetProperty("blobId").GetString())))
                ;

            CreateMap<JsonElement, SuddenDeath>()
                //string fields
                .ForMember(dest => dest.WhereLastSeenAlive, map => map.MapFrom(src => src.GetProperty("WhereLastSeenAlive").GetString()))
                .ForMember(dest => dest.IncidentNumber, map => map.MapFrom(src => src.GetProperty("Incident Number").GetString()))
                //guid fields
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.GetProperty("Id").GetGuid()))
                //date fields
                .ForMember(dest => dest.IncidentDate, map => map.MapFrom(src => src.GetProperty("Inicident Date").GetDateTime()))
                ;



            CreateMap<JsonElement, PocketNotebook>()
                //string fields
                .ForMember(dest => dest.Notes, map => map.MapFrom(src => src.GetProperty("Notes").GetString()))
                .ForMember(dest => dest.IncidentNumber, map => map.MapFrom(src => src.GetProperty("IncidentNumber").GetString()))
                .ForMember(dest => dest.SignatoryName, map => map.MapFrom(src => src.GetProperty("SignatoryName").GetString()))
                //guid fields
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.GetProperty("Id").GetGuid()))
                //date fields
                .ForMember(dest => dest.NoteDateAndTime, map => map.MapFrom(src => src.GetProperty("NoteDateAndTime").GetDateTime()))
                .ForMember(dest => dest.IncidentDate, map => map.MapFrom(src => src.GetProperty("IncidentDateTime").ValueKind == JsonValueKind.Null ? (DateTime?)null : src.GetProperty("IncidentDateTime").GetDateTime()))
                .ForMember(dest => dest.SignatureDateandTime, map => map.MapFrom(src => src.GetProperty("Signature Date and Time").ValueKind == JsonValueKind.Null ? (DateTime?)null : src.GetProperty("Signature Date and Time").GetDateTime()))
                //blob fields
                .ForMember(dest => dest.Signature, map => map.MapFrom(src => GetBlob(src.GetProperty("SignatureId").GetString())))
                .ForMember(dest => dest.Sketch, map => map.MapFrom(src => GetBlob(src.GetProperty("SketchId").GetString())))
//photos
//.ForMember(dest => dest.Photos, map => map.MapFrom(src => (JsonElement)src.Single(x => x.Key == "photos").Value));

;


           // CreateMap<AppPhoto, Photo>()
               // .ForMember(dest => dest.Blob, map => map.MapFrom(src => GetBlob(src.blobId)))
          //  ;

        }


        private string GetBlob(string blobId)
        {
            if (string.IsNullOrEmpty(blobId))
            {
                return null;
            }

            var blobClient = _containerClient.GetBlobClient(blobId);

            using (var memorystream = new MemoryStream())
            {
                blobClient.DownloadTo(memorystream);

                return Convert.ToBase64String(memorystream.ToArray());
            }
        }



    }
}
