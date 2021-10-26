using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

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

            if (!entity.Attributes.Contains("cp_participant"))
            {
                throw new InvalidPluginExecutionException("Participant must be set on RPRP create.");
            }

            var grantAccessRequest = new GrantAccessRequest
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess,
                    Principal = entity.GetAttributeValue<EntityReference>("cp_participant")
                },
                Target = entity.ToEntityReference()
            };
            serviceAsAdmin.Execute(grantAccessRequest);
        }
    }
}
