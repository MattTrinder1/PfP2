using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tisski.PfP.RPRP.Plugins
{
    public class RPRPSnapshotPostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPSnapshotPostCreate";
            const string targetEntityName = "cp_rprphistory";
            const string messageName = "Create";

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace($"{pluginName} Execute");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName != messageName)
                throw new InvalidPluginExecutionException($"Invalid {pluginName} invocation. Message should be {messageName} but is {context.MessageName}.");

            if (!context.InputParameters.Contains("Target"))
                throw new InvalidPluginExecutionException($"Invalid {pluginName} invocation. Target is missing.");

            Entity entity = context.InputParameters["Target"] as Entity;
            if (entity == null || entity.LogicalName != targetEntityName)
                throw new InvalidPluginExecutionException($"Invalid {pluginName} invocation. Target is not Entity of type {targetEntityName}.");

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService serviceAsAdmin = serviceFactory.CreateOrganizationService(null);

            if (!entity.Attributes.Contains("cp_rprp"))
            {
                throw new InvalidPluginExecutionException("RPRP must be set on create of RPRP Snapshot.");
            }

            EntityReference rprpRef = entity.GetAttributeValue<EntityReference>("cp_rprp");
            tracingService.Trace("Retrieving RPRP");
            Entity retrievedRprp = serviceAsAdmin.Retrieve(rprpRef.LogicalName, rprpRef.Id, new ColumnSet("cp_reviewer", "cp_participant"));

            if (retrievedRprp.Attributes.Contains("cp_reviewer"))
            {
                tracingService.Trace("Sharing RPRP Snapshot with Reviewer");
                //Share RPRP Snapshot with Reviewer with Read access.
                var reviewerGrantAccessRequest = new GrantAccessRequest
                {
                    PrincipalAccess = new PrincipalAccess
                    {
                        AccessMask = AccessRights.ReadAccess,
                        Principal = retrievedRprp.GetAttributeValue<EntityReference>("cp_reviewer")
                    },
                    Target = entity.ToEntityReference()
                };
                serviceAsAdmin.Execute(reviewerGrantAccessRequest);
            }

            bool shareSnapshotWithParticipant = false; //FUTURE TODO: Allow Type configuration to determine this.
            if (shareSnapshotWithParticipant && retrievedRprp.Attributes.Contains("cp_participant"))
            {
                tracingService.Trace("Sharing RPRP Snapshot with Participant");
                var participantGrantAccessRequest = new GrantAccessRequest
                {
                    PrincipalAccess = new PrincipalAccess
                    {
                        AccessMask = AccessRights.ReadAccess,
                        Principal = retrievedRprp.GetAttributeValue<EntityReference>("cp_participant")
                    },
                    Target = entity.ToEntityReference()
                };
                serviceAsAdmin.Execute(participantGrantAccessRequest);
            }
        }
    }
}
