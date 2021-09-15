using Common.Models.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using MoD.CAMS.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITests
{
    [TestClass]

    public abstract class TestBase
    {
        protected void ValidateIncident(EntityBase original, Entity incident,string incidentTypeName,string userEmail)
        {
            Assert.AreEqual(original.IncidentNumber, incident.GetValue<string>("cp_incidentnumber"));
            Assert.IsTrue(DateTimesMatch(original.IncidentDate, incident.GetValue<DateTime>("cp_incidentdate")));

            //check incident type
            var incidentType = DynamicsServiceHelper.GetEntity(StartUp.adminService, "cp_incidenttype", "cp_incidenttypename", incidentTypeName);
            Assert.AreEqual(incidentType.Id, incident.GetEntityReferenceValue("cp_incidenttype").Id);

            var userId = StartUp.adminService.GetUserId(userEmail);
            Assert.AreEqual(userId, incident.GetEntityReferenceValue("cp_enteredby").Id);
            Assert.AreEqual(userId, incident.GetEntityReferenceValue("ownerid").Id);
        }


        protected bool DateTimesMatch(DateTime? expected, DateTime? actual)
        {

            if (!expected.HasValue && !actual.HasValue)
            {
                return true;
            }
            if (expected.HasValue && !actual.HasValue)
            {
                return false;
            }
            if (!expected.HasValue && actual.HasValue)
            {
                return false;
            }


            return (expected.Value - actual.Value.ToLocalTime()).TotalSeconds < 1;
        }

    }
}
