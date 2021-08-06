using API.DataverseAccess;
using API.Mappers;
using API.Models.Dataverse;
using Common.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefDataController : ControllerBase
    {
        public RefDataController(MapperConfig mapperconfig, DVDataAccessFactory dataAccessFactory, CacheOrchestrator cache, ILogger<PNBController> log) :
            base(mapperconfig, dataAccessFactory, cache, log)
        {
        }

        [HttpGet("lookupfield/{filterId}")]
        public ActionResult<LookupField> GetLookupField(string filterId)
        {
            try
            {
                if (!VerifyIntegrationKey()) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"LookupField:{filterId}";
                LookupField lookupField;
                if (!cache.TryGetValue(cacheKey, out lookupField))
                {
                    DVLookupField dvLookupField = AdminDataAccess.GetEntityByField<DVLookupField>("cp_id", filterId);
                    if (dvLookupField != null)
                    {
                        lookupField = mapper.Map(dvLookupField, lookupField);
                        var dvLookupValues = AdminDataAccess.GetAll<DVLookupValue>("cp_lookupfield", dvLookupField.Id);
                        if (dvLookupValues != null && dvLookupValues.Count > 0)
                        {
                            List<LookupValue> lookupValues = new List<LookupValue>(dvLookupValues.Count);
                            foreach (var dvLookupValue in dvLookupValues)
                            {
                                LookupValue lookupValue = mapper.Map<LookupValue>(dvLookupValue);
                                lookupValues.Add(lookupValue);
                            }
                            lookupField.Values = lookupValues;
                        }
                    }

                    cache.Set(cacheKey, lookupField);
                }

                return lookupField;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }
    }
}