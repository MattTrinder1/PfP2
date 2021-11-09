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
    public class PhotoController : ControllerBase
    {
        public PhotoController(
            ApiConfiguration configuration,
            DVDataAccessFactory dataAccessFactory,
            CacheOrchestrator cache,
            ILogger<PNBController> log) : base(configuration, dataAccessFactory, cache, log)
        {
        }




        [HttpGet("{id}")]
        public ActionResult<Photo> Get(string id)
        {
            try
            {
                if (!VerifyIntegrationKey("Photo:GET")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                DVPhoto photo = UserDataAccess.GetEntityByField<DVPhoto>("cp_photoid", id);

                UserDataAccess.GetImages(photo, true);

                Photo result = mapper.Map<Photo>(photo);

                return result;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

        [HttpGet("thumbnail/{id}")]
        public ActionResult<Photo> GetThumbnail(string id)
        {
            try
            {
                if (!VerifyIntegrationKey("Photo:GET")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                DVPhoto photo = UserDataAccess.GetEntityByField<DVPhoto>("cp_photoid", id);

                UserDataAccess.GetImages(photo,false);

                Photo result = mapper.Map<Photo>(photo);

                return result;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }


    }
}