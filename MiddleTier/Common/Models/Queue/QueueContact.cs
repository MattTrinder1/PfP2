using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Models.Queue
{
    public class QueueContact
    {
        public string AdditionalNames { get; set; }
        public string Address1 { get; set; }
        public string ContactKey { get; set; }
        public string Contactrole { get; set; }
        public string Contactrolename { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        
        [JsonPropertyName("Date of Birth")]
        public string DateofBirth { get; set; }

        public string DeceasedBodyLabel { get; set; }
        public string Deceaseddetailsknown { get; set; }
        public string Deceasedrelathionship { get; set; }
        public string Deceasedrelationshipduration { get; set; }
        public string District { get; set; }
        public string Forename { get; set; }
        public string Formeroccupation{ get; set; }
        public string Fullname { get; set; }
        public string Gender { get; set; }
        public string Gendername { get; set; }
        public bool HasAdditionalNames { get; set; }
        public string Housename { get; set; }
        public string Housenumber{ get; set; }
        public string Key { get; set; }
        public string MaritalStatus { get; set; }
        public string OfficerDefinedEthnicity { get; set; }
        public string OfficerDefinedEthnicityname { get; set; }
        public string Retired { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public string Titlename { get; set; }
        public string Town { get; set; }
        public string age { get; set; }
        public string pcode { get; set; }
        public string placeofbirth { get; set; }
        public string Homephone { get; set; }
        public string Mobile { get; set; }
        public string Workphone { get; set; }
        public string EMail { get; set; }
        public string Preferedcontactmethod { get; set; }
        public string Locationofid { get; set; }
        public string signdate { get; set; }

    }
}
