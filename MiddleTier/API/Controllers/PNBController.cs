using API.DataverseAccess;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PNBController : ControllerBase
    {
        public PNBController(
            ApiConfiguration configuration,
            DVDataAccessFactory dataAccessFactory,
            CacheOrchestrator cache,
            ILogger<PNBController> log) : base(configuration, dataAccessFactory, cache, log)
        {
        }

        [HttpGet("getwhereowner")]
        public ActionResult<List<PocketNotebookListEntry>> GetListForUser()
        {
            try
            {
                if (!VerifyIntegrationKey("PNB:GET:getwhereowner")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                var userId = AdminDataAccess.GetUserId(Request.Headers["UserEmail"].ToString());

                logger.LogDebug(userId.ToString());

                ICollection<DVPocketNotebook> pnb = UserDataAccess.GetAll<DVPocketNotebook>("ownerid", userId, "cp_notedateandtime",SelectColumns.AllTypeProperties);

                return mapper.Map<List<PocketNotebookListEntry>>(pnb);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }



        [HttpGet("{id}")]
        public ActionResult<PocketNotebook> Get(string id)
        {
            try
            {
                if (!VerifyIntegrationKey("PNB:GET")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                DVPocketNotebook pnb = UserDataAccess.GetEntityByField<DVPocketNotebook>("cp_pocketnotebookid", id);

                DVPocketNotebookImages pnbImages = UserDataAccess.GetEntityByField<DVPocketNotebookImages>("cp_pocketnotebookid", id, SelectColumns.TypePropertiesWithoutImages);
                UserDataAccess.GetImages(pnbImages, true);

                DVIncident incident = null;
                if (pnb.cp_incidentno != null)
                {
                    incident = UserDataAccess.GetEntityByField<DVIncident>("cp_incidentid", pnb.cp_incidentno.Id.ToString());
                }

                var pnbPhotosCol = UserDataAccess.GetAll<DVPhoto>("cp_pocketnotebook", pnb.cp_pocketnotebookid, SelectColumns.TypePropertiesWithoutImages);
                UserDataAccess.GetImages(pnbPhotosCol, true);

                PocketNotebook result = mapper.Map<PocketNotebook>(pnb);
                result = mapper.Map(pnbImages, result);
                if (incident != null)
                {
                    result = mapper.Map(incident, result);
                }
                if (pnbPhotosCol != null && pnbPhotosCol.Count > 0)
                {
                    List<Photo> photos = new List<Photo>(pnbPhotosCol.Count);
                    foreach (var pnbPhoto in pnbPhotosCol)
                    {
                        Photo photo = mapper.Map<Photo>(pnbPhoto);
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

        [HttpPatch("{pocketNotebookId}/{photoId}/{imageType}/{deleted}/{caption}")]
        [HttpPatch("{pocketNotebookId}/{photoId}/{imageType}/{deleted}")]
        public ActionResult UpdateImage(Guid pocketNotebookId, Guid photoId, string imageType,string caption,bool deleted, [FromBody] string newImage)
        {

            try
            {

                var trans = new DVTransaction();

                if (deleted)
                {
                    trans.AddDeleteRequest(new EntityReference("cp_photo", photoId));
                }
                else
                {

                    if (imageType == "sketch")
                    {
                        var sd = new DVPocketNotebookImages();
                        sd.Id = pocketNotebookId;
                        sd.cp_sketch = Convert.FromBase64String(newImage);

                        trans.AddCreateEntityImage(sd, "cp_sketch");

                    }

                    if (imageType == "signature")
                    {
                        var sd = new DVPocketNotebookImages();
                        sd.Id = pocketNotebookId;
                        sd.cp_signature = Convert.FromBase64String(newImage);

                        trans.AddCreateEntityImage(sd, "cp_signature");

                    }

                    if (imageType == "photo")
                    {

                        var photo = new Photo();
                        photo.Id = photoId;
                        photo.PocketNotebookId = pocketNotebookId;
                        photo.Caption = caption;
                        photo.Blob = newImage;

                        var existingPhoto = AdminDataAccess.GetEntityByField<DVPhoto>("cp_photoid", photoId);
                        if (existingPhoto != null)
                        {
                            var dvPhoto = GetDataverseEntity<DVPhoto>(photo, UserId, existingPhoto.Id);
                            trans.AddUpsertEntity(dvPhoto);
                        }
                        else
                        {
                            var dvPhoto = GetDataverseEntity<DVPhoto>(photo, UserId);
                            trans.AddUpsertEntity(dvPhoto);
                        }


                    }
                }

                AdminDataAccess.ExecuteTransaction(trans);


                return new OkResult();
            }
            catch (Exception ex)
            {

                return new NotFoundResult();
            }
        }


        [HttpPost()]
        public ActionResult<Guid> Post([FromBody] PocketNotebook pnb)
        {

            logger.LogInformation("Post");

            try
            {
                logger.LogDebug("Authorising");
                if (!VerifyIntegrationKey("PNB:POST")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);


                logger.LogDebug("Mapping to DV entity");
                var dvPb = GetDataverseEntity<DVPocketNotebook>(pnb, UserId);


                DVTransaction transaction = new DVTransaction();

                logger.LogDebug("Find/Create incident");
                if (!string.IsNullOrEmpty(pnb.IncidentNumber))
                {
                    var dvIncident = AdminDataAccess.GetEntityByField<DVIncident>("cp_incidentnumber", pnb.IncidentNumber);
                    
                    if (dvIncident == null)
                    {
                        dvIncident = CreateIncident(pnb, "Pocket Notebook");
                        transaction.AddCreateEntity(dvIncident);
                    }

                    dvPb.cp_incidentno= dvIncident.ToEntityReference();
                }
                if (!dvPb.cp_pocketnotebookid.HasValue)
                {
                    dvPb.cp_pocketnotebookid = Guid.NewGuid();
                }
                
                transaction.AddUpsertEntity(dvPb);

                logger.LogDebug("Create images");
                //var dvPbImages = mapper.Map<PocketNotebook, DVPocketNotebookImages>(pnb);
                //transaction.AddCreateEntityImage(dvPbImages, "cp_sketch");
                //transaction.AddCreateEntityImage(dvPbImages, "cp_signature");

                logger.LogDebug("Create photos");
                //foreach (var photo in pnb.Photos)
               // {
               //     photo.PocketNotebookId = dvPb.cp_pocketnotebookid.Value;
               //     var dvPhoto = GetDataverseEntity<DVPhoto>(photo, UserId); 
               //     transaction.AddCreateEntity(dvPhoto);
                //}

                logger.LogDebug("Save");

                if (pnb.Complete)
                {
                    //set the sudden death record to active
                    transaction.AddSetStateRequest(dvPb.ToEntityReference(), 0, 1);
                }

                AdminDataAccess.ExecuteTransaction(transaction);


                return dvPb.cp_pocketnotebookid.Value;
            }
            catch (Exception e)
            {
                logger.LogError(e,e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

       
    }
}