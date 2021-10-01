using API.DataverseAccess;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefDataController : ControllerBase
    {
        public RefDataController(
            ApiConfiguration configuration,
            DVDataAccessFactory dataAccessFactory, 
            CacheOrchestrator cache, 
            ILogger<PNBController> log) : base(configuration, dataAccessFactory, cache, log)
        {
        }
         
        [HttpGet("lookupfield/{filterId}")]
        public ActionResult<LookupField> GetLookupField(string filterId)
        {
            try
            {
                if (!VerifyIntegrationKey("RefData:GET:lookupfield")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"LookupField:{filterId}";
                LookupField lookupField;
                if (!cache.TryGetValue(cacheKey, out lookupField))
                {
                    DVLookupField dvLookupField = AdminDataAccess.GetEntityByField<DVLookupField>("cp_id", filterId);
                    if (dvLookupField != null)
                    {
                        lookupField = mapper.Map(dvLookupField, lookupField);
                        var dvLookupValues = AdminDataAccess.GetAll<DVLookupValue>("cp_lookupfield", dvLookupField.Id, "cp_displaysequenceno");
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

                        cache.Set(cacheKey, lookupField);
                    }
                }

                return lookupField;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

        [HttpGet("territorialpolicingarea")]
        public ActionResult<LookupField> GetTerritorialPolicingArea()
        {
            try
            {
                if (!VerifyIntegrationKey("RefData:GET:lookupfield")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"TerritorialPolicingArea";
                LookupField lookupField;
                if (!cache.TryGetValue(cacheKey, out lookupField))
                {
                    lookupField = new LookupField();
                    var dvTerritorialPolicingAreas = AdminDataAccess.GetAll<DVTerritorialPolicingArea>(orderby: "cp_name");
                    if (dvTerritorialPolicingAreas != null && dvTerritorialPolicingAreas.Count > 0)
                    {
                        List<LookupValue> lookupValues = new List<LookupValue>(dvTerritorialPolicingAreas.Count);
                        foreach (var dvTerritorialPolicingArea in dvTerritorialPolicingAreas)
                        {
                            LookupValue lookupValue = mapper.Map<LookupValue>(dvTerritorialPolicingArea);
                            lookupValues.Add(lookupValue);
                        }
                        lookupField.Values = lookupValues;
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

        [HttpGet("contactroletype")]
        public ActionResult<LookupField> GetContactRoleType()
        {
            try
            {
                if (!VerifyIntegrationKey("RefData:GET:lookupfield")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"ContactRoleType";
                LookupField lookupField;
                if (!cache.TryGetValue(cacheKey, out lookupField))
                {
                    lookupField = new LookupField();
                    var dvContactRoleTypes = AdminDataAccess.GetAll<DVContactRoleType>(orderby: "cp_name");
                    if (dvContactRoleTypes != null && dvContactRoleTypes.Count > 0)
                    {
                        List<LookupValue> lookupValues = new List<LookupValue>(dvContactRoleTypes.Count);
                        foreach (var dvContactRoleType in dvContactRoleTypes)
                        {
                            LookupValue lookupValue = mapper.Map<LookupValue>(dvContactRoleType);
                            lookupValues.Add(lookupValue);
                        }
                        lookupField.Values = lookupValues;
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


        [HttpGet("civilianusers")]
        public ActionResult<User[]> GetCivilianUsers()
        {
            return GetUsers(null);
        }

        [HttpGet("officers/{badgeNumberBeginsWith}")]
        public ActionResult<User[]> GetUsers(string badgeNumberBeginsWith)
        {
            try
            {
                if (!VerifyIntegrationKey("RefData:GET:users")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"Users:{badgeNumberBeginsWith}";
                List<User> users;
                if (!cache.TryGetValue(cacheKey, out users))
                {
                    FilterExpression criteria = new FilterExpression();
                    if (!string.IsNullOrWhiteSpace(badgeNumberBeginsWith))
                    {
                        criteria.AddCondition("cp_badgenumber", ConditionOperator.BeginsWith, badgeNumberBeginsWith);
                    }
                    else
                    {
                        criteria.AddCondition("cp_badgenumber", ConditionOperator.Null);
                    }
                    OrderExpression order = new OrderExpression("fullname", OrderType.Ascending);

                    var dvUsers = AdminDataAccess.GetAll<DVUser>(criteria, order);

                    if (dvUsers != null && dvUsers.Count > 0)
                    {
                        users = new List<User>(dvUsers.Count);
                        foreach (var dvUser in dvUsers)
                        {
                            User user = mapper.Map<User>(dvUser);
                            users.Add(user);
                        }

                        cache.Set(cacheKey, users);
                    }
                }

                return users != null ? users.ToArray() : new User[0];
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }
    }
}