using API.DataverseAccess;
using Common.Models.Dataverse;
using AutoMapper;
using Common.Models.Business;
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
                .ForMember(dest => dest.cp_identificationsignature, map => map.MapFrom(src => Convert.FromBase64String(src.IdentificationSignature)))
            ;

            CreateMap<SuddenDeathProperty, DVSuddenDeathProperty>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_propertyname, map => map.MapFrom(src => src.PropertyDescription))
                .ForMember(dest => dest.cp_actionrelatedwith, map => map.MapFrom(src => src.PersonAuthorisingProperty))
                .ForMember(dest => dest.cp_actiontaken, map => map.MapFrom(src => src.IsDisposedOrRetained))
                .ForMember(dest => dest.cp_location, map => map.MapFrom(src => src.PropertyLocation))
                .ForMember(dest => dest.cp_relationshiptodeceased, map => map.MapFrom(src => src.PropertyRelationshipToDeceased))
                .ForMember(dest => dest.cp_signedon, map => map.MapFrom(src => src.PropertyDate))
            ;

            CreateMap<SuddenDeathProperty, DVSuddenDeathPropertyImages>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_propertyphoto, map => map.MapFrom(src => Convert.FromBase64String(src.PhotoProperty)))
                .ForMember(dest => dest.cp_signature, map => map.MapFrom(src => Convert.FromBase64String(src.PropertySignature)))
            ;

            CreateMap<SuddenDeath, DVAccount>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.name, map => map.MapFrom(src => src.GPName))
                .ForMember(dest => dest.cp_surgery, map => map.MapFrom(src => src.GPSurgery))
                .ForMember(dest => dest.cp_isgp, map => map.MapFrom(src => true))
                .ForMember(dest => dest.address1_city, map => map.MapFrom(src => src.GPTown))
                .ForMember(dest => dest.address1_county, map => map.MapFrom(src => src.GPCounty))
                .ForMember(dest => dest.address1_line1, map => map.MapFrom(src => src.GPAddress))
                .ForMember(dest => dest.address1_line2, map => map.MapFrom(src => src.GPStreet))
                .ForMember(dest => dest.address1_line3, map => map.MapFrom(src => src.GPDistrict))
                .ForMember(dest => dest.address1_postalcode, map => map.MapFrom(src => src.GPPostcode))
                .ForMember(dest => dest.telephone1, map => map.MapFrom(src => src.GPPhoneNumber))

                ;

            CreateMap<SuddenDeath, DVMedicalHistory>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_diagnosismedicationprescribed, map => map.MapFrom(src => src.MedicalHistoryDiagnosisAnMedication))
                .ForMember(dest => dest.cp_gpvisitdate, map => map.MapFrom(src => src.MedicalHistoryLastVisitDate))
                .ForMember(dest => dest.cp_knownriskfactors, map => map.MapFrom(src => src.MedicalHistoryRiskFactors))
                .ForMember(dest => dest.cp_pastmedicalhistory, map => map.MapFrom(src => src.MedicalHistoryPastHistory))
                .ForMember(dest => dest.cp_reasonforvisit, map => map.MapFrom(src => src.MedicalHistoryReasonForVisit))

                ;

            CreateMap<SuddenDeath, DVLocation>()
                .IncludeBase<EntityBase, DVEntityBase>()
                .ForMember(dest => dest.cp_housename, map => map.MapFrom(src => src.HouseNameSuddenDeath))
                .ForMember(dest => dest.cp_housenumber, map => map.MapFrom(src => src.HouseNoSuddenDeath))
                .ForMember(dest => dest.cp_latitude, map => map.MapFrom(src => src.LatitudeSuddenDeath))
                .ForMember(dest => dest.cp_longitude, map => map.MapFrom(src => src.LongtitudeSuddenDeath))
                .ForMember(dest => dest.cp_street, map => map.MapFrom(src => src.AddressSuddenDeath))
                .ForMember(dest => dest.cp_district, map => map.MapFrom(src => src.DistrictSuddenDeath))
                .ForMember(dest => dest.cp_city, map => map.MapFrom(src => src.TownSuddenDeath))
                .ForMember(dest => dest.cp_country, map => map.MapFrom(src => src.CountrySuddenDeath))
                .ForMember(dest => dest.cp_county, map => map.MapFrom(src => src.CountySuddenDeath))
                .ForMember(dest => dest.cp_description, map => map.MapFrom(src => src.PlaceOfDeath))
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
