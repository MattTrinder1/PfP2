using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tisski.PfP.RPRP.Plugins
{
    public class RPRPParticipantResponseAction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPParticipantResponseAction";
            const string targetEntityName = "cp_rprp";

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace($"{pluginName} Execute");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            EntityReference rprpRef = null;
            if (context.InputParameters.Contains("Target"))
            {
                rprpRef = context.InputParameters["Target"] as EntityReference;
            }
            if (rprpRef == null || rprpRef.LogicalName != targetEntityName)
            {
                throw new InvalidPluginExecutionException($"{pluginName} requires a Target Entity Reference parameter of type {targetEntityName}.");
            }

            if (!context.InputParameters.Contains("Accept") && !context.InputParameters.Contains("ReviewRequired"))
            {
                throw new InvalidPluginExecutionException($"{pluginName} requires an Accept or ReviewRequired boolean parameter.");
            }

            bool? accept = null;
            if (context.InputParameters.Contains("Accept"))
            {
                accept = (bool)context.InputParameters["Accept"];
            }
            bool? reviewRequired = null;
            if (context.InputParameters.Contains("ReviewRequired"))
            {
                reviewRequired = (bool)context.InputParameters["ReviewRequired"];
            }

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService serviceAsAdmin = serviceFactory.CreateOrganizationService(null);

            Entity retrievedRprp = serviceAsAdmin.Retrieve(rprpRef.LogicalName, rprpRef.Id, new ColumnSet("cp_participant"));
            Guid participantId = retrievedRprp.GetAttributeValue<EntityReference>("cp_participant").Id;

            if (context.InitiatingUserId != participantId)
            {
                throw new InvalidPluginExecutionException($"{pluginName} - Only the Participant can set Participant's Response.");
            }

            Entity updateRprp = new Entity(rprpRef.LogicalName, rprpRef.Id);
            if (accept != null)
            {
                updateRprp["cp_participantresponse"] = new OptionSetValue((accept.Value ? 1 : 0));
                serviceAsAdmin.Update(updateRprp);
            } 
            else if (reviewRequired != null && reviewRequired.Value == true)
            {
                updateRprp["statecode"] = new OptionSetValue(0);
                updateRprp["statuscode"] = new OptionSetValue(778230005) /* Review Required */;
                serviceAsAdmin.Update(updateRprp);
            }
        }
    }
}
