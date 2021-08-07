using API.DataverseAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace API.Controllers
{
    //TODO: Get DataVerse to invoke these cache removals when the associated records are changed.
    //(Otherwise the API will need to be restarted to pick up changed records outside of any cache expiry windows.)

    /// <summary>
    /// Methods supporting 'immediate' cache refresh upon change of associated records in DataVerse.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        public CacheController(
            ApiConfiguration configuration, 
            DVDataAccessFactory dataAccessFactory, 
            CacheOrchestrator cache, 
            ILogger<PNBController> log) : base(configuration, dataAccessFactory, cache, log)
        {
        }


        [HttpPatch("removelookupfield/{filterId}")]
        public ActionResult RemoveLookupFieldFromCache(string filterId)
        {
            try
            {
                if (!VerifyIntegrationKey("Cache:PATCH:removelookupfield")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"LookupField:{filterId}";
                cache.Remove(cacheKey);

                return new StatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

        [HttpPatch("removeintegrationkey/{key}")]
        public ActionResult RemoveIntegrationKeyFromCache(string key)
        {
            try
            {
                if (!VerifyIntegrationKey("Cache:PATCH:removeintegrationkey")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                Guid integrationKeyId;
                if (Guid.TryParse(key, out integrationKeyId))
                {
                    string cacheKey = $"IntegrationKey:{integrationKeyId}";
                    cache.Remove(cacheKey);
                }

                return new StatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }
    }
}