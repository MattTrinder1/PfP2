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
    public class RPRPPostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPPostCreate";
            const string targetEntityName = "cp_rprp";
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

            //Share RPRP with Participant with Read access.
            if (!entity.Attributes.Contains("cp_participant"))
            {
                throw new InvalidPluginExecutionException("Participant must be set on create of RPRP.");
            }

            var participantGrantAccessRequest = new GrantAccessRequest
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess,
                    Principal = entity.GetAttributeValue<EntityReference>("cp_participant")
                },
                Target = entity.ToEntityReference()
            };
            serviceAsAdmin.Execute(participantGrantAccessRequest);

            if (entity.Attributes.Contains("cp_reviewer"))
            {
                EntityReference reviewerRef = entity.GetAttributeValue<EntityReference>("cp_reviewer");
                if (context.InitiatingUserId != reviewerRef.Id)
                {
                    //Share RPRP with Reviewer with Read & Write access.
                    var reviewerGrantAccessRequest = new GrantAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess,
                            Principal = reviewerRef
                        },
                        Target = entity.ToEntityReference()
                    };
                    serviceAsAdmin.Execute(reviewerGrantAccessRequest);
                }
            }

            EntityReference typeRef = entity.GetAttributeValue<EntityReference>("cp_type");
            tracingService.Trace($"Retrieving RPRP Type");
            Entity retrievedType = serviceAsAdmin.Retrieve(typeRef.LogicalName, typeRef.Id, new ColumnSet("cp_owningteam"));
            if (retrievedType.Attributes.Contains("cp_owningteam"))
            {
                EntityReference team = retrievedType.GetAttributeValue<EntityReference>("cp_owningteam");
                AssignRequest assignRequest = new AssignRequest()
                {
                    Assignee = team,
                    Target = entity.ToEntityReference()
                };
                tracingService.Trace($"Assigning to Team with id, {team.Id}");
                serviceAsAdmin.Execute(assignRequest);
            }
        }
    }
}
