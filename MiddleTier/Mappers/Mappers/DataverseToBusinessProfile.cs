using API.Models.Business;
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
    public class DataverseToBusinessProfile : Profile
    {
        public DataverseToBusinessProfile()
        {

            //SourceMemberNamingConvention = new ();
            //RecognizePrefixes("em_");
            RecognizePrefixes("cp_");



            CreateMap<DVLookupValue, LookupValue>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.cp_lookupvalueid));

            CreateMap<DVPocketNotebook, PocketNotebookListEntry>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.cp_pocketnotebookid))
                
                ;

            CreateMap<DVVehicleTicket, VehicleTicket>()
                ;

            CreateMap<ExpandoObject, EntityBase>()
                //.ForMember(dest => dest.Id, map => map.MapFrom(src => src.Single(x => x.Key == "cp_policevehiclenumber").Value))
                //.ForMember(dest => dest.OffenceDateTime, map => map.MapFrom(src => DateTime.Parse(src.Single(x => x.Key == "cp_offencedatetime").Value.ToString())))
                .ForMember(dest => dest.OwnerId, map => map.MapFrom(src => MapEntityBaseFields<User>(src, "ownerid")))
                ;


            CreateMap<ExpandoObject, VehicleTicket>()
                .IncludeBase<ExpandoObject, EntityBase>()
                .ForMember(dest => dest.PoliceVehicleNumber, map => map.MapFrom(src => src.Single(x => x.Key == "cp_policevehiclenumber").Value))
                .ForMember(dest => dest.OffenceDateTime, map => map.MapFrom(src => DateTime.Parse(src.Single(x => x.Key == "cp_offencedatetime").Value.ToString())))
                .ForMember(dest => dest.IssuedTo, map => map.MapFrom(src => MapEntityBaseFields<LookupValue>(src, "cp_issuedto")));
                 ;

            //.ForMember(dest => dest.cp_notedateandtime, map => map.MapFrom(src => src.NoteDateAndTime));

        }

        public T MapEntityBaseFields<T>(ExpandoObject src, string fieldName) where T : EntityBase,new()
        {
            var ret = new T();
            ret.Id = Guid.Parse(src.Single(x => x.Key == $"_{fieldName}_value").Value.ToString());
            ret.Name = src.Single(x => x.Key == $"_{fieldName}_value@OData.Community.Display.V1.FormattedValue").Value.ToString();

            return ret;
        }

    }
}
