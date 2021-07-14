using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Base;
//using System.Web.Mvc.Filters;
using API.Models.IYC;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text.Json.Serialization;
using System.Text.Json;
using API.Models.PNB;
using API.DataverseAccess;
using API.Mappers;
using Common.Models.Business;
using Microsoft.Extensions.Caching.Memory;
using API.Models.Business;
using System.Dynamic;
using Common.Models.Dataverse;

namespace API.Controllers
{

    public class PNBMapperProfile : Profile
    {
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PNBController : ControllerBase
    {
        IMapper mapper;
        DVDataAccess adminDataAccess;
        DVDataAccess userDataAccess;

        public PNBController(MapperConfig mapperconfig, Func<string, DVDataAccess> da)
        {
            mapper = new Mapper(mapperconfig.mapperConfig);
            adminDataAccess = da("Admin");
            userDataAccess = da("User");
        }

        [HttpGet("getwhereowner")]
        public ActionResult<List<PocketNotebookListEntry>> GetListForUser()
        {
            var userId = adminDataAccess.GetUserId(Request.Headers["UserEmail"].ToString());
            

            var pnb = userDataAccess.GetAll<DVPocketNotebook>($"_ownerid_value eq { userId}","cp_notedateandtime");

            return mapper.Map<List<PocketNotebookListEntry>>(pnb);

        }


        private Guid? FindOrCreateIncident(string incidentNumber)
        {
            if (string.IsNullOrEmpty(incidentNumber))
            {
                return null;
            }

            var incident = adminDataAccess.GetEntityByField<DVIncident>("cp_incidentnumber", incidentNumber);
            if (incident == null)
            {
                incident = new DVIncident();
                incident.cp_incidentnumber = incidentNumber;
                var incidentId = userDataAccess.CreateEntity(incident);
                return incidentId;
            }
            else
            {
                return incident.cp_incidentid;
            }

            
        }


        [HttpPost()]
        public ActionResult<Guid> Post([FromBody] PocketNotebook pnb)
        {

            string userEmail = Request.Headers["UserEmail"].ToString();


            var dvPb = mapper.Map<DVPocketNotebook>(pnb);
            
            //find/create the related incident
            Guid? incidentId = FindOrCreateIncident(pnb.IncidentNumber);
            if (incidentId != null)
            {
                dvPb.cp_incidentno = $"/cp_incidents({incidentId})";
            }


            //dvPb.ownerid = dataAccessHelper.GetUserId(Request.Headers["UserEmail"].ToString());
            var pnbGuid = userDataAccess.CreateEntity( dvPb);

            var dvPbImages = mapper.Map<PocketNotebook, DVPocketNotebookImages>(pnb);
            userDataAccess.CreateEntityImage(pnbGuid, dvPbImages, x => x.cp_sketch);
            userDataAccess.CreateEntityImage(pnbGuid, dvPbImages, x => x.cp_signature);

            foreach (var photo in pnb.Photos)
            {
                photo.PocketNotebookId = pnbGuid;
                var dvPhoto = mapper.Map<DVPhoto>(photo);
                var dvPhotoId = userDataAccess.CreateEntity(dvPhoto);

                var dvPhotoImage = mapper.Map<DVPhotoImages>(photo);
                userDataAccess.CreateEntityImage(dvPhotoId, dvPhotoImage, x => x.cp_image);

            }

            return pnbGuid ;
        }
    }
}
