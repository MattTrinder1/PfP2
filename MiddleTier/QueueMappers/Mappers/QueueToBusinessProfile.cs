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
    public class NullStringConverter : ITypeConverter<string, string>
    {

        public string Convert(string source, string destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return null;
            }
            else
            {
                return source;
            }
        }
    }

    public class BoolConverter : ITypeConverter<string, bool>
    {

        public bool Convert(string source, bool destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }
            else
            {
                return source == "True";
            }
        }
    }

    public class NullDateConverter : ITypeConverter<string, DateTime?>
    {

        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source) || (DateTime.Parse(source) - new DateTime(1900,1,1)).Days == 0  )
            {
                return null;
            }
            else
            {
                return DateTime.Parse(source);
            }
        }
    }

    public class NullGuidConverter : ITypeConverter<string, Guid?>
    {

        public Guid? Convert(string source, Guid? destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source) || Guid.Parse(source) == Guid.Empty)
            {
                return null;
            }
            else
            {
                return Guid.Parse(source);
            }
        }
    }

    public class QueueToBusinessProfile : Profile
    {
        private BlobContainerClient _PNBcontainerClient;
        private BlobContainerClient _SDcontainerClient;

        public QueueToBusinessProfile(BlobContainerClientFactory containerClientFactory)
        {
            _PNBcontainerClient = containerClientFactory.GetBlobContainerClient("pnb");
            _SDcontainerClient = containerClientFactory.GetBlobContainerClient("suddendeath");

            CreateMap<string, string>().ConvertUsing<NullStringConverter>();
            CreateMap<string, Guid?>().ConvertUsing<NullGuidConverter>();
            CreateMap<string, DateTime?>().ConvertUsing<NullDateConverter>();
            CreateMap<string, bool>().ConvertUsing<BoolConverter>();

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
                .ForMember(dest => dest.Gender, map => map.MapFrom(src => src.Gender))
                .ForMember(dest => dest.OfficerDefinedEthnicity, map => map.MapFrom(src => src.OfficerDefinedEthnicity))

                ;

            CreateMap<QueueSuddenDeathProperty, SuddenDeathProperty>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.PropertyId))
                .ForMember(dest => dest.PhotoProperty, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PhotoPropertyBlobName)))
                .ForMember(dest => dest.PropertySignature, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PropertySignatureBlobName)))
                ;

            CreateMap<QueuePocketNotebook, PocketNotebook>();

            CreateMap<QueueSuddenDeath, SuddenDeath>()
                    .ForMember(dest => dest.Id, map => map.MapFrom(src => Guid.Parse(src.Id)))
                    .ForMember(dest => dest.AreaLastSeenAlive, map => map.MapFrom(src => src.WhereLastSeenAlive))
                    .ForMember(dest => dest.NextOfKinInformedMethod, map => map.MapFrom(src => src.NextOfKinWayOfInfo))
                    .ForMember(dest => dest.DeathDiagnosedBy, map => map.MapFrom(src => src.DeathDiagnosedBy))
                    .ForMember(dest => dest.UndertakerArrangingFuneral, map => map.MapFrom(src => src.UndertakerFuneral))
                    .ForMember(dest => dest.FamilyLiaisonOfficer, map => map.MapFrom(src => src.FamilyLiasionOfficer))
                    .ForMember(dest => dest.LastSeenAliveBy, map => map.MapFrom(src => src.LastSeenAliveBy))
                    .ForMember(dest => dest.NextOfKinActionToInform, map => map.MapFrom(src => src.ActionOfNextOfKin))
                    .ForMember(dest => dest.PlaceOfDeath, map => map.MapFrom(src => src.PlaceOfDeathDesc))
                    .ForMember(dest => dest.BodyRemovedTo, map => map.MapFrom(src => src.RemovedTo))
                    .ForMember(dest => dest.BodyFoundBy, map => map.MapFrom(src => src.BodyFoundBy))
                    .ForMember(dest => dest.MarkBruises, map => map.MapFrom(src => src.MarksBruises))
                    .ForMember(dest => dest.FormalIdentificationSteps, map => map.MapFrom(src => src.FormalIdentificationSteps))
                    .ForMember(dest => dest.BodyPhysicalPosition, map => map.MapFrom(src => src.PhysicalPosition))
                    .ForMember(dest => dest.ClothingGeneralCondition, map => map.MapFrom(src => src.Clothing))
                    .ForMember(dest => dest.AdditionalNotes, map => map.MapFrom(src => src.AdditionalNotes))
                    .ForMember(dest => dest.Circumstances, map => map.MapFrom(src => src.Circumstances))
                    .ForMember(dest => dest.DatetimeDeathConfirmed, map => map.MapFrom(src => src.DateFactConfirmed ))
                    .ForMember(dest => dest.DatetimeBodyFound, map => map.MapFrom(src => src.DateBodyFound))
                    .ForMember(dest => dest.DatetimeLastSeenAlive, map => map.MapFrom(src => src.DateLastSeenAlive ))
                    .ForMember(dest => dest.CIDCSIAttended, map => map.MapFrom(src => src.CIDattended))
                    .ForMember(dest => dest.NextOfKinInformed, map => map.MapFrom(src => src.NextOfKinInformed))
                    .ForMember(dest => dest.CIDCSIPhotosTaken, map => map.MapFrom(src => src.PhotosTakenbyCID))
                    .ForMember(dest => dest.FormalIdentification, map => map.MapFrom(src => src.FormalIdentification))
                    .ForMember(dest => dest.UndertakerRemovingBody, map => map.MapFrom(src => src.UndertakerRemovingBody))
                    .ForMember(dest => dest.SuspectSuicide, map => map.MapFrom(src => src.SuspectSuicide))
                    .ForMember(dest => dest.DOLS, map => map.MapFrom(src => src.Dols))
                    .ForMember(dest => dest.SuicideNoteLeft, map => map.MapFrom(src => src.SuicideNoteLeft))
                    .ForMember(dest => dest.Smoker, map => map.MapFrom(src => src.DeceasedSmoker))
                    .ForMember(dest => dest.PoliceInvolvementPriorDeath, map => map.MapFrom(src => src.PoliceContactPriorToDeath))
                    .ForMember(dest => dest.DeathInHealthCare, map => map.MapFrom(src => src.DeathInHealthCare))
                    .ForMember(dest => dest.WorkRelatedDeath, map => map.MapFrom(src => src.WorkRelatedDeath))
                    .ForMember(dest => dest.DeathInCustody, map => map.MapFrom(src => src.DeathInCustody))
                    .ForMember(dest => dest.DeathInHospital, map => map.MapFrom(src => src.DeathInHospital))
                    .ForMember(dest => dest.HouseTemperature, map => map.MapFrom(src => GetGuid(src.HouseTemperature)))
                    .ForMember(dest => dest.BurialCremation, map => map.MapFrom(src => src.BurialOrCremation))
                    .ForMember(dest => dest.TPA, map => map.MapFrom(src => GetGuid(src.NPTSuddenDeath)))
                    .ForMember(dest => dest.HouseSecure, map => map.MapFrom(src => GetGuid(src.SecureHouse)))
                    .ForMember(dest => dest.PhotoCircumstances, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PhotoCircumstancesBlobName)))
                    .ForMember(dest => dest.PhotoSuicideNote, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.PhotoSuicideNoteBlobName)))
                    .ForMember(dest => dest.IdentificationSignature, map => map.MapFrom(src => GetBlob(_SDcontainerClient, src.identificationSignatureBlobName)))
                    .ForMember(dest => dest.CIDCSISelectedIds, map => map.MapFrom(src => GetGuids(src.CIDcsiselectid)))
                    .ForMember(dest => dest.AdditionalOfficerIds, map => map.MapFrom(src => GetGuids(src.Additionalofficerid)))
                    .ForMember(dest => dest.BurialCremation, map => map.MapFrom(src => GetGuid(src.BurialOrCremation)))
                    .ForMember(dest => dest.LatitudeSuddenDeath, map => map.MapFrom(src => GetNullableDouble(src.LatitudeSuddenDeath)))
                    .ForMember(dest => dest.LongtitudeSuddenDeath, map => map.MapFrom(src => GetNullableDouble(src.LongtitudeSuddenDeath)))




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

        private DateTime? GetDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            return DateTime.Parse(dateString);
        }

        private List<Guid> GetGuids(string GuidCSVList)
        {
            if (string.IsNullOrEmpty(GuidCSVList))
            {
                return new List<Guid>();
            }

            return GuidCSVList.Split(new char[] { ',' }).Select(x => new Guid(x)).ToList();
        }

        private Guid? GetGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid) || Guid.Parse(guid) == Guid.Empty)
            {
                return null;
            }

            return Guid.Parse(guid);
        }

        private double? GetNullableDouble(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return double.Parse(value);
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
