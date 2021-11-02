using API.DataverseAccess;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("configdataversion")]
        public ActionResult<ConfigDataVersion> GetConfigDataVersion()
        {
            try
            {
                if (!VerifyIntegrationKey("RefData:GET:lookupfield")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"ConfigDataVersion";
                string configDataVersion = "";
                if (!cache.TryGetValue(cacheKey, out configDataVersion))
                {
                    var q = new QueryExpression("cp_configdatahistory");
                    q.Criteria.AddCondition("cp_name", ConditionOperator.Equal, "CurrentVersion");
                    q.ColumnSet = new ColumnSet(true);
                    var res = AdminDataAccess.GetMultiple(q).SingleOrDefault();

                    if (res != null)
                    {
                        configDataVersion = res.GetAttributeValue<string>("cp_value");
                    }
                    cache.Set(cacheKey, configDataVersion);
                }

                return new ConfigDataVersion() { ConfigDataVersionString = configDataVersion};
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
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
                        lookupField.Values = mapper.Map<List<LookupValue>>(dvLookupValues);

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
                string customerSettingCacheKey = $"CustomerSetting";
                LookupField lookupField;
                DVCustomerSetting customerSetting;

                //get the currently active customer setting
                if (!cache.TryGetValue(customerSettingCacheKey, out customerSetting))
                {
                    var dvCustomerSettings = AdminDataAccess.GetAll<DVCustomerSetting>(field: "cp_active", value: true);
                    cache.Set(customerSettingCacheKey, dvCustomerSettings.Single());
                    customerSetting = dvCustomerSettings.Single();
                }


                if (!cache.TryGetValue(cacheKey, out lookupField))
                {
                    
                    var dvTerritorialPolicingAreas = AdminDataAccess.GetAll<DVTerritorialPolicingArea>(field:"cp_customer",value:customerSetting.cp_customersettingid,  orderby: "cp_name");
                    lookupField = new LookupField(mapper.Map<List<LookupValue>>(dvTerritorialPolicingAreas)); 
                    cache.Set(cacheKey, lookupField);
                }

                return lookupField;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

        [HttpGet("contactroletype/{application}")]
        public ActionResult<LookupField> GetContactRoleType(int application)
        {
            try
            {
                if (!VerifyIntegrationKey("RefData:GET:lookupfield")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

                string cacheKey = $"ContactRoleType:{application}";
                LookupField lookupField;
                if (!cache.TryGetValue(cacheKey, out lookupField))
                {
                    lookupField = new LookupField();
                    var f = new FilterExpression();
                    f.AddCondition("cp_applicationused", ConditionOperator.Equal, application);
                    var o = new OrderExpression("cp_name", OrderType.Ascending);

                    var dvContactRoleTypes = AdminDataAccess.GetAll<DVContactRoleType>(f,o);
                
                    lookupField.Values = mapper.Map<List<LookupValue>>(dvContactRoleTypes);
                
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
                    users = new List<User>();
                    users.AddRange(mapper.Map<List<User>>(dvUsers));
                    
                    cache.Set(cacheKey, users);
                    
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