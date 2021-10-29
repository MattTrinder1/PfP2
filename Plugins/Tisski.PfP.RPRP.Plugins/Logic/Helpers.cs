using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tisski.PfP.RPRP.Plugins
{
    public static class Helpers
    {
        public static int GetEntityTypeCode(IOrganizationService adminService, string entityLogicalName)
        {
            if (adminService == null) throw new ArgumentNullException(nameof(adminService));
            if (String.IsNullOrWhiteSpace(entityLogicalName)) throw new ArgumentNullException(nameof(entityLogicalName));

            int typecode = -1;
            try
            {
                var request = new RetrieveEntityRequest()
                {
                    RetrieveAsIfPublished = true,
                    EntityFilters = EntityFilters.Entity,
                    LogicalName = entityLogicalName
                };

                var response = adminService.Execute(request) as RetrieveEntityResponse;

                typecode = response.EntityMetadata.ObjectTypeCode.Value;
            }
            catch
            {
                throw new ArgumentException($"Entity with name {entityLogicalName} does not exist.");
            }
            return typecode;
        }

        public static bool UserHasRole(IOrganizationService service, Guid userId, string roleName, bool includeTeams = true)
        {
            string userRoleFetchXml = 
                "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                    "<entity name='systemuser'>" +
                        "<filter type='and'>" +
                            $"<condition attribute='systemuserid' operator='eq' value='{userId}' />" +
                        "</filter>" +
                        "<link-entity name='systemuserroles' from='systemuserid' to='systemuserid' visible='false' intersect='true'>" +
                           "<link-entity name='role' from='roleid' to='roleid' alias='ac'>" +
                                "<filter type='and'>" +
                                    $"<condition attribute='name' operator='eq' value='{roleName}'/>" +
                                "</filter>" +
                            "</link-entity>" +
                        "</link-entity>" +
                    "</entity>" +
                "</fetch>";

            var userRoleQuery = new FetchExpression(userRoleFetchXml);
            var userRoleQueryResponse = service.RetrieveMultiple(userRoleQuery);
            bool userHasRole = (userRoleQueryResponse != null && userRoleQueryResponse.Entities != null && userRoleQueryResponse.Entities.Count > 0);

            if (!userHasRole && includeTeams)
            {
                string teamRoleFetchXml =
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "<entity name='systemuser'>" +
                            "<filter type='and'>" +
                                $"<condition attribute='systemuserid' operator='eq' value='{userId}' />" +
                            "</filter>" +
                            "<link-entity name='teammembership' from='systemuserid' to='systemuserid' visible='false' intersect='true'>" +
                                "<link-entity name='team' from='teamid' to='teamid' alias='ac'>" +
                                    "<link-entity name='teamroles' from='teamid' to='teamid' visible='false' intersect='true'>" +
                                       "<link-entity name='role' from='roleid' to='roleid' alias='ae'>" +
                                            "<filter type='and'>" +
                                                $"<condition attribute='name' operator='eq' value='{roleName}'/>" +
                                            "</filter>" +
                                        "</link-entity>" +
                                    "</link-entity>" +
                                "</link-entity>" +
                            "</link-entity>" +
                        "</entity>" +
                    "</fetch>";

                var teamRoleQuery = new FetchExpression(teamRoleFetchXml);
                var teamRoleQueryResponse = service.RetrieveMultiple(teamRoleQuery);
                userHasRole = (teamRoleQueryResponse != null && teamRoleQueryResponse.Entities != null && teamRoleQueryResponse.Entities.Count > 0);
            }

            return userHasRole;
        }

        public static Guid GetProcessStageId(IOrganizationService service, Guid processId, string stageName)
        {
            QueryExpression processStageQuery = new QueryExpression("processstage");
            processStageQuery.Criteria.AddCondition("processid", ConditionOperator.Equal, processId);
            processStageQuery.Criteria.AddCondition("stagename", ConditionOperator.Equal, stageName);

            var processStageQueryResponse = service.RetrieveMultiple(processStageQuery);
            if (processStageQueryResponse != null && processStageQueryResponse.Entities != null && processStageQueryResponse.Entities.Count > 0)
            {
                return processStageQueryResponse.Entities[0].Id;
            }
            return Guid.Empty;
        }
    }
}
