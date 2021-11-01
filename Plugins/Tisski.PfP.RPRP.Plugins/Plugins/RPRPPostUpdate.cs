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
    public class RPRPPostUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "RPRPPostUpdate";
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

            tracingService.Trace($"Attributes in target: {Helpers.GetAttributeNamesCsv(entity)}");

            //Handle Reviewer access.
            if (entity.Attributes.Contains("cp_reviewer"))
            {
                //Reviewer is being updated.
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

            //Get the BPF instance associated to this RPRP.
            QueryExpression rprpBpfQuery = new QueryExpression("cp_rprpbpf");
            rprpBpfQuery.Criteria.AddCondition("bpf_cp_rprpid", ConditionOperator.Equal, entity.Id);
            rprpBpfQuery.ColumnSet = new ColumnSet("activestageid", "processid");

            var rprpBpfQueryResponse = serviceAsAdmin.RetrieveMultiple(rprpBpfQuery);

            if (rprpBpfQueryResponse != null && rprpBpfQueryResponse.Entities != null && rprpBpfQueryResponse.Entities.Count > 0)
            {
                Entity rprpBpf = rprpBpfQueryResponse.Entities[0];
                tracingService.Trace($"processid = {rprpBpf.GetAttributeValue<EntityReference>("processid").Id}");

                //Get stage IDs for RPRP BPF.
                const string rprpBpfIdString = "EB1F4A29-A838-EC11-8C64-000D3A871D8B";
                Guid rprpBpfId = Guid.Parse(rprpBpfIdString);

                Guid initiateStageId = Helpers.GetProcessStageId(serviceAsAdmin, rprpBpfId, "Initiate");
                tracingService.Trace($"Initiate stageid = {initiateStageId}");

                Guid reviewStageId = Helpers.GetProcessStageId(serviceAsAdmin, rprpBpfId, "Review");
                tracingService.Trace($"Review stageid = {reviewStageId}");

                Guid actionStageId = Helpers.GetProcessStageId(serviceAsAdmin, rprpBpfId, "Action");
                tracingService.Trace($"Action stageid = {actionStageId}");

                Guid completeStageId = Helpers.GetProcessStageId(serviceAsAdmin, rprpBpfId, "Complete");
                tracingService.Trace($"Complete stageid = {completeStageId}");

                Guid activeStageId = rprpBpf.GetAttributeValue<EntityReference>("activestageid").Id;
                tracingService.Trace($"activestageid = {activeStageId}");

                Entity retrievedRprp = serviceAsAdmin.Retrieve(
                    entity.LogicalName,
                    entity.Id,
                    new ColumnSet(
                        "cp_nextstepsfollowup",
                        "cp_participantaccepted",
                        "cp_participantnotifiedon",
                        "cp_participantrespondedon",
                        "cp_reflectivereviewendedon",
                        "cp_reflectivereviewscheduledstart",
                        "cp_reviewer",
                        "cp_reviewercompletedon",
                        "cp_completionapprovedby")
                    );

                Guid nextStageId = Guid.Empty;
                if (activeStageId == initiateStageId)
                {
                    if (retrievedRprp.Attributes.Contains("cp_reviewer") &&
                        retrievedRprp.Attributes.Contains("cp_participantnotifiedon") &&
                        retrievedRprp.Attributes.Contains("cp_reflectivereviewscheduledstart"))
                    {
                        nextStageId = reviewStageId;
                    }
                }
                else if (activeStageId == reviewStageId)
                {
                    if (retrievedRprp.Attributes.Contains("cp_reflectivereviewendedon") &&
                        retrievedRprp.Attributes.Contains("cp_participantaccepted") &&
                        retrievedRprp.GetAttributeValue<bool>("cp_participantaccepted"))
                    {
                        if (retrievedRprp.Attributes.Contains("cp_nextstepsfollowup") &&
                            retrievedRprp.GetAttributeValue<bool>("cp_nextstepsfollowup"))
                        {
                            nextStageId = actionStageId;
                        }
                        else
                        {
                            nextStageId = completeStageId;
                        }
                    }
                }
                else if (activeStageId == actionStageId)
                {
                    if (retrievedRprp.Attributes.Contains("cp_reviewercompletedon"))
                    {
                        nextStageId = completeStageId;
                    }
                }

                if (nextStageId != Guid.Empty)
                {
                    //Attempt to move the BPF forward.
                    try
                    {
                        Entity updateRprpBpf = new Entity(rprpBpf.LogicalName, rprpBpf.Id);
                        updateRprpBpf["activestageid"] = new EntityReference("processstage", nextStageId);
                        serviceAsAdmin.Update(updateRprpBpf);

                        tracingService.Trace($"Moved BPF to next stage.");
                    }
                    catch (Exception e)
                    {
                        tracingService.Trace($"Failed to move BPF to next stage: {e.Message}");
                    }
                }

                if ((activeStageId == completeStageId || nextStageId == completeStageId) &&
                    retrievedRprp.Attributes.Contains("cp_completionapprovedby"))
                {
                    //Attempt to finish BPF.
                    try
                    {
                        Entity updateRprpBpf = new Entity(rprpBpf.LogicalName, rprpBpf.Id);
                        updateRprpBpf["statecode"] = new OptionSetValue(1);
                        updateRprpBpf["statuscode"] = new OptionSetValue(2 /* Finished */);
                        serviceAsAdmin.Update(updateRprpBpf);

                        tracingService.Trace($"Set BPF Finished.");
                    }
                    catch (Exception e)
                    {
                        tracingService.Trace($"Failed to move set BPF to Finished: {e.Message}");
                    }
                }
            }
            else
            {
                tracingService.Trace("Failed to retrieve BPF instance.");
            }
        }
    }
}