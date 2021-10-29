﻿using System;
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

            Entity retrievedRprp = serviceAsAdmin.Retrieve(entity.LogicalName, entity.Id, new ColumnSet("cp_participant", "cp_reviewer", "cp_type"));
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

            if (entity.Attributes.Contains("statecode"))
            {
                //Only "PfP RPRP All" role can deactivate (/complete/cancel) an RPRP unless the 
                //"RPRP Type" has "Reviewer Can Complete" set to true and the deactivate is 
                //initiated by the Reviewer.
                //(Participant can never deactivate even if having "PfP RPRP All" role but this 
                // is already handled in the Participant update validation above.)
                bool allowedStatusChange = false;
                if (Helpers.UserHasRole(serviceAsAdmin, context.InitiatingUserId, "PfP RPRP All", true))
                {
                    allowedStatusChange = true;
                }
                else if (retrievedRprp.Attributes.Contains("cp_reviewer") && 
                    context.InitiatingUserId == retrievedRprp.GetAttributeValue<EntityReference>("cp_reviewer").Id)
                {
                    EntityReference typeRef = retrievedRprp.GetAttributeValue<EntityReference>("cp_type");
                    Entity retrievedType = serviceAsAdmin.Retrieve(typeRef.LogicalName, typeRef.Id, new ColumnSet("cp_reviewercancomplete"));
                    if (retrievedType.Attributes.Contains("cp_reviewercancomplete") && 
                        retrievedType.GetAttributeValue<bool>("cp_reviewercancomplete"))
                    {
                        allowedStatusChange = true;
                    }
                    else if (entity.GetAttributeValue<OptionSetValue>("statuscode").Value == 778230009 /* Completed */)
                    {
                        //Switch the Reviewer's Complete attempt to Awaiting Approval status.
                        entity["statecode"] = new OptionSetValue(0);
                        entity["statuscode"] = new OptionSetValue(778230008 /* Awaiting Approval */);
                        allowedStatusChange = true;
                    }
                }

                if (allowedStatusChange)
                {
                    if (entity.Attributes.Contains("statuscode") && 
                            entity.GetAttributeValue<OptionSetValue>("statuscode").Value == 778230009 /* Completed */)
                    {
                        //Allowing final complete so add Completion Approved By to update.
                        entity["cp_completionapprovedby"] = new EntityReference("systemuser", context.InitiatingUserId);
                    }
                }
                else
                {
                    throw new InvalidPluginExecutionException("You are not allowed to deactivate this RPRP.");
                }
            }

            if (retrievedRprp.Attributes.Contains("cp_reviewer") &&
                    context.InitiatingUserId == retrievedRprp.GetAttributeValue<EntityReference>("cp_reviewer").Id &&
                    entity.Attributes.Contains("statuscode") &&
                        (entity.GetAttributeValue<OptionSetValue>("statuscode").Value == 778230009 /* Completed */ ||
                        entity.GetAttributeValue<OptionSetValue>("statuscode").Value == 778230008 /* Awaiting Approval */))
            {
                //Add Reviewer Completed On to update.
                entity["cp_reviewercompletedon"] = DateTime.Now.ToUniversalTime();
            }
        }
    }
}
