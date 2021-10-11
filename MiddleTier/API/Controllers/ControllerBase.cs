using API.DataverseAccess;
using AutoMapper;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

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
        protected ApiConfiguration configuration = null; 

        public ControllerBase(
            ApiConfiguration configuration, 
            DVDataAccessFactory dataAccessFactory, 
            CacheOrchestrator cache, 
            ILogger<PNBController> logger)
        {
            this.configuration = configuration;
            this.mapper = new Mapper(configuration.MapperConfiguration.mapperConfig);
            this.dataAccessFactory = dataAccessFactory;
            this.cache = cache;
            this.logger = logger;
        }

        public Guid UserId { get { return UserDataAccess.UserId.Value; } }

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

        protected DVIncident CreateIncident(IncidentRelatedEntityBase sd, string incidentType)
        {
            if (string.IsNullOrEmpty(sd.IncidentNumber))
            {
                return null;
            }

            var incidentTypeId = AdminDataAccess.GetEntityId("cp_incidenttype", "cp_incidenttypename", incidentType);

            var incident = new DVIncident();
            incident.cp_incidentnumber = sd.IncidentNumber;
            incident.cp_incidenttype = new EntityReference("cp_incidenttype", incidentTypeId.Value);
            if (sd.IncidentDate.HasValue)
            {
                incident.cp_incidentdate = sd.IncidentDate;
            }
            if (!string.IsNullOrEmpty(sd.PrimaryOfficer))
            {
                incident.cp_reportingofficer = adminDataAccess.GetOptionSetValue("cp_yes_no_na_list", sd.PrimaryOfficer);
            }
            if (!string.IsNullOrEmpty(sd.SingleOfficer))
            {
                incident.cp_singleofficerevent = adminDataAccess.GetOptionSetValue("cp_yes_no_na_list", sd.SingleOfficer);
            }
            if (sd.AdditionalOfficerIds.Any())
            {
                //should only be one
                incident.cp_additionalofficer = new EntityReference("systemuser", sd.AdditionalOfficerIds.Single());
            }


            incident.cp_enteredby = new EntityReference("systemuser", UserId);
            incident.ownerid = new EntityReference("systemuser", UserId);


            return incident;
        }


        protected bool VerifyIntegrationKey(string forInvocationOf)
        {
            string userEmail = Request.Headers["UserEmail"];
            string integrationKeyString = Request.Headers["IntegrationKey"];

            logger.LogDebug($"UserEmail: {userEmail} IntegrationKey: {integrationKeyString}");

            if (configuration.SuppressIntegrationKeyVerification)
            {
                logger.LogWarning("SuppressIntegrationKeyVerification = true");
                return true;
            }

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
                        isValid = IsInvocationAllowed(integrationKey, forInvocationOf);
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
                    //(Also covers scenarios of previously cached expiry having been moved forward 
                    // or previously cached restrictions having been loosened.)
                    KeyValuePair<string, object>[] queryConditions = new KeyValuePair<string, object>[]
                    {
                        new KeyValuePair<string, object>("cp_integrationkeyid", integrationKeyId),
                        new KeyValuePair<string, object>("statecode", 0 /* Active */),
                    };
                    integrationKey = AdminDataAccess.GetEntityByFields<DVIntegrationKey>(queryConditions);

                    if (integrationKey != null)
                    {
                        //The key itself is valid so cache it (regardless of whether it is valid for this specific invocation or not).
                        if (integrationKey.cp_expiry == null)
                        {
                            cache.Set(cacheKey, integrationKey);
                            isValid = IsInvocationAllowed(integrationKey, forInvocationOf);
                        }
                        else if (integrationKey.cp_expiry.Value.ToUniversalTime() > DateTime.Now)
                        {
                            cache.Set(cacheKey, integrationKey, integrationKey.cp_expiry.Value.ToUniversalTime());
                            isValid = IsInvocationAllowed(integrationKey, forInvocationOf);
                        }
                    }
                }
            }

            return isValid;
        }

        private bool IsInvocationAllowed(DVIntegrationKey integrationKey, string invocationOf)
        {
            if (string.IsNullOrWhiteSpace(integrationKey.cp_restrictedto) || string.IsNullOrWhiteSpace(invocationOf))
            {
                return true;
            }
            else
            {
                string[] validInvocations = integrationKey.cp_restrictedto.Split('\n');
                foreach (string validInvocation in validInvocations)
                {
                    if (validInvocation == invocationOf) return true;
                }
            }
            return false;
        }

        protected T GetDataverseEntity<T>(IncidentRelatedEntityBase entity, Guid? ownerId = null) where T:DVEntityBase
        {
            var dvEntity = mapper.Map<T>(entity);

            if (dvEntity.Id == Guid.Empty)
            {
                dvEntity.Id = Guid.NewGuid();
            }
            if (typeof(T).GetProperty("cp_enteredby") != null && ownerId.HasValue)
            {
                typeof(T).GetProperty("cp_enteredby").SetValue(dvEntity, new EntityReference("systemuser", ownerId.Value));
            }
            if (typeof(T).GetProperty("ownerid") != null && ownerId.HasValue)
            {
                typeof(T).GetProperty("ownerid").SetValue(dvEntity, new EntityReference("systemuser", ownerId.Value));
            }

            return dvEntity;
        }
    }
}
