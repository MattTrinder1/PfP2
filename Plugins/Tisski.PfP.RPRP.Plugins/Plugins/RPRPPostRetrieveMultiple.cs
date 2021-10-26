using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Tisski.PfP.RPRP.Plugins
{
    public class RPRPPostRetrieveMultiple : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPPostRetrieveMultiple";
            const string targetEntityName = "cp_rprp";
            const string messageName = "RetrieveMultiple";

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace($"{pluginName} Execute");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName != messageName)
                throw new InvalidPluginExecutionException($"Invalid {pluginName} invocation. Message should be {messageName} but is {context.MessageName}.");

            if (!context.OutputParameters.Contains("BusinessEntityCollection"))
                throw new InvalidPluginExecutionException($"OutputParameter BusinessEntityCollection is missing.");

            EntityCollection entityCollection = (EntityCollection)context.OutputParameters["BusinessEntityCollection"];

            foreach (Entity entity in entityCollection.Entities)
            {
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
}
