using API.DataverseAccess;
using API.Models.Dataverse;
using API.Models.PNB;
using AutoMapper;
using Common.Models.Business;
using Common.Models.Dataverse;
using Microsoft.Xrm.Sdk;
using System;

namespace API.Mappers
{
    public class BusinessToDataverseProfile : Profile
    {
        enum BooleanAliases
        {
            Yes = 1,
            No = 0
        }

        public BusinessToDataverseProfile(DVDataAccess adminDataAccess)
        {
            RecognizeDestinationPrefixes("cp_");
            RecognizeDestinationPrefixes("address1_");

            CreateMap<EntityBase, DVEntityBase>()
            ;


            CreateMap<PocketNotebook, DVPocketNotebookImages>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_sketch, map => map.MapFrom(src => Convert.FromBase64String(src.Sketch)))
                .ForMember(dest => dest.cp_signature, map => map.MapFrom(src => Convert.FromBase64String(src.Signature)))
            ;


            CreateMap<PocketNotebook, DVPocketNotebook>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_pocketnotebookid, map => map.MapFrom(src => src.Id))
            ;

            CreateMap<Contact, DVContact>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.birthdate, map => map.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.address1_city, map => map.MapFrom(src => src.Town))
                .ForMember(dest => dest.address1_line1, map => map.MapFrom(src => src.Address1))
                .ForMember(dest => dest.address1_postalcode, map => map.MapFrom(src => src.Postcode))
                .ForMember(dest => dest.cp_contacttitle, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.Title)))
                .ForMember(dest => dest.cp_gender, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.Gender)))
                .ForMember(dest => dest.cp_officerdefinedethnicity, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.OfficerDefinedEthnicity)))
                .ForMember(dest => dest.cp_contactpreferredmethodofcontact, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.Preferedcontactmethod)))
                .ForMember(dest => dest.address1_telephone1, map => map.MapFrom(src => src.HomePhone))
                .ForMember(dest => dest.mobilephone, map => map.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.company, map => map.MapFrom(src => src.WorkPhone))
                .ForMember(dest => dest.emailaddress1, map => map.MapFrom(src => src.Email))
                .ForMember(dest => dest.cp_additionalname, map => map.MapFrom(src => src.AdditionalNames))
                .ForMember(dest => dest.cp_birthplace, map => map.MapFrom(src => src.PlaceOfBirth))
                .ForMember(dest => dest.familystatuscode, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("familystatuscode", src.MaritalStatus,"contact")))
                .ForMember(dest => dest.cp_retired, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_yes_no_na_list", src.Retired)))
                .ForMember(dest => dest.cp_otheroccupation, map => map.MapFrom(src => src.FormerOccupation))

            ;


            CreateMap<SuddenDeath, DVSuddenDeathImages>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_suicidenotepicture, map => map.MapFrom(src => Convert.FromBase64String(src.PhotoSuicideNote)))
            ;


            CreateMap<SuddenDeath, DVSuddenDeath>()
                .IncludeBase<EntityBase, DVEntityBase>()
                //string exceptions
                .ForMember(dest => dest.cp_sdfamilylaisonofficer, map => map.MapFrom(src => src.FamilyLiaisonOfficer))
                .ForMember(dest => dest.cp_sdlastseenaliveby, map => map.MapFrom(src => src.LastSeenAliveBy))

                //booleans
                .ForMember(dest => dest.cp_cidcsiattended, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.CIDCSIAttended))))
                .ForMember(dest => dest.cp_cidcsiphotostaken, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.CIDCSIPhotosTaken))))
                .ForMember(dest => dest.cp_nextofkininformed, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.NextOfKinInformed))))
                .ForMember(dest => dest.cp_bodyidentified, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.BodyIdentified))))
                .ForMember(dest => dest.cp_formalidentification, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.FormalIdentification))))
                .ForMember(dest => dest.cp_undertakerremovebody, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.UndertakerRemovingBody))))
                .ForMember(dest => dest.cp_suspectsuicide, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.SuspectSuicide))))
                .ForMember(dest => dest.cp_deathcertificateissued, map => map.MapFrom(src => Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), src.DeathCertificateIssued))))
                
                //option sets
                .ForMember(dest => dest.cp_dols, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.DOLS)))
                .ForMember(dest => dest.cp_suicidenoteleft_new, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.SuicideNoteLeft)))
                .ForMember(dest => dest.cp_issubmitted, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_yes_no_na_list", src.IsSubmitted)))
                .ForMember(dest => dest.cp_smoker, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.Smoker)))
                .ForMember(dest => dest.cp_policeinvolvementpriordeath, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.PoliceInvolvementPriorDeath)))
                .ForMember(dest => dest.cp_deathinhealthcare, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.DeathInHealthCare)))
                .ForMember(dest => dest.cp_workrelateddeath, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.WorkRelatedDeath)))
                .ForMember(dest => dest.cp_deathincustody, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.DeathInCustody)))
                .ForMember(dest => dest.cp_approvalstatus, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_approvalstatus", src.ApprovalStatus)))
                .ForMember(dest => dest.cp_deathinhospital, map => map.MapFrom(src => adminDataAccess.GetOptionSetValue("cp_subjectofdols", src.DeathInHospital)))
                
                //lookups
                .ForMember(dest => dest.cp_housetemperature, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.HouseTemperature)))
                .ForMember(dest => dest.cp_secondpointofcontact, map => map.MapFrom(src => GetEntityReference("contact", src.SecondPointOfContact)))
                .ForMember(dest => dest.cp_deathtype, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.DeathType)))
                .ForMember(dest => dest.cp_burialcremation, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.BurialCremation)))
                .ForMember(dest => dest.cp_tpa, map => map.MapFrom(src => GetEntityReference("cp_territorialpolicingarea", src.TPA)))
                .ForMember(dest => dest.cp_certifiedrole, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.CertifiedRole)))
                .ForMember(dest => dest.cp_housesecure, map => map.MapFrom(src => GetEntityReference("cp_lookupvalue", src.HouseSecure)))
                .ForMember(dest => dest.cp_investigationstatus, map => map.MapFrom(src => GetEntityReference("cp_investigationstatus", src.InvestigationStatus)))
            ;

            CreateMap<Photo, DVPhoto>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_phototitle, map => map.MapFrom(src => src.Caption))
                .ForMember(dest => dest.cp_pocketnotebook, map => map.MapFrom(src => GetEntityReference("cp_pocketnotebook", src.PocketNotebookId)))
                .ForMember(dest => dest.cp_suddendeath, map => map.MapFrom(src => GetEntityReference("cp_suddendeath", src.SuddenDeathId)))
                .ForMember(dest => dest.cp_image, map => map.MapFrom(src => Convert.FromBase64String(src.Blob)))
            ;
        }

        private EntityReference GetEntityReference(string entityName, Guid? id)
        {
            if (id.HasValue)
            {
                return new EntityReference(entityName, id.Value);
            }
            else
            {
                return null;
            }
        }

    }
}
