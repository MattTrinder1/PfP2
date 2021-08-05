﻿using API.DataverseAccess;
using API.Mappers;
using API.Models.IYC;
using API.Models.PNB;
using AutoMapper;
using Common.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PNBController : ControllerBase
    {
        public PNBController(MapperConfig mapperconfig, DVDataAccessFactory dataAccessFactory, ILogger<PNBController> log) : 
            base(mapperconfig, dataAccessFactory, log)
        {
        }

        [HttpGet("getwhereowner")]
        public ActionResult<List<PocketNotebookListEntry>> GetListForUser()
        {
            try
            {
                logger.LogDebug(Request.Headers["UserEmail"].ToString());

                var userId = AdminDataAccess.GetUserId(Request.Headers["UserEmail"].ToString());

                logger.LogDebug(userId.ToString());

                ICollection<DVPocketNotebook> pnb = UserDataAccess.GetAll<DVPocketNotebook>("ownerid", userId, "cp_notedateandtime");

                return mapper.Map<List<PocketNotebookListEntry>>(pnb);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }


        private Guid? FindOrCreateIncident(string incidentNumber, DVTransaction transaction)
        {
            if (string.IsNullOrEmpty(incidentNumber))
            {
                return null;
            }

            Guid? incidentId = null;
            var incident = AdminDataAccess.GetEntityByField<DVIncident>("cp_incidentnumber", incidentNumber);
            if (incident == null)
            {
                incident = new DVIncident();
                incident.cp_incidentnumber = incidentNumber;
                incidentId = Guid.NewGuid();
                incident.cp_incidentid = incidentId;

                if (transaction != null)
                {
                    transaction.AddCreateEntity(incident);
                }
                else
                {
                    incidentId = UserDataAccess.CreateEntity(incident);
                }
            }
            else
            {
                incidentId = incident.cp_incidentid;
            }

            return incidentId;
        }

        [HttpGet("{id}")]
        public ActionResult<PocketNotebook> Get(string id)
        {
            try
            {
                logger.LogDebug(Request.Headers["UserEmail"].ToString());

                DVPocketNotebook pnb = UserDataAccess.GetEntityByField<DVPocketNotebook>("cp_pocketnotebookid", id);

                DVPocketNotebookImages pnbImages = UserDataAccess.GetEntityByField<DVPocketNotebookImages>("cp_pocketnotebookid", id, SelectColumns.TypePropertiesWithoutImages);
                UserDataAccess.GetImages(pnbImages, true);

                DVIncident incident = null;
                if (pnb.cp_incidentno != null)
                {
                    incident = UserDataAccess.GetEntityByField<DVIncident>("cp_incidentid", pnb.cp_incidentno.EntityId.Value.ToString());
                }

                var pnbPhotosCol = UserDataAccess.GetAll<DVPhoto>("cp_pocketnotebook", pnb.cp_pocketnotebookid, SelectColumns.TypePropertiesWithoutImages);
                UserDataAccess.GetImages(pnbPhotosCol, true);

                PocketNotebook result = mapper.Map<PocketNotebook>(pnb);
                result = mapper.Map(pnbImages, result);
                if (incident != null)
                {
                    result = mapper.Map(incident, result);
                }
                if (pnbPhotosCol != null && pnbPhotosCol.Count > 0)
                {
                    List<Photo> photos = new List<Photo>(pnbPhotosCol.Count);
                    foreach (var pnbPhoto in pnbPhotosCol)
                    {
                        Photo photo = mapper.Map<Photo>(pnbPhoto);
                        photos.Add(photo);
                    }
                    result.Photos = photos;
                }

                return result;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }

        [HttpPost()]
        public ActionResult<Guid> Post([FromBody] PocketNotebook pnb)
        {
            try
            {
                string userEmail = Request.Headers["UserEmail"].ToString();

                var dvPb = mapper.Map<DVPocketNotebook>(pnb);

                DVTransaction transaction = new DVTransaction();

                Guid? incidentId = FindOrCreateIncident(pnb.IncidentNumber, transaction);
                if (incidentId != null)
                {
                    dvPb.cp_incidentno = new EntityReference("cp_incident", incidentId);
                }

                Guid pnbGuid = Guid.Empty;
                if (dvPb.cp_pocketnotebookid.HasValue)
                {
                    pnbGuid = dvPb.cp_pocketnotebookid.Value;
                }
                if (pnbGuid == Guid.Empty)
                {
                    pnbGuid = Guid.NewGuid();
                    dvPb.cp_pocketnotebookid = pnbGuid;
                }
                transaction.AddCreateEntity(dvPb);

                var dvPbImages = mapper.Map<PocketNotebook, DVPocketNotebookImages>(pnb);
                transaction.AddCreateEntityImage(pnbGuid, dvPbImages, x => x.cp_sketch);
                transaction.AddCreateEntityImage(pnbGuid, dvPbImages, x => x.cp_signature);

                foreach (var photo in pnb.Photos)
                {
                    photo.PocketNotebookId = pnbGuid;
                    var dvPhoto = mapper.Map<DVPhoto>(photo);
                    transaction.AddCreateEntity(dvPhoto);
                }

                transaction.Execute(UserDataAccess.DVService);

                return pnbGuid;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }
    }
}