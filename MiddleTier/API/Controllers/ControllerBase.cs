using API.DataverseAccess;
using API.Mappers;
using API.Models.Dataverse;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private DVDataAccessFactory dataAccessFactory = null;
        private DVDataAccess adminDataAccess = null;
        private DVDataAccess userDataAccess = null;
        protected IMapper mapper = null;
        protected ILogger logger = null;
        protected CacheOrchestrator cache = null;

        public ControllerBase(MapperConfig mapperconfig, DVDataAccessFactory dataAccessFactory, CacheOrchestrator cache, ILogger<PNBController> logger)
        {
            this.mapper = new Mapper(mapperconfig.mapperConfig);
            this.dataAccessFactory = dataAccessFactory;
            this.cache = cache;
            this.logger = logger;
        }

        public DVDataAccess UserDataAccess
        {
            get
            {
                if (userDataAccess == null)
                {
                    userDataAccess = dataAccessFactory.GetUserDataAccess();
                }

                return userDataAccess;
            }
        }

        public DVDataAccess AdminDataAccess
        {
            get
            {
                if (adminDataAccess == null)
                {
                    adminDataAccess = dataAccessFactory.GetAdminDataAccess();
                }

                return adminDataAccess;
            }
        }

        protected bool VerifyIntegrationKey()
        {
            string userEmail = Request.Headers["UserEmail"];
            string integrationKeyString = Request.Headers["IntegrationKey"];

            logger.LogDebug($"UserEmail: {userEmail} IntegrationKey: {integrationKeyString}");

            bool isValid = false;
            Guid integrationKeyId;
            if (Guid.TryParse(integrationKeyString, out integrationKeyId))
            {
                string cacheKey = $"IntegrationKey:{integrationKeyId}";
                DVIntegrationKey integrationKey;
                if (cache.TryGetValue(cacheKey, out integrationKey))
                {
                    if (integrationKey.cp_expiry == null || integrationKey.cp_expiry.Value.ToUniversalTime() > DateTime.Now)
                    {
                        isValid = true;
                    }
                    else
                    {
                        //Expired
                        cache.Remove(cacheKey);
                    }
                }

                if (!isValid)
                {
                    //Not cached as a valid integration key so check DataVerse.
                    //(Also covers scenario of previously cached expiry having been moved forward.)
                    KeyValuePair<string, object>[] queryConditions = new KeyValuePair<string, object>[]
                    {
                        new KeyValuePair<string, object>("cp_integrationkeyid", integrationKeyId),
                        new KeyValuePair<string, object>("statecode", 0 /* Active */),
                    };
                    integrationKey = AdminDataAccess.GetEntityByFields<DVIntegrationKey>(queryConditions);

                    if (integrationKey != null)
                    {
                        if (integrationKey.cp_expiry == null)
                        {
                            isValid = true;
                            cache.Set(cacheKey, integrationKey);
                        }
                        else if (integrationKey.cp_expiry.Value.ToUniversalTime() > DateTime.Now)
                        {
                            isValid = true;
                            cache.Set(cacheKey, integrationKey, integrationKey.cp_expiry.Value.ToUniversalTime());
                        }
                    }
                }
            }

            return isValid;
        }
    }
}
