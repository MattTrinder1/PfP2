﻿using API.Models.Business;
using API.Models.Dataverse;
using API.Models.IYC;
using API.Models.PNB;
using AutoMapper;
using Common.Models.Business;
using Common.Models.Dataverse;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

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

            CreateMap<Note, DVNote>()
                .ForMember(dest => dest.documentbody, map => map.MapFrom(src => src.Attachment))
            ;

            CreateMap<PocketNotebook, DVPocketNotebook>()
                .ForMember(dest => dest.cp_pocketnotebookid, map => map.MapFrom(src => src.Id))
            ;

            CreateMap<Photo, DVPhoto>()
                .ForMember(dest => dest.cp_phototitle, map => map.MapFrom(src => src.Caption))
                .ForMember(dest => dest.cp_pocketnotebook, map => map.MapFrom(src => new EntityReference("cp_pocketnotebook", src.PocketNotebookId.Value)))
            ;

            CreateMap<Photo, DVPhotoImage>()
                .ForMember(dest => dest.cp_image, map => map.MapFrom(src => src.Blob))                
            ;
        }

    }
}
