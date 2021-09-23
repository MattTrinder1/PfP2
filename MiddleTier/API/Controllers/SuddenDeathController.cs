using API.DataverseAccess;
using API.Models.Dataverse;
using API.Models.PNB;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuddenDeathController : ControllerBase
    {

        public SuddenDeathController(
         ApiConfiguration configuration,
         DVDataAccessFactory dataAccessFactory,
         CacheOrchestrator cache,
         ILogger<PNBController> log) : base(configuration, dataAccessFactory, cache, log)
        {
        }
        [HttpPost()]
        public ActionResult<Guid> Post([FromBody] SuddenDeath sd)
        {
            try
            {
                if (!VerifyIntegrationKey("PNB:POST")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);


                sd.Name = $"Sudden Death for Incident {sd.IncidentNumber}";

                var dvSD = GetDataverseEntity<DVSuddenDeath>(sd, UserId);

                //entites are created in the order they are added to the transaction, so the sequence of event is important.
                //contacts need to be created first

                DVTransaction transaction = new DVTransaction();

                var dvIncident = AdminDataAccess.GetEntityByField<DVIncident>("cp_incidentnumber", sd.IncidentNumber);
                DVLocation dvLocation = null;

                if (dvIncident != null)
                {
                    if (dvIncident.cp_incidentlocation == null)
                    {
                        dvLocation = GetDataverseEntity<DVLocation>(sd, UserId);
                        dvLocation.cp_name = $"Location, Incident {sd.IncidentNumber}";
                        transaction.AddCreateEntity(dvLocation);

                        dvIncident.cp_incidentlocation = dvLocation.ToEntityReference();
                        transaction.AddUpdateEntity(dvIncident);
                    }
                }
                else
                {
                    dvLocation = GetDataverseEntity<DVLocation>(sd, UserId);
                    dvLocation.cp_name = $"Location, Incident {sd.IncidentNumber}";
                    transaction.AddCreateEntity(dvLocation);

                    dvIncident = CreateIncident(sd.IncidentNumber, sd.IncidentDate, "Sudden Death");
                    dvIncident.cp_incidentlocation = dvLocation.ToEntityReference();
                    transaction.AddCreateEntity(dvIncident);
                }
                
                dvSD.cp_incident = dvIncident.ToEntityReference();

                var newInvolvedContacts = new List<DVSuddenDeathInvolvedContact>();

                DVContact deceased = null;

                foreach (var contact in sd.Contacts)
                {
                    var dvContact = GetDataverseEntity<DVContact>(contact,UserId);
                    transaction.AddCreateEntity(dvContact);
                    

                    foreach (var role in contact.ContactRoles)
                    {
                        string roleName = (string)AdminDataAccess.GetEntityFieldValue("cp_contactroletype", role, "cp_name");

                        if (roleName == "Deceased")
                        {
                            deceased = dvContact;
                            //set the deceased field on the sudden death record
                            dvSD.cp_deceased = dvContact.ToEntityReference();
                        }
                        if (roleName == "Identifier")
                        {
                            //set the identified by field on the sudden death record, 
                            dvSD.cp_identifiedby = dvContact.ToEntityReference();

                            //and get these 2 off the identifier contact
                            dvSD.cp_identificationlocation = contact.LocationOfId;
                            dvSD.cp_identificationsignedon = contact.SignDate;

                        }
                        if (roleName != "Deceased")
                        {
                            //create the involved contact record - not sure why deceased don't get one...
                         
                            var involved = new DVSuddenDeathInvolvedContact();
                            involved.cp_contact = dvContact.ToEntityReference();
                            involved.cp_suddendeath = dvSD.ToEntityReference();
                            involved.cp_contactroletype = new EntityReference("cp_contactroletype", role);
                            involved.cp_involvedcontactname = $"Involved Contact for Sudden Death, Incident {sd.IncidentNumber}";
                            involved.cp_relationshiptodeceased = contact.DeceasedRelationship;
                            involved.cp_relationshiptodeceasedduration = contact.DeceasedRelationshipDuration;
                            newInvolvedContacts.Add(involved);

                            
                        }
                    }

                }
                
                //add the main sd to the transaction
                transaction.AddCreateEntity(dvSD);

                //and the involvedContacts
                newInvolvedContacts.ForEach(x => transaction.AddCreateEntity(x));

                var dvSDImages = mapper.Map<SuddenDeath, DVSuddenDeathImages>(sd);
                transaction.AddCreateEntityImage(dvSDImages, "cp_suicidenotepicture");
                transaction.AddCreateEntityImage(dvSDImages, "cp_identificationsignature");

                //circumstances photo goes to photo 1
                if (!string.IsNullOrEmpty(sd.PhotoCircumstances))
                {
                    var photo = new Photo();
                    photo.SuddenDeathId = dvSD.Id;
                    photo.Caption = $"Photo 1, Sudden Death for Incident {sd.IncidentNumber}";
                    photo.Blob = sd.PhotoCircumstances;
                    var dvPhoto = GetDataverseEntity<DVPhoto>(photo,UserId);
                    transaction.AddCreateEntity(dvPhoto);
                }

                foreach (var property in sd.Properties)
                {
                    var sdProperty = GetDataverseEntity<DVSuddenDeathProperty>(property, UserId);
                    sdProperty.cp_suddendeath = dvSD.ToEntityReference();
                    transaction.AddCreateEntity(sdProperty);

                    var sdPropertyImages = GetDataverseEntity<DVSuddenDeathPropertyImages>(property, UserId);
                    sdPropertyImages.Id = sdProperty.Id;
                    transaction.AddCreateEntityImage(sdPropertyImages, "cp_propertyphoto");
                    transaction.AddCreateEntityImage(sdPropertyImages, "cp_signature");


                }

                //if we have any of the medical fields populated
                if (!
                    (string.IsNullOrEmpty(sd.MedicalHistoryDiagnosisAnMedication)
                    && string.IsNullOrEmpty(sd.MedicalHistoryLastVisitDate)
                    && string.IsNullOrEmpty(sd.MedicalHistoryPastHistory)
                    && string.IsNullOrEmpty(sd.MedicalHistoryReasonForVisit)
                    && string.IsNullOrEmpty(sd.MedicalHistoryRiskFactors)))
                {
                    //if we have a gp name then locate/create the account for the GP
                    Guid? accountId = null;
                    if (!string.IsNullOrEmpty(sd.GPName))
                    {
                        accountId = FindOrCreateGPAccount(sd, transaction);
                    }

                    var dvMedicalHistory = GetDataverseEntity<DVMedicalHistory>(sd, UserId);
                    if (accountId.HasValue)
                    {
                        dvMedicalHistory.cp_gp = new EntityReference("account", accountId.Value);
                    }
                    dvMedicalHistory.cp_contact = deceased.ToEntityReference();
                    dvMedicalHistory.cp_medicalhistoryname = $"Medical History Deceased: {deceased.firstname} {deceased.lastname}";
                    transaction.AddCreateEntity(dvMedicalHistory);


                }

                foreach (var CIDId in sd.CIDCSISelectedIds)
                {
                    transaction.AddAssociateEntities(dvSD.ToEntityReference(), new EntityReference("systemuser", CIDId), "cp_suddendeath_SystemUser_SystemUser");
                }

                //foreach (var photo in pnb.Photos)
                //{
                //    photo.PocketNotebookId = pnbGuid;
                //    var dvPhoto = mapper.Map<DVPhoto>(photo);
                //    transaction.AddCreateEntity(dvPhoto);
                //}

                    AdminDataAccess.ExecuteTransaction(transaction);


                return dvSD.Id;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }


        protected Guid? FindOrCreateGPAccount(SuddenDeath sd, DVTransaction transaction)
        {
            if (string.IsNullOrEmpty(sd.GPName))
            {
                return null;
            }

            var q = new QueryExpression("account");
            q.Criteria.AddCondition("name", ConditionOperator.Equal, sd.GPName);
            q.Criteria.AddCondition("telephone1", ConditionOperator.Equal, sd.GPPhoneNumber);
            var existing = AdminDataAccess.GetMultiple(q);

            if (existing.Any())
            {
                return existing.First().Id;
            }
            else
            {
                var newAccount = GetDataverseEntity<DVAccount>(sd);
                transaction.AddCreateEntity(newAccount);
                return newAccount.Id;
            }

            
        }


    }
}
