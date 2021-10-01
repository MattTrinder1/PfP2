using System;
using System.Collections.Generic;

namespace Common.Models.Business
{
    public class Contact : IncidentRelatedEntityBase
    {
        public List<Guid> ContactRoles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string HouseName { get; set; }
        public string HouseNumber { get; set; }
        public string Address1 { get; set; }
        public string District { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string MaritalStatus { get; set; }
        public string Retired { get; set; }
        public string FormerOccupation { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }

        public Guid? Title { get; set; }
        public Guid? Gender { get; set; }
        public Guid? OfficerDefinedEthnicity { get; set; }

        public string AdditionalNames { get; set; }
        
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public Guid? Preferedcontactmethod { get; set; }

        public string DeceasedRelationship { get; set; }
        public string DeceasedRelationshipDuration { get; set; }

        public DateTime?  SignDate { get; set; }
        public string LocationOfId { get; set; }
    }
}
