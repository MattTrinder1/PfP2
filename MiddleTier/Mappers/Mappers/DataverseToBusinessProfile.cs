using AutoMapper;
using Common.Models.Business;
using Common.Models.Dataverse;
using System;

namespace API.Mappers
{
    public class DataverseToBusinessProfile : Profile
    {
        public DataverseToBusinessProfile()
        {
            RecognizePrefixes("cp_");

            CreateMap<DVPocketNotebook, PocketNotebook>()
                .ForMember(dest => dest.IncidentNumber, map => map.MapFrom(src => src.cp_incidentno.Name))
            ;

            CreateMap<DVPocketNotebookImages, PocketNotebook>()
                .ForMember(dest => dest.Sketch, map => map.MapFrom(src => Convert.ToBase64String(src.cp_sketch)))
                .ForMember(dest => dest.Signature, map => map.MapFrom(src => Convert.ToBase64String(src.cp_signature)))
            ;

            CreateMap<DVIncident, PocketNotebook>()
            ;

            CreateMap<DVPhoto, Photo>()
                .ForMember(dest => dest.Caption, map => map.MapFrom(src => src.cp_phototitle))
                .ForMember(dest => dest.PocketNotebookId, map => map.MapFrom(src => src.cp_pocketnotebook.Id))
                .ForMember(dest => dest.Blob, map => map.MapFrom(src => Convert.ToBase64String(src.cp_image)))
            ;

            CreateMap<DVPocketNotebook, PocketNotebookListEntry>()
                .ForMember(dest => dest.Sketch, map => map.MapFrom(src => Convert.ToBase64String(src.cp_sketch)))
                .ForMember(dest => dest.Signature, map => map.MapFrom(src => Convert.ToBase64String(src.cp_signature)))
                .ForMember(dest => dest.IncidentNumber, map => map.MapFrom(src => src.cp_incidentno.Name))
            ;

            CreateMap<DVLookupField, LookupField>()
            ;

            CreateMap<DVLookupValue, LookupValue>()
                .ForMember(dest => dest.DisplayValue, map => map.MapFrom(src => src.cp_name))
            ;

            CreateMap<DVTerritorialPolicingArea, LookupValue>()
                .ForMember(dest => dest.DisplayValue, map => map.MapFrom(src => src.cp_name))
            ;

            CreateMap<DVContactRoleType, LookupValue>()
                .ForMember(dest => dest.DisplayValue, map => map.MapFrom(src => src.cp_name))
            ;

            CreateMap<DVUser, User>()
                .ForMember(dest => dest.Name, map => map.MapFrom(src => src.fullname))
                .ForMember(dest => dest.DisplayValue, map => map.MapFrom(src => $"{src.cp_badgenumber} - {src.fullname}"))
                .ForMember(dest => dest.EmailAddress, map => map.MapFrom(src => src.internalemailaddress))
            ;

            //CreateMap<DVVehicleTicket, VehicleTicket>()
            //;

            //CreateMap<ExpandoObject, EntityBase>()
            //    .ForMember(dest => dest.OwnerId, map => map.MapFrom(src => MapEntityBaseFields<User>(src, "ownerid")))
            //;

            //CreateMap<ExpandoObject, VehicleTicket>()
            //    .IncludeBase<ExpandoObject, EntityBase>()
            //    .ForMember(dest => dest.PoliceVehicleNumber, map => map.MapFrom(src => src.Single(x => x.Key == "cp_policevehiclenumber").Value))
            //    .ForMember(dest => dest.OffenceDateTime, map => map.MapFrom(src => DateTime.Parse(src.Single(x => x.Key == "cp_offencedatetime").Value.ToString())))
            //    .ForMember(dest => dest.IssuedTo, map => map.MapFrom(src => MapEntityBaseFields<LookupValue>(src, "cp_issuedto")));
            //;
        }

        //public T MapEntityBaseFields<T>(ExpandoObject src, string fieldName) where T : EntityBase,new()
        //{
        //    var ret = new T();
        //    ret.Id = Guid.Parse(src.Single(x => x.Key == $"_{fieldName}_value").Value.ToString());
        //    ret.Name = src.Single(x => x.Key == $"_{fieldName}_value@OData.Community.Display.V1.FormattedValue").Value.ToString();

        //    return ret;
        //}

    }
}
