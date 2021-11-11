using API.DataverseAccess;
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
                if (!VerifyIntegrationKey("SuddenDeath:POST")) return new StatusCodeResult((int)HttpStatusCode.Unauthorized);


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

                    dvIncident = CreateIncident(sd, "Sudden Death");
                    dvIncident.cp_incidentlocation = dvLocation.ToEntityReference();
                    transaction.AddCreateEntity(dvIncident);
                }

                dvSD.cp_incident = dvIncident.ToEntityReference();

                var newInvolvedContacts = new List<DVSuddenDeathInvolvedContact>();

                DVContact deceased = null;

                foreach (var contact in sd.Contacts)
                {
                    var dvContact = GetDataverseEntity<DVContact>(contact, UserId);
                    transaction.AddUpsertEntity(dvContact);


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

                            //check to see if we already have involved contact records
                            var q = new QueryExpression("cp_suddendeathinvolvedcontact");
                            q.Criteria.AddCondition("cp_contact", ConditionOperator.Equal, contact.Id);
                            q.Criteria.AddCondition("cp_suddendeath", ConditionOperator.Equal, dvSD.Id);
                            q.Criteria.AddCondition("cp_contactroletype", ConditionOperator.Equal, role);

                            var existing = AdminDataAccess.GetMultiple(q).SingleOrDefault();


                            var involved = new DVSuddenDeathInvolvedContact();
                            if (existing != null)
                            {
                                involved.Id = existing.Id;
                            }
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

                //query to see if there are any contact roles in the db which the contact no longer has (ie, have been removed in the app)
                //and delete them if so
                var qContactRoles = new QueryExpression("cp_suddendeathinvolvedcontact");
                qContactRoles.Criteria.AddCondition("cp_suddendeath", ConditionOperator.Equal, dvSD.Id);
                qContactRoles.ColumnSet = new ColumnSet(true);
                var existingContactRoles = AdminDataAccess.GetMultiple(qContactRoles);

                foreach (var existingRole in existingContactRoles)
                {
                    if (!sd.Contacts.Select(x => x.Id).Contains(existingRole.GetAttributeValue<EntityReference>("cp_contact").Id))
                    {
                        //delete all of the roles for this contact
                        var qContactRoles2 = new QueryExpression("cp_suddendeathinvolvedcontact");
                        qContactRoles2.Criteria.AddCondition("cp_contact", ConditionOperator.Equal, existingRole.GetAttributeValue<EntityReference>("cp_contact").Id);
                        qContactRoles2.ColumnSet = new ColumnSet(true);
                        var existingContactRoles2 = AdminDataAccess.GetMultiple(qContactRoles2);
                        foreach (var role in existingContactRoles2)
                        {
                            transaction.AddDeleteRequest(role.ToEntityReference());
                        }

                        //delete the contact
                        transaction.AddDeleteRequest(existingRole.GetAttributeValue<EntityReference>("cp_contact"));
                    }
                    else
                    {
                        //check to see if the contact still has the contact role
                        if (!sd.Contacts.Single(x=>x.Id == existingRole.GetAttributeValue<EntityReference>("cp_contact").Id).ContactRoles.Contains(existingRole.GetAttributeValue<EntityReference>("cp_contactroletype").Id))
                        {
                            //delete this role
                            transaction.AddDeleteRequest(existingRole.ToEntityReference());



                        }
                    }

                }




                    //add the main sd to the transaction
                    transaction.AddUpsertEntity(dvSD);

                //and the involvedContacts
                newInvolvedContacts.ForEach(x => transaction.AddUpsertEntity(x));

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
                    var dvPhoto = GetDataverseEntity<DVPhoto>(photo, UserId);
                    transaction.AddCreateEntity(dvPhoto);
                }

                foreach (var property in sd.Properties)
                {
                    var sdProperty = GetDataverseEntity<DVSuddenDeathProperty>(property, UserId,property.Id);
                    sdProperty.cp_suddendeath = dvSD.ToEntityReference();
                    transaction.AddUpsertEntity(sdProperty);

             //       var sdPropertyImages = GetDataverseEntity<DVSuddenDeathPropertyImages>(property, UserId);
             //       sdPropertyImages.Id = sdProperty.Id;
             //       transaction.AddCreateEntityImage(sdPropertyImages, "cp_propertyphoto");
             //       transaction.AddCreateEntityImage(sdPropertyImages, "cp_signature");


                }

                //query for any properties which may have been deleted
                var qProp = new QueryExpression("cp_suddendeathproperty");
                qProp.Criteria.AddCondition("cp_suddendeath", ConditionOperator.Equal, dvSD.Id);
                var existingProps = AdminDataAccess.GetMultiple(qProp);
                foreach (var existingProp in existingProps)
                {
                    if (!sd.Properties.Select(x => x.Id).Contains(existingProp.Id))
                    {
                        transaction.AddDeleteRequest(existingProp.ToEntityReference());
                    }
                }



                //if we have any of the medical fields populated
                if (!
                    (string.IsNullOrEmpty(sd.MedicalHistoryDiagnosisAnMedication)
                    && sd.MedicalHistoryLastVisitDate == null
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

                    //check to see if we have a medical history for this sudden death deceased
                    var medicalHistoryId = FindMedicalHistory(deceased.Id);

                    var dvMedicalHistory = GetDataverseEntity<DVMedicalHistory>(sd, UserId, medicalHistoryId);

                    if (accountId.HasValue)
                    {
                        dvMedicalHistory.cp_gp = new EntityReference("account", accountId.Value);
                    }
                    dvMedicalHistory.cp_contact = deceased.ToEntityReference();
                    dvMedicalHistory.cp_medicalhistoryname = $"Medical History Deceased: {deceased.firstname} {deceased.lastname}";
                    transaction.AddUpsertEntity(dvMedicalHistory);


                }

                //query for and disassociate any existing links
                var q2 = new QueryExpression("cp_suddendeath_cidcsiofficers");
                q2.Criteria.AddCondition("cp_suddendeathid", ConditionOperator.Equal, dvSD.Id);
                q2.ColumnSet = new ColumnSet(true);
                var existingLinks = AdminDataAccess.GetMultiple(q2);
                foreach (var link in existingLinks)
                {
                    transaction.AddDisassociateEntities(dvSD.ToEntityReference(), new EntityReference("systemuser", link.GetAttributeValue<Guid>("systemuserid")), "cp_suddendeath_SystemUser_SystemUser");
                }


                //re set up the new ones
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


                if (sd.Submitted)
                {
                    //set the sudden death record to active
                    transaction.AddSetStateRequest(dvSD.ToEntityReference(),0,1);
                }

                AdminDataAccess.ExecuteTransaction(transaction);


                return dvSD.Id;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

        [HttpPatch("{suddenDeathId}/{imageType}")]
        [HttpPatch("{suddenDeathId}/{parentId}/{imageType}")]
        public ActionResult UpdateImage(Guid suddenDeathId,Guid? parentId, string imageType, [FromBody] string newImage)
        {

            try
            {

            

            var trans = new DVTransaction();

            if (imageType == "property")
            {
                var sd = new DVSuddenDeathPropertyImages();
                sd.Id = suddenDeathId;
                sd.cp_suddendeathpropertyid = parentId;
                sd.cp_propertyphoto = Convert.FromBase64String(newImage);

                trans.AddCreateEntityImage(sd, "cp_propertyphoto");

            }
            if (imageType == "propertysignature")
            {
                var sd = new DVSuddenDeathPropertyImages();
                sd.Id = suddenDeathId;
                sd.cp_suddendeathpropertyid = parentId;
                sd.cp_signature = Convert.FromBase64String(newImage);

                trans.AddCreateEntityImage(sd, "cp_signature");

            }
            if (imageType == "suicide")
            {
                var sd = new DVSuddenDeathImages();
                sd.Id = suddenDeathId;
                sd.cp_suicidenotepicture = Convert.FromBase64String(newImage);

                trans.AddCreateEntityImage(sd, "cp_suicidenotepicture");

            }
            
             if (imageType == "identificationsignature")
            {
                var sd = new DVSuddenDeathImages();
                sd.Id = suddenDeathId;
                sd.cp_identificationsignature = Convert.FromBase64String(newImage);

                trans.AddCreateEntityImage(sd, "cp_identificationsignature");

            }

            if (imageType == "circumstances")
            {
                var sd = AdminDataAccess.GetEntityByField<DVSuddenDeath>("cp_suddendeathid", suddenDeathId);
                var incident = AdminDataAccess.GetEntityByField<DVIncident>("cp_incidentid", sd.cp_incident.Id);

                //see if we already have a photo


                var photo = new Photo();
                photo.SuddenDeathId = suddenDeathId;
                photo.Caption = $"Photo 1, Sudden Death for Incident {incident.cp_incidentnumber}";
                photo.Blob = newImage;

                var existingPhoto = AdminDataAccess.GetEntityByField<DVPhoto>("cp_suddendeath", suddenDeathId);
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

            AdminDataAccess.ExecuteTransaction(trans);


            return new OkResult();
            }
            catch (Exception ex)
            {

                return new NotFoundResult(); 
            }
        }

        protected Guid? FindMedicalHistory(Guid contactId)
        {
            var q = new QueryExpression("cp_medicalhistory");
            q.Criteria.AddCondition("cp_contact", ConditionOperator.Equal, contactId);
            var existing = AdminDataAccess.GetMultiple(q);
            if (existing.Any())
            {
                return existing.First().Id;
            }
            else
            {
                return null;
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
                transaction.AddUpsertEntity(newAccount);
                return newAccount.Id;
            }

            
        }


    }
}
