using AutoMapper;
using Azure.Storage.Blobs;
using Common.Models.Business;
using Common.Models.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace API.Mappers
{
    public class QueueToBusinessProfile : Profile
    {
        private BlobContainerClient _PNBcontainerClient;
        private BlobContainerClient _SDcontainerClient;

        public QueueToBusinessProfile(BlobContainerClientFactory containerClientFactory)
        {
            _PNBcontainerClient = containerClientFactory.GetBlobContainerClient("pnb");
            _SDcontainerClient = containerClientFactory.GetBlobContainerClient("suddendeath");

            CreateMap<JsonElement, Photo>()
                .ForMember(dest => dest.Caption, map => map.MapFrom(src => src.GetProperty("caption").GetString()))
                .ForMember(dest => dest.Blob, map => map.MapFrom(src => GetBlob(_PNBcontainerClient, src.GetProperty("blobId").GetString())))
                ;

            CreateMap<QueueContact, Contact>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => Guid.Parse(src.ContactKey)))
                .ForMember(dest => dest.ContactRoles, map => map.MapFrom(src => src.Contactrole.Split(new char[] { ',' }).Select(x => Guid.Parse(x))))
                .ForMember(dest => dest.FirstName, map => map.MapFrom(src => src.Forename))
                .ForMember(dest => dest.LastName, map => map.MapFrom(src => src.Surname))
                .ForMember(dest => dest.DeceasedRelationship, map => map.MapFrom(src => src.Deceasedrelathionship))
                .ForMember(dest => dest.Postcode, map => map.MapFrom(src => src.pcode))

                ;

            CreateMap<QueueSuddenDeathProperty, SuddenDeathProperty>()
                .ForMember(dest => dest.PhotoProperty, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PhotoPropertyBlobName)))
                .ForMember(dest => dest.PropertySignature, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PropertySignatureBlobName)))
                ;


            CreateMap<QueueSuddenDeath, SuddenDeath>()
                    .ForMember(dest => dest.Id, map => map.MapFrom(src => Guid.Parse(src.Id)))
                    .ForMember(dest => dest.AreaLastSeenAlive, map => map.MapFrom(src => src.WhereLastSeenAlive))
                    .ForMember(dest => dest.NextOfKinInformedMethod, map => map.MapFrom(src => src.NextOfKinWayOfInfo))
                    .ForMember(dest => dest.DeathDiagnosedBy, map => map.MapFrom(src => src.DeathDiagnosedBy))
                    //.ForMember(dest => dest.IdentificationLocation, map => map.MapFrom(src => src.))
                    .ForMember(dest => dest.UndertakerArrangingFuneral, map => map.MapFrom(src => src.UndertakerFuneral))
                    .ForMember(dest => dest.FamilyLiaisonOfficer, map => map.MapFrom(src => src.FamilyLiasionOfficer))
                    .ForMember(dest => dest.LastSeenAliveBy, map => map.MapFrom(src => src.LastSeenAliveBy))
                    .ForMember(dest => dest.NextOfKinActionToInform, map => map.MapFrom(src => src.ActionOfNextOfKin))
                    //.ForMember(dest => dest.CertifiedBy, map => map.MapFrom(src => src.cer))
                    .ForMember(dest => dest.PlaceOfDeath, map => map.MapFrom(src => src.PlaceOfDeathDesc))
                    .ForMember(dest => dest.BodyRemovedTo, map => map.MapFrom(src => src.RemovedTo))
                    .ForMember(dest => dest.BodyFoundBy, map => map.MapFrom(src => src.BodyFoundBy))
                    //.ForMember(dest => dest.BodyLabel, map => map.MapFrom(src => src.bod))
                    //.ForMember(dest => dest.SupervisorNotes, map => map.MapFrom(src => src.super))
                    .ForMember(dest => dest.MarkBruises, map => map.MapFrom(src => src.MarksBruises))
                    .ForMember(dest => dest.FormalIdentificationSteps, map => map.MapFrom(src => src.FormalIdentificationSteps))
                    .ForMember(dest => dest.BodyPhysicalPosition, map => map.MapFrom(src => src.PhysicalPosition))
                    //.ForMember(dest => dest.InquestVerdict, map => map.MapFrom(src => src.ver))
                    .ForMember(dest => dest.ClothingGeneralCondition, map => map.MapFrom(src => src.Clothing))
                    //.ForMember(dest => dest.ApprovalStatusReason, map => map.MapFrom(src => src.ap))
                    //.ForMember(dest => dest.AdditionalDetails, map => map.MapFrom(src => src.ad))
                    .ForMember(dest => dest.AdditionalNotes, map => map.MapFrom(src => src.AdditionalNotes))
                    //.ForMember(dest => dest.CauseOfDeath, map => map.MapFrom(src => src.))
                    //.ForMember(dest => dest.CoronerOfficeNotes, map => map.MapFrom(src => src.WhereLastSeenAlive))
                    .ForMember(dest => dest.Circumstances, map => map.MapFrom(src => src.Circumstances))
                    //.ForMember(dest => dest.InquestLocation, map => map.MapFrom(src => src.WhereLastSeenAlive))

                    //.ForMember(dest => dest.IdentificationSignedOn, map => map.MapFrom(src => src.dat))
                    .ForMember(dest => dest.DatetimeDeathConfirmed, map => map.MapFrom(src => DateTime.Parse(src.DateFactConfirmed + " " + src.TimeFactConfirmed)))
                    .ForMember(dest => dest.DatetimeBodyFound, map => map.MapFrom(src => DateTime.Parse(src.DateBodyFound + " " + src.TimeBodyFound)))
                    .ForMember(dest => dest.DatetimeLastSeenAlive, map => map.MapFrom(src => DateTime.Parse(src.DateLastSeenAlive + " " + src.TimeLastSeenalive)))
                    //.ForMember(dest => dest.AllPropertiesSignedOn, map => map.MapFrom(src => src.pr))
                    //.ForMember(dest => dest.InquestDate, map => map.MapFrom(src => src.WhereLastSeenAlive))

                    .ForMember(dest => dest.CIDCSIAttended, map => map.MapFrom(src => src.CIDattended))
                    .ForMember(dest => dest.NextOfKinInformed, map => map.MapFrom(src => src.NextOfKinInformed))
                    .ForMember(dest => dest.CIDCSIPhotosTaken, map => map.MapFrom(src => src.PhotosTakenbyCID))
                    //.ForMember(dest => dest.BodyIdentified, map => map.MapFrom(src => src.id))
                    .ForMember(dest => dest.FormalIdentification, map => map.MapFrom(src => src.FormalIdentification))
                    .ForMember(dest => dest.UndertakerRemovingBody, map => map.MapFrom(src => src.UndertakerRemovingBody))
                    .ForMember(dest => dest.SuspectSuicide, map => map.MapFrom(src => src.SuspectSuicide))
                    //.ForMember(dest => dest.DeathCertificateIssued, map => map.MapFrom(src => src.deathc))

//                    .ForMember(dest => dest.DeceasedAge, map => map.MapFrom(src => src.age))

                    .ForMember(dest => dest.DOLS, map => map.MapFrom(src => src.Dols))
                    .ForMember(dest => dest.SuicideNoteLeft, map => map.MapFrom(src => src.SuicideNoteLeft))
                    //.ForMember(dest => dest.IsSubmitted, map => map.MapFrom(src => src.WhereLastSeenAlive))
                    .ForMember(dest => dest.Smoker, map => map.MapFrom(src => src.DeceasedSmoker))
                    .ForMember(dest => dest.PoliceInvolvementPriorDeath, map => map.MapFrom(src => src.PoliceContactPriorToDeath))
                    .ForMember(dest => dest.DeathInHealthCare, map => map.MapFrom(src => src.DeathInHealthCare))
                    .ForMember(dest => dest.WorkRelatedDeath, map => map.MapFrom(src => src.WorkRelatedDeath))
                    .ForMember(dest => dest.DeathInCustody, map => map.MapFrom(src => src.DeathInCustody))
                    //.ForMember(dest => dest.ApprovalStatus, map => map.MapFrom(src => src.ap))
                    .ForMember(dest => dest.DeathInHospital, map => map.MapFrom(src => src.DeathInHospital))

                    .ForMember(dest => dest.HouseTemperature, map => map.MapFrom(src => src.HouseTemperature))
                    //.ForMember(dest => dest.SecondPointOfContact, map => map.MapFrom(src => src.sec))
                    //.ForMember(dest => dest.DeathType, map => map.MapFrom(src => src.deat))
                    //.ForMember(dest => dest.Spouse, map => map.MapFrom(src => src.spo))
                    .ForMember(dest => dest.BurialCremation, map => map.MapFrom(src => src.BurialOrCremation))
                    //.ForMember(dest => dest.NextOfKin, map => map.MapFrom(src => src.ne))
                    .ForMember(dest => dest.TPA, map => map.MapFrom(src => src.NPTSuddenDeath))
                    //.ForMember(dest => dest.ParentGuardian, map => map.MapFrom(src => src.pa))
                    //.ForMember(dest => dest.CertifiedRole, map => map.MapFrom(src => src.WhereLastSeenAlive))
                    //.ForMember(dest => dest.Deceased, map => map.MapFrom(src => src.WhereLastSeenAlive))
                    .ForMember(dest => dest.HouseSecure, map => map.MapFrom(src => src.SecureHouse))
                    //.ForMember(dest => dest.IdentifiedBy, map => map.MapFrom(src => src.i))
                    //.ForMember(dest => dest.InvestigationStatus       , map => map.MapFrom(src => src.WhereLastSeenAlive))

                    .ForMember(dest => dest.PhotoCircumstances, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PhotoCircumstancesBlobName)))
                    .ForMember(dest => dest.PhotoSuicideNote, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PhotoSuicideNoteBlobName)))
                    .ForMember(dest => dest.IdentificationSignature, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.identificationSignatureBlobName)))

                    .ForMember(dest => dest.CIDCSISelectedIds, map => map.MapFrom(src => GetGuids(src.CIDcsiselectid)))
                    .ForMember(dest => dest.AdditionalOfficerIds, map => map.MapFrom(src => GetGuids(src.Additionalofficerid)))


                ;







            CreateMap<JsonElement, PocketNotebook>()
                //string fields
                .ForMember(dest => dest.Notes, map => map.MapFrom(src => src.GetProperty("Notes").GetString()))
                .ForMember(dest => dest.IncidentNumber, map => map.MapFrom(src => src.GetProperty("IncidentNumber").GetString()))
                .ForMember(dest => dest.SignatoryName, map => map.MapFrom(src => src.GetProperty("SignatoryName").GetString()))
                //guid fields
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.GetProperty("Id").GetGuid()))
                //date fields
                .ForMember(dest => dest.NoteDateAndTime, map => map.MapFrom(src => src.GetProperty("NoteDateAndTime").GetDateTime()))
                .ForMember(dest => dest.IncidentDate, map => map.MapFrom(src => src.GetProperty("IncidentDateTime").ValueKind == JsonValueKind.Null ? (DateTime?)null : src.GetProperty("IncidentDateTime").GetDateTime()))
                .ForMember(dest => dest.SignatureDateandTime, map => map.MapFrom(src => src.GetProperty("Signature Date and Time").ValueKind == JsonValueKind.Null ? (DateTime?)null : src.GetProperty("Signature Date and Time").GetDateTime()))
                //blob fields
                .ForMember(dest => dest.Signature, map => map.MapFrom(src => GetBlob(_PNBcontainerClient, src.GetProperty("SignatureId").GetString())))
                .ForMember(dest => dest.Sketch, map => map.MapFrom(src => GetBlob(_PNBcontainerClient, src.GetProperty("SketchId").GetString())))
//photos
//.ForMember(dest => dest.Photos, map => map.MapFrom(src => (JsonElement)src.Single(x => x.Key == "photos").Value));

;


           // CreateMap<AppPhoto, Photo>()
               // .ForMember(dest => dest.Blob, map => map.MapFrom(src => GetBlob(src.blobId)))
          //  ;

        }


        private List<Guid> GetGuids(string GuidCSVList)
        {
            if (string.IsNullOrEmpty(GuidCSVList))
            {
                return new List<Guid>();
            }

            return GuidCSVList.Split(new char[] { ',' }).Select(x => new Guid(x)).ToList();
        }

        private string GetBlob(BlobContainerClient client, string blobId)
        {
            if (string.IsNullOrEmpty(blobId))
            {
                return null;
            }

            var blobClient = client.GetBlobClient(blobId);

            using (var memorystream = new MemoryStream())
            {
                blobClient.DownloadTo(memorystream);

                return Convert.ToBase64String(memorystream.ToArray());
            }
        }



    }
}
