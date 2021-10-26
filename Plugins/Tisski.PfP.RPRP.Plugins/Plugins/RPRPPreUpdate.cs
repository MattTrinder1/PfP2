using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tisski.PfP.RPRP.Plugins
{
    public class RPRPPreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPPreUpdate";
            const string targetEntityName = "cp_rprp";
            const string messageName = "Update";

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

            Entity retrievedRprp = serviceAsAdmin.Retrieve(entity.LogicalName, entity.Id, new ColumnSet("cp_participant"));
            Guid participantId = retrievedRprp.GetAttributeValue<EntityReference>("cp_participant").Id;

            if (context.InitiatingUserId == participantId)
            {
                string notAllowed = "";
                foreach (var attrib in entity.Attributes)
                {
                    if (attrib.Key != "cp_participantresponse")
                    {
                        notAllowed += $"{attrib.Key},";
                    }
                    if (notAllowed != "")
                    {
                        notAllowed = notAllowed.TrimEnd(',');
                        throw new InvalidPluginExecutionException($"Participant can only update Participant's Response. (Cannot update: {notAllowed})");
                    }
                }
            }
            else if (entity.Attributes.Contains("cp_participantresponse"))
            {
                if (context.ParentContext == null || context.ParentContext.MessageName != "cp_RPRPSetParticipantResponse")
                {
                    throw new InvalidPluginExecutionException($"{pluginName} - Only the Participant can set Participant's Response.");
                }
            }
        }
    }
}
