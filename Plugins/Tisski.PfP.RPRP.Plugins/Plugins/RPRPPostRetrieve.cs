using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Tisski.PfP.RPRP.Plugins
{
    public class RPRPPostRetrieve: IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPPostRetrieve";
            const string targetEntityName = "cp_rprp";
            const string messageName = "Retrieve";

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace($"{pluginName} Execute");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName != messageName)
                throw new InvalidPluginExecutionException($"Invalid {pluginName} invocation. Message should be {messageName} but is {context.MessageName}.");

            if (!context.OutputParameters.Contains("BusinessEntity"))
                throw new InvalidPluginExecutionException($"OutputParameter BusinessEntity is missing.");

            Entity entity = context.OutputParameters["BusinessEntity"] as Entity;
            if (entity == null || entity.LogicalName != targetEntityName)
                throw new InvalidPluginExecutionException($"Invalid {pluginName} invocation. BusinessEntity is not Entity of type {targetEntityName}.");

            bool applyObfuscation = true;
            if (entity.Attributes.Contains("cp_participant"))
            {
                Guid participantId = entity.GetAttributeValue<EntityReference>("cp_participant").Id;
                applyObfuscation = (context.InitiatingUserId == participantId);
            }

            if (applyObfuscation)
            {
                foreach (string obfuscatedAttributeName in RPRP.ParticipantObfuscatedAttributeNames)
                {
                    if (entity.Attributes.Contains(obfuscatedAttributeName))
                    {
                        entity[obfuscatedAttributeName] = RPRP.ObfuscatedAttributeReplacementValue;
                    }
                }
            }
        }
    }
}
