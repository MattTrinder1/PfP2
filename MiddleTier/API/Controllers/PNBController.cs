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
using Microsoft.Extensions.Logging;
using System.Net;

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
        ILogger logger;

        public PNBController(MapperConfig mapperconfig, Func<string, DVDataAccess> da, ILogger<PNBController> log)
        {
            mapper = new Mapper(mapperconfig.mapperConfig);
            adminDataAccess = da("Admin");
            userDataAccess = da("User");
            logger = log;
        }

        [HttpGet("getwhereowner")]
        public ActionResult<List<PocketNotebookListEntry>> GetListForUser()
        {
            try
            {
                logger.LogDebug(Request.Headers["UserEmail"].ToString());

                var userId = adminDataAccess.GetUserId(Request.Headers["UserEmail"].ToString());

                logger.LogDebug(userId.ToString());

                ICollection<DVPocketNotebook> pnb = userDataAccess.GetAll<DVPocketNotebook>($"_ownerid_value eq {userId}", "cp_notedateandtime");

                return mapper.Map<List<PocketNotebookListEntry>>(pnb);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
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

        [HttpGet("{id}")]
        public ActionResult<PocketNotebook> Get(string id)
        {
            try
            {
                logger.LogDebug(Request.Headers["UserEmail"].ToString());

                DVPocketNotebook pnb = userDataAccess.GetEntityByField<DVPocketNotebook>("cp_pocketnotebookid", id);

                DVPocketNotebookImages pnbImages = userDataAccess.GetEntityByField<DVPocketNotebookImages>("cp_pocketnotebookid", id, DVDataAccess.SelectColumns.TypePropertiesWithoutImages);
                userDataAccess.GetImages(pnbImages, true);

                DVIncident incident = null;
                if (pnb.cp_incidentno != null)
                {
                    incident = userDataAccess.GetEntityByField<DVIncident>("cp_incidentid", pnb.cp_incidentno.EntityId.Value.ToString());
                }

                var pnbPhotosCol = userDataAccess.GetAll<DVPhoto>($"_cp_pocketnotebook_value eq {pnb.cp_pocketnotebookid}");
                var pnbPhotoImagesCol = userDataAccess.GetAll<DVPhotoImage>($"_cp_pocketnotebook_value eq {pnb.cp_pocketnotebookid}", DVDataAccess.SelectColumns.TypePropertiesWithoutImages);
                userDataAccess.GetImages(pnbPhotoImagesCol, true);

                PocketNotebook result = mapper.Map<PocketNotebook>(pnb);
                result = mapper.Map(pnbImages, result);
                if (incident != null)
                {
                    result = mapper.Map(incident, result);
                }
                if (pnbPhotosCol != null && pnbPhotosCol.Count > 0)
                {
                    DVPhoto[] pnbPhotos = new DVPhoto[pnbPhotosCol.Count];
                    pnbPhotosCol.CopyTo(pnbPhotos, 0);
                    DVPhotoImage[] pnbPhotoImages = new DVPhotoImage[pnbPhotoImagesCol.Count];
                    pnbPhotoImagesCol.CopyTo(pnbPhotoImages, 0);

                    List<Photo> photos = new List<Photo>(pnbPhotosCol.Count);
                    for (int i = 0; i < pnbPhotos.Length; i++)
                    {
                        Photo photo = mapper.Map<Photo>(pnbPhotos[i]);
                        if (i < pnbPhotoImages.Length)
                        {
                            photo = mapper.Map(pnbPhotoImages[i], photo);
                        }
                        photos.Add(photo);
                    }
                    result.Photos = photos;
                }

                return result;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

        [HttpPost()]
        public ActionResult<Guid> Post([FromBody] PocketNotebook pnb)
        {
            try
            {
                string userEmail = Request.Headers["UserEmail"].ToString();

                var dvPb = mapper.Map<DVPocketNotebook>(pnb);

                Guid? incidentId = FindOrCreateIncident(pnb.IncidentNumber);
                if (incidentId != null)
                {
                    dvPb.cp_incidentno = new EntityReference("cp_incident", incidentId);
                }

                Guid pnbGuid = userDataAccess.CreateEntity(dvPb);

                var dvPbImages = mapper.Map<PocketNotebook, DVPocketNotebookImages>(pnb);
                userDataAccess.CreateEntityImage(pnbGuid, dvPbImages, x => x.cp_sketch);
                userDataAccess.CreateEntityImage(pnbGuid, dvPbImages, x => x.cp_signature);

                foreach (var photo in pnb.Photos)
                {
                    photo.PocketNotebookId = pnbGuid;
                    var dvPhoto = mapper.Map<DVPhoto>(photo);
                    var dvPhotoId = userDataAccess.CreateEntity(dvPhoto);

                    var dvPhotoImage = mapper.Map<DVPhotoImage>(photo);
                    userDataAccess.CreateEntityImage(dvPhotoId, dvPhotoImage, x => x.cp_image);
                }

                return pnbGuid;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError (e.Message));
            }
        }
    }
}