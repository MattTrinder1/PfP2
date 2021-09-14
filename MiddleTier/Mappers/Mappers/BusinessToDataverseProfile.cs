using API.Models.PNB;
using AutoMapper;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;

namespace API.Mappers
{
    public class BusinessToDataverseProfile : Profile
    {
        public BusinessToDataverseProfile()
        {
            RecognizeDestinationPrefixes("cp_");

            CreateMap<PocketNotebook, DVPocketNotebookImages>()
                .ForMember(dest => dest.cp_sketch, map => map.MapFrom(src => src.Sketch))
                .ForMember(dest => dest.cp_signature, map => map.MapFrom(src => src.Signature))
            ;


            CreateMap<PocketNotebook, DVPocketNotebook>()
                .ForMember(dest => dest.cp_pocketnotebookid, map => map.MapFrom(src => src.Id))
            ;

            CreateMap<SuddenDeath, DVSuddenDeath>()
                .ForMember(dest => dest.cp_suddendeathid, map => map.MapFrom(src => src.Id))
                .ForMember(dest => dest.cp_sdfamilylaisonofficer, map => map.MapFrom(src => src.FamilyLiaisonOfficer))
                .ForMember(dest => dest.cp_sdlastseenaliveby, map => map.MapFrom(src => src.LastSeenAliveBy))
            ;

            CreateMap<Photo, DVPhoto>()
                .ForMember(dest => dest.cp_phototitle, map => map.MapFrom(src => src.Caption))
                .ForMember(dest => dest.cp_pocketnotebook, map => map.MapFrom(src => new EntityReference("cp_pocketnotebook", src.PocketNotebookId.Value)))
                .ForMember(dest => dest.cp_image, map => map.MapFrom(src => src.Blob))
            ;
        }

    }
}
