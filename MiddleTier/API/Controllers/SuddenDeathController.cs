using API.DataverseAccess;
using API.Models.Dataverse;
using API.Models.PNB;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
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

                Guid? incidentId = FindOrCreateIncident(sd.IncidentNumber,sd.IncidentDate,"Sudden Death", transaction);
                if (incidentId != null)
                {
                    dvSD.cp_incident = new EntityReference("cp_incident", incidentId.Value);
                }




                var newInvolvedContacts = new List<DVSuddenDeathInvolvedContact>();                

                foreach (var contact in sd.Contacts)
                {
                    var dvContact = GetDataverseEntity<DVContact>(contact,UserId);
                    transaction.AddCreateEntity(dvContact);
                    

                    foreach (var role in contact.ContactRoles)
                    {
                        string roleName = (string)AdminDataAccess.GetEntityFieldValue("cp_contactroletype", role, "cp_name");

                        if (roleName == "Deceased")
                        {
                            //set the deceased field on the sudden death record
                            dvSD.cp_deceased = dvContact.ToEntityReference();
                        }
                        if (roleName == "Identifier")
                        {
                            //set the identified by field on the sudden death record, 
                            dvSD.cp_identifiedby = dvContact.ToEntityReference();
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
                
                //circumstances photo goes to photo 1
                if (!string.IsNullOrEmpty(sd.PhotoCircumstances))
                {
                    var photo = new Photo();
                    photo.SuddenDeathId = dvSD.Id;
                    photo.Caption = $"Photo 1, Sudden Death for Incident {sd.IncidentNumber}";
                    photo.Blob = sd.PhotoCircumstances;
                    var dvPhoto = mapper.Map<DVPhoto>(photo);
                    transaction.AddCreateEntity(dvPhoto);
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


    }
}
