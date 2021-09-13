using API.DataverseAccess;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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


                var dvSD = mapper.Map<DVSuddenDeath>(sd);

                DVTransaction transaction = new DVTransaction();

                //Guid? incidentId = FindOrCreateIncident(pnb.IncidentNumber, transaction);
                //if (incidentId != null)
                //{
                //    dvPb.cp_incidentno = new EntityReference("cp_incident", incidentId);
                //}

                Guid pnbGuid = Guid.Empty;
                if (dvSD.cp_suddendeathid.HasValue)
                {
                    pnbGuid = dvSD.cp_suddendeathid.Value;
                }
                if (pnbGuid == Guid.Empty)
                {
                    pnbGuid = Guid.NewGuid();
                    dvSD.cp_suddendeathid = pnbGuid;
                }


                transaction.AddCreateEntity(dvSD);

                //var dvPbImages = mapper.Map<PocketNotebook, DVPocketNotebookImages>(pnb);
                //transaction.AddCreateEntityImage(pnbGuid, dvPbImages, "cp_sketch");
                //transaction.AddCreateEntityImage(pnbGuid, dvPbImages, "cp_signature");

                //foreach (var photo in pnb.Photos)
                //{
                //    photo.PocketNotebookId = pnbGuid;
                //    var dvPhoto = mapper.Map<DVPhoto>(photo);
                //    transaction.AddCreateEntity(dvPhoto);
                //}

                UserDataAccess.ExecuteTransaction(transaction);


                return pnbGuid;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }


    }
}
