using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tisski.PfP.RPRP.Plugins
{
    public class RPRPRelationPostRetrieveMultiple : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPRelationPostRetrieveMultiple";
            const string messageName = "RetrieveMultiple";

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace($"{pluginName} Execute");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName != messageName)
                throw new InvalidPluginExecutionException($"Invalid {pluginName} invocation. Message should be {messageName} but is {context.MessageName}.");

            if (!context.OutputParameters.Contains("BusinessEntityCollection"))
                throw new InvalidPluginExecutionException($"OutputParameter BusinessEntityCollection is missing.");

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService serviceAsAdmin = serviceFactory.CreateOrganizationService(null);

            List<Entity> removeEntities = new List<Entity>();
            EntityCollection entityCollection = (EntityCollection)context.OutputParameters["BusinessEntityCollection"];
            for (int i=0; i < entityCollection.Entities.Count; i++)
            {
                Entity entity = entityCollection.Entities[i];

                string rprpRefAttribute = null;
                if (entity.LogicalName == "annotation")
                {
                    rprpRefAttribute = "objectid";
                }
                else if (entity.LogicalName == "cp_rprphistory")
                {
                    rprpRefAttribute = "cp_rprp";
                }
                else
                {
                    rprpRefAttribute = "regardingobjectid";
                }

                if (rprpRefAttribute != null)
                {
                    EntityReference rprpRef = entity.GetAttributeValue<EntityReference>(rprpRefAttribute);
                    if (rprpRef == null)
                    {
                        tracingService.Trace($"{rprpRefAttribute} not already retrieved so retrieving now.");
                        Entity retrievedEntity = serviceAsAdmin.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(rprpRefAttribute));
                        rprpRef = retrievedEntity.GetAttributeValue<EntityReference>(rprpRefAttribute);
                    }

                    if (rprpRef == null)
                    {
                        tracingService.Trace($"Include - {rprpRefAttribute} not populated on {entity.LogicalName} with ID, {entity.Id}.");
                    }
                    else if (rprpRef.LogicalName != "cp_rprp")
                    {
                        tracingService.Trace($"Include - {entity.LogicalName} with ID, {entity.Id} is not related to RPRP.");
                    }
                    else
                    {
                        tracingService.Trace($"Retrieving Participant ref from related RPRP.");
                        Entity retrievedRprp = serviceAsAdmin.Retrieve(rprpRef.LogicalName, rprpRef.Id, new ColumnSet("cp_participant"));
                        EntityReference participant = retrievedRprp.GetAttributeValue<EntityReference>("cp_participant");

                        if (context.InitiatingUserId != participant.Id)
                        {
                            tracingService.Trace($"Include - Initiating User is NOT Participant of related RPRP.");
                        }
                        else
                        {
                            tracingService.Trace($"Exclude - Initiating User is Participant of related RPRP.");
                            removeEntities.Add(entity);
                        }
                    }
                }
            }

            foreach(Entity removeEntity in removeEntities)
            {
                entityCollection.Entities.Remove(removeEntity);
            }
        }
    }
}
