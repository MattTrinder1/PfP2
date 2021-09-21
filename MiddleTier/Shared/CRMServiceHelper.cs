namespace MoD.CAMS.Plugins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.IO;
    using System.Runtime.Serialization;

    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Metadata;
    using System.Xml;
    using Microsoft.Xrm.Sdk.Metadata.Query;
    using System.ServiceModel;
    using System.Threading;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text.RegularExpressions;


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal static class DynamicsServiceHelper
    {
        #region constants

        private const string ROLENAME = "NonExecutionRole";

        #endregion

        public static Guid GetUserId(this IOrganizationService service,string emailAddress)
        {
            Guid userId;
                QueryExpression query = new QueryExpression("systemuser");
                query.Criteria.AddCondition("internalemailaddress", ConditionOperator.Equal, emailAddress);
                query.ColumnSet = new ColumnSet("systemuserid");

                var dvResponse = service.RetrieveMultiple(query);

                userId = dvResponse.Entities.Single().GetAttributeValue<Guid>("systemuserid");


            return userId;
        }


        internal static void Assign(this IOrganizationService service, EntityReference target, EntityReference assignee)
        {
            var req = new AssignRequest();
            req.Assignee = assignee;
            req.Target = target;
            service.Execute(req);
        }

        internal static void Assign(this IOrganizationService service, Entity target, Entity assignee)
        {
            Assign(service, target.ToEntityReference(), assignee.ToEntityReference());
        }

        internal static void Assign(this IOrganizationService service, EntityReference target, Entity assignee)
        {
            Assign(service, target, assignee.ToEntityReference());
        }

        internal static void Assign(this IOrganizationService service, Entity target, EntityReference assignee)
        {
            Assign(service, target.ToEntityReference(), assignee);
        }

        internal static void RevokeSharedAccess(this IOrganizationService service, EntityReference target, EntityReference revokee)
        {
            var revoke = new RevokeAccessRequest();
            revoke.Target = target;
            revoke.Revokee = revokee ;
            service.Execute(revoke);
        }

        internal static T GetValue<T>(this Entity entity, string attributeName)
        {
            //get the attribute

            if (entity.Attributes.ContainsKey(attributeName))
            {
                var attr = entity[attributeName];
                if (attr is AliasedValue)
                {
                    return GetAliasedValueValue<T>(entity, attributeName);
                }
                else
                {
                    return entity.GetAttributeValue<T>(attributeName);
                }
            }
            else
            {
                return default(T);
            }

        }

        internal static string GetStringValue(this Entity entity, string attributeName)
        {
            //get the attribute

            if (entity.Attributes.ContainsKey(attributeName))
            {
                var attr = entity[attributeName];
                if (attr is AliasedValue)
                {
                    return GetAliasedValueValue<string>(entity, attributeName);
                }
                else
                {
                    return entity.GetAttributeValue<string>(attributeName);
                }
            }
            else
            {
                return default(string);
            }

        }

        internal static EntityReference GetEntityReferenceValue(this Entity entity, string attributeName)
        {
            //get the attribute

            if (entity.Attributes.ContainsKey(attributeName))
            {
                var attr = entity[attributeName];
                if (attr is AliasedValue)
                {
                    return GetAliasedValueValue<EntityReference>(entity, attributeName);
                }
                else
                {
                    return entity.GetAttributeValue<EntityReference>(attributeName);
                }
            }
            else
            {
                return default(EntityReference);
            }

        }
        internal static OptionSetValue GetOptionSetValue(this Entity entity, string attributeName)
        {
            //get the attribute

            if (entity.Attributes.ContainsKey(attributeName))
            {
                var attr = entity[attributeName];
                if (attr is AliasedValue)
                {
                    return GetAliasedValueValue<OptionSetValue>(entity, attributeName);
                }
                else
                {
                    return entity.GetAttributeValue<OptionSetValue>(attributeName);
                }
            }
            else
            {
                return default(OptionSetValue);
            }

        }


        internal static decimal SumMoneyField(this IEnumerable<Entity> entities, string fieldName)
        {
            return entities.Where(x => x.Attributes.ContainsKey(fieldName))
                           .Sum(x => x.GetValue<Money>(fieldName).Value);
        }

        internal static T GetAliasedValueValue<T>(this Entity entity, string attributeName)
        {
            var attr = entity.GetAttributeValue<AliasedValue>(attributeName);
            if (attr != null)
            {
                return (T)attr.Value;
            }
            else
            {
                return default(T);
            }

        }

        internal static Entity CloneEntity(Entity entityToClone)
        {
            //does not work in sandbox - use cloneentitysandbox instead
            var earlyBoundSerializer = new DataContractSerializer(typeof(Entity));
            var newEntity = new Entity();
            using (var stream = new MemoryStream())
            {
                // Write the XML to disk.B
                earlyBoundSerializer.WriteObject(stream, entityToClone);
                stream.Position = 0;

                newEntity = (Entity)earlyBoundSerializer.ReadObject(stream);

            }

            return newEntity;

        }

        internal static Entity CloneEntitySandbox(Entity entityToClone, string newLogicalName = "")
        {
            Entity newEntity;

            if (newLogicalName != string.Empty)
            {
                newEntity = new Entity(newLogicalName);
            }
            else
            {
                newEntity = new Entity(entityToClone.LogicalName);
            }

            var systemAttributes = new List<string>();
            systemAttributes.Add("createdon");
            systemAttributes.Add("createdby");
            systemAttributes.Add("modifiedon");
            systemAttributes.Add("modifiedby");
            systemAttributes.Add("owninguser");
            systemAttributes.Add("owningbusinessunit");
            systemAttributes.Add("organizationid");


            foreach (var attribute in entityToClone.Attributes
                .Where(x => x.Key != entityToClone.LogicalName + "id")
                .Where(x => !systemAttributes.Contains(x.Key))) 
            {

                switch (attribute.Value.GetType().Name)
                {
                    case "Money":
                        var m = attribute.Value as Money;
                        newEntity[attribute.Key] = new Money(m.Value);
                        break;
                    case "EntityReference":
                        var er = attribute.Value as EntityReference;
                        newEntity[attribute.Key] = new EntityReference(er.LogicalName, er.Id);
                        break;
                    case "OptionSetValue":
                        var os = attribute.Value as OptionSetValue;
                        newEntity[attribute.Key] = new OptionSetValue(os.Value);
                        break;
                    default:
                        newEntity[attribute.Key] = attribute.Value;
                        break;
                }
                
            }

            return newEntity;
        }

        internal static Entity CloneEntitySandbox(Entity entityToClone, List<string> fieldNamesToSkip)
        {
            var e = new Entity(entityToClone.LogicalName);

            foreach (var attr in entityToClone.Attributes)
            {
                if (attr.Key != entityToClone.LogicalName + "id" && !fieldNamesToSkip.Contains(attr.Key))
                {
                    e.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            return e;
        }

        internal static T CloneEntitySandbox<T>(T entityToClone) where T : Entity, new()
        {
            var e = new T();
            e.LogicalName = entityToClone.LogicalName;

            foreach (var attr in entityToClone.Attributes)
            {
                if (attr.Key != entityToClone.LogicalName + "id")
                {
                    e.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            foreach (var f in entityToClone.FormattedValues)
            {
                e.FormattedValues.Add(f.Key, f.Value);
            }


            return e;
        }

        internal static string GetFormattedValue(this Entity entity, string attributeName)
        {
            if (entity.Attributes.Contains(attributeName))
            {
                return entity.FormattedValues[attributeName];
            }
            else
            {
                return null;
            }

        }


        internal static bool IsSystemAdmin(this IOrganizationService service, Guid userId)
        {
            // All MS Dynamics CRM instances share the same System Admin role GUID.
            // Hence, we can hardode it as this will not represent a security issue
            Guid adminId = new Guid("627090FF-40A3-4053-8790-584EDC5BE201");

            var q = new QueryExpression("role");
            q.Criteria.AddCondition("roletemplateid", ConditionOperator.Equal, adminId);
            var link = q.AddLink("systemuserroles", "roleid", "roleid");
            link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            return service.GetMultiple(q).Count > 0;

        }

        internal static bool HasSupportRole(this IOrganizationService service, Guid userId)
        {
            var q = new QueryExpression("role");
            q.Criteria.AddCondition("name", ConditionOperator.Equal, "CAMS Support");
            var link = q.AddLink("systemuserroles", "roleid", "roleid");
            link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            return service.GetMultiple(q).Count > 0;
        }
        internal static List<Entity> UserRoles(this IOrganizationService service, Guid userId)
        {
            var q = new QueryExpression("role");
            q.ColumnSet = new ColumnSet(true);
            var link = q.AddLink("systemuserroles", "roleid", "roleid");
            link.Columns = new ColumnSet(true);
            link.EntityAlias = "sur";
            link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            return service.GetMultiple(q);
        }

        internal static T GetAttributeValue<T>(this Entity entity, string attributeName, Entity image)
        {
            if (entity.Attributes.Contains(attributeName))
            {
                return entity.GetAttributeValue<T>(attributeName);
            }
            else if (image != null && image.Attributes.Contains(attributeName))
            {
                return image.GetAttributeValue<T>(attributeName);
            }
            else
            {
                return default(T);
            }
        }

        internal static void SetAttributeValue(this Entity entity, string attributeName, object attributeValue)
        {
            if (entity.Attributes.Contains(attributeName))
            {
                entity.Attributes[attributeName] = attributeValue;
            }
            else
            {
                entity.Attributes.Add(attributeName, attributeValue);
            }
        }

        internal static string SerializeToString(this Entity entity)
        {
            string result = string.Empty;
            using (MemoryStream memStm = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(Entity));
                serializer.WriteObject(memStm, entity);

                memStm.Seek(0, SeekOrigin.Begin);
                result = new StreamReader(memStm).ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Gets specified entity metadata (include attributes)
        /// </summary>
        /// <param name="service">CRM organization service</param>
        /// <param name="logicalName">Logical name of the entity</param>
        /// <returns>Entity metadata</returns>
        internal static EntityMetadata RetrieveEntityMetadata(IOrganizationService service, string logicalName)
        {
            try
            {
                var request = new RetrieveEntityRequest
                {
                    LogicalName = logicalName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true

                };

                var response = (RetrieveEntityResponse)service.Execute(request);

                return response.EntityMetadata;
            }
            catch (Exception error)
            {
                throw new Exception("RetrieveAuditHistory Error while retrieving entity metadata: " + error.StackTrace);
            }
        }

        /// <summary>
        /// Get metadata of an attribute
        /// </summary>
        /// <param name="service">the instance of the organisation service</param>
        /// <param name="entityName">the entity name</param>
        /// <param name="attributeName">the attribute name</param>
        /// <returns>the attribute metadata</returns>
        internal static AttributeMetadata RetrieveAttributeMetadata(IOrganizationService service, string entityName, string attributeName)
        {
            try
            {
                var attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = true
                };

                // Execute the request
                var attributeResponse =
                    (RetrieveAttributeResponse)service.Execute(attributeRequest);

                return attributeResponse.AttributeMetadata;
            }
            catch (Exception error)
            {
                throw new Exception("RetrieveAuditHistory Error while retrieving attribute metadata: " + error);
            }
        }

        /// <summary>
        /// Get the name of the target entity
        /// </summary>
        /// <param name="service">the instance of the organisation service</param>
        /// <param name="entityName">the schema name of the entity</param>
        /// <param name="entityId">the GUID id of the entity</param>
        /// <param name="primaryAttributeName">the primary attribute name of the entity</param>
        /// <returns>the name of the entity record</returns>
        internal static string RetrieveTargetName(IOrganizationService service, string entityName, Guid entityId, string primaryAttributeName)
        {
            try
            {
                var record = service.Retrieve(entityName, entityId, new ColumnSet(primaryAttributeName));
                return record.GetAttributeValue<string>(primaryAttributeName);
            }
            catch (Exception error)
            {
                throw new Exception("RetrieveAuditHistory Error while retrieving name for one record: " + error);
            }
        }


        internal static void ExecuteMultiple(IOrganizationService service, IEnumerable<OrganizationRequest> requests)
        {
            var req = new ExecuteMultipleRequest();
            req.Requests = new OrganizationRequestCollection();
            req.Settings = new ExecuteMultipleSettings();
            req.Settings.ContinueOnError = false;
            req.Settings.ReturnResponses = false;

            req.Requests.AddRange(requests);

            service.Execute(req);
        }



        internal static void SetStatus(this IOrganizationService service, EntityReference entity, int state, int status)
        {
            var req = new SetStateRequest();
            req.EntityMoniker = entity;
            req.State = new OptionSetValue(state);
            req.Status = new OptionSetValue(status);
            service.Execute(req);
        }

        internal static Entity GetEntityByName(this IOrganizationService service, string entityName,string nameFieldName, string nameValue)
        {
            var q = new QueryExpression(entityName);
            q.ColumnSet = new ColumnSet(true);
            q.AddCriteria(nameFieldName, nameValue);

            return service.GetMultiple(q).SingleOrDefault();
        }

        internal static Entity GetEntity(this IOrganizationService service, string entityName, Guid id)
        {
            return service.Retrieve(entityName, id, new ColumnSet(true));
        }

        internal static Entity GetEntity(this IOrganizationService service, string entityName, Guid id, string[] columns)
        {
            return service.Retrieve(entityName, id, new ColumnSet(columns));
        }
        internal static Entity GetEntity(this IOrganizationService service, string entityName, Guid id, ColumnSet columns)
        {
                return service.Retrieve(entityName, id, columns);
        }

        internal static Entity GetEntity(this IOrganizationService service, EntityReference entity)
        {
            return service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));
        }

        internal static Entity GetEntity(this IOrganizationService service, EntityReference entity, string[] columns)
        {
            return service.Retrieve(entity.LogicalName, entity.Id,  new ColumnSet(columns));
        }
        internal static Entity GetEntity(this IOrganizationService service, EntityReference entity, ColumnSet columns)
        {
            return service.Retrieve(entity.LogicalName, entity.Id, columns);
        }

        internal static bool AccessTeamExists(this IOrganizationService service, EntityReference record, Guid teamTemplateId)
        {
            var q = new QueryExpression("team");
            q.AddCriteria("teamtype", 1);
            q.AddCriteria("teamtemplateid", teamTemplateId);
            q.AddCriteria("regardingobjectid", record.Id);

            return service.GetMultiple(q).Any();

        }
        internal static bool UserIsInAccessTeam(IOrganizationService service, Guid stakeholderId, string teamTemplateName, EntityReference recordToUse, ITracingService trace)
        {
            return UserIsInAccessTeam(service, stakeholderId, GetAccessTeamTemplateByName(service, teamTemplateName).Id, recordToUse, trace);
        }
        internal static bool UserIsInAccessTeam(IOrganizationService service, Guid stakeholderId, string teamTemplateName, EntityReference recordToUse)
        {
            return UserIsInAccessTeam(service, stakeholderId, GetAccessTeamTemplateByName(service, teamTemplateName).Id, recordToUse, null);
        }

        internal static bool UserIsInAccessTeam(IOrganizationService service, Guid stakeholderId, Guid teamTemplateId, EntityReference recordToUse)
        {
            return UserIsInAccessTeam(service, stakeholderId, teamTemplateId, recordToUse, null);
        }

        internal static List<Entity> GetAccessTeamMembers(IOrganizationService service, string teamTemplateName, EntityReference recordToUse)
        {
            var fetchXml = $@"
                    <fetch>
                      <entity name='teammembership'>
                        <attribute name='systemuserid' />
                        <link-entity name='team' from='teamid' to='teamid'>
                          <attribute name='name' />
                          <attribute name='regardingobjectid' />
                          <filter type='and'>
                            <condition attribute='regardingobjectid' operator='eq' value='{recordToUse.Id}'/>
                            <condition attribute='teamtemplateid' operator='eq' value='{GetAccessTeamTemplateByName(service, teamTemplateName).Id}'/>
                          </filter>
                        </link-entity>
                      </entity>
                      <!---->
                    </fetch>";

            var res = DynamicsServiceHelper.GetMultiple(service, fetchXml);

            return res;
        }


        internal static bool UserIsInAccessTeam(IOrganizationService service, Guid stakeholderId, Guid teamTemplateId, EntityReference recordToUse, ITracingService trace)
        {
#if MOCKUP
            var q = new QueryExpression("teammembership");
            q.ColumnSet = new ColumnSet(true);
            q.AddCriteria("systemuserid", stakeholderId);
            //var link = q.AddLink("team", "teamid", "teamid");
            //link.EntityAlias = "team";
            //link.Columns = new ColumnSet(true);
            //link.LinkCriteria.AddCondition("regardingobjectid", ConditionOperator.Equal, recordToUse.Id);
            var sw = new Stopwatch();
            sw.Start();
            var mem = service.GetMultiple(q);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            if (!mem.Any())
                return false;

            var q2 = new QueryExpression("team");
            q2.ColumnSet = new ColumnSet(true);
            q2.Criteria.AddCondition("teamtype", ConditionOperator.Equal, 1);
            q2.Criteria.AddCondition("teamtemplateid", ConditionOperator.Equal, teamTemplateId);
            q2.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, recordToUse.Id);
            var teams = service.GetMultiple(q2);

            if (!teams.Any())
                return false;

            var any = mem.Select(x => x.GetAttributeValue<Guid>("teamid")).Intersect(teams.Select(x => x.Id));

            //mem = mem.Where(x => x.GetValue<OptionSetValue>("team.teamtype").Value == 1)
            //         .Where(x => x.Contains("team.regardingobjectid") && x.GetValue<EntityReference>("team.regardingobjectid").Id == recordToUse.Id)
            //         .Where(x => x.Contains("team.teamtemplateid") && x.GetValue<EntityReference>("team.teamtemplateid").Id == teamTemplateId)
            //        .ToList();

            return any.Any();

#else
            //much as i don't like inline fetchxml, it made this query across the 3 team entiteis much easier to visualise
            if (trace != null)
            {
                //trace.Trace("checking user is in access team");
            }
            var fetchXml = $@"
                    <fetch>
                      <entity name='teammembership'>
                        <attribute name='systemuserid' />
                        <filter>
                          <condition attribute='systemuserid' operator='eq' value='{stakeholderId}'/>
                        </filter>
                        <link-entity name='team' from='teamid' to='teamid'>
                          <attribute name='name' />
                          <attribute name='regardingobjectid' />
                          <filter type='and'>
                            <condition attribute='regardingobjectid' operator='eq' value='{recordToUse.Id}'/>
                            <condition attribute='teamtemplateid' operator='eq' value='{teamTemplateId}'/>
                          </filter>
                        </link-entity>
                      </entity>
                      <!---->
                    </fetch>";

            var res = DynamicsServiceHelper.GetMultiple(service, fetchXml);
            return res.Any();
#endif
        }


        internal static Entity GetAccessTeamTemplateByName(IOrganizationService service, string accessTeamName)
        {
            var q = new QueryExpression("teamtemplate");
            q.Criteria.AddCondition("teamtemplatename", ConditionOperator.Equal, accessTeamName);
            return service.GetMultiple(q).SingleOrDefault();
        }

        internal static void AddUserToAccessTeam(this IOrganizationService service, EntityReference record, Guid userId, string teamTemplateName,ITracingService trace,string process)
        {
            AddUserToAccessTeam(service, record, userId, GetAccessTeamTemplateByName(service, teamTemplateName).Id, trace,false,process);
        }

        internal static void DisablePluginStep(IOrganizationService service,ITracingService trace, string stepName)
        {
            trace.Trace($"disable the {stepName} plugin");
            var q = new QueryExpression("sdkmessageprocessingstep");
            q.AddCriteria("name", stepName);
            var step = service.GetMultiple(q).Single();

            var ssr = new SetStateRequest();
            ssr.EntityMoniker = new EntityReference("sdkmessageprocessingstep", step.Id);
            ssr.State = new OptionSetValue(1);
            ssr.Status = new OptionSetValue(2);
            service.Execute(ssr);
            trace.Trace("plugin step disabled");
        }


        internal static void EnablePluginStep(IOrganizationService service, ITracingService trace, string stepName)
        {
            trace.Trace($"enable the {stepName} plugin");
            var q = new QueryExpression("sdkmessageprocessingstep");
            q.AddCriteria("name", stepName);
            var step = service.GetMultiple(q).Single();

            var ssr = new SetStateRequest();
            ssr.EntityMoniker = new EntityReference("sdkmessageprocessingstep", step.Id);
            ssr.State = new OptionSetValue(0);
            ssr.Status = new OptionSetValue(1);
            service.Execute(ssr);
            trace.Trace("plugin step enabled");
        }

        internal static Guid GetMilestoneCompleteStatusId(IOrganizationService service)
        {
            var q = new QueryExpression("cams_milestonestatus");
            q.AddCriteria("cams_finalstatus", true);
            return service.GetMultiple(q).Single().Id;
        }
        internal static Guid GetMilestoneRejectedStatusId(IOrganizationService service)
        {
            var q = new QueryExpression("cams_milestonestatus");
            q.AddCriteria("cams_name", "REJECTED");
            return service.GetMultiple(q).Single().Id;
        }

        internal static Guid GetMilestoneDeliveredStatusId(IOrganizationService service)
        {
            var q = new QueryExpression("cams_milestonestatus");
            q.AddCriteria("cams_name", "COMPLETED");
            return service.GetMultiple(q).Single().Id;
        }

        internal static void UpdateTokenizedField(IOrganizationService service, ITracingService tracer, Entity docPart, Entity entity, string fieldName,string templateFieldName = "",bool replaceNonEditableSection=false)
        {

            //see if we can find the customer and delivery agent tokens on the prelimes section signatures field
            tracer.Trace("\t start");
            if (string.IsNullOrEmpty(templateFieldName))
            {
                templateFieldName = fieldName;
            }
            tracer.Trace("\t 1");

            if (!entity.Contains(fieldName) || string.IsNullOrEmpty(entity[fieldName].ToString()))
            {
                tracer.Trace("field does not exist on entity or is blank, exiting");
                return;
            }

            tracer.Trace("\t 2");

            //tracer.Trace(fieldName + " : " + entity[fieldName].ToString());
            //tracer.Trace(templateFieldName + " : " + entity.GetValue<string>(templateFieldName));

            string test = entity[templateFieldName].ToString();
            Regex reg = new Regex(@"(\[[a-zA-Z\s]*\])+");
            tracer.Trace(reg.ToString());

            var matches = reg.Matches(test);
            if (matches.Count > 0)
            {
                tracer.Trace("\t found matches");

                tracer.Trace("a");

                Entity ap;
                if (docPart.LogicalName == "cams_agreementplan")
                {
                    tracer.Trace("\tap");
                    ap = service.GetEntity(docPart.ToEntityReference());
                }
                else
                {
                    tracer.Trace("\tan or ax");
                    if (docPart.Contains("cams_agreementplanid"))
                    {
                        var apId = docPart.GetValue<EntityReference>("cams_agreementplanid").Id;
                        ap = service.GetEntity("cams_agreementplan", apId);
                    }
                    else
                    {
                        tracer.Trace("no ap id - exiting");
                        return;
                    }
                    
                }

                tracer.Trace("b");

                foreach (Match match in matches)
                {
                    tracer.Trace(match.Value);

                    string replace = string.Empty;
                    switch (match.Value)
                    {
                        case "[CUSTOMER]":
                            if (ap.Contains("cams_customer") && ap.GetEntityReferenceValue("cams_customer") != null)
                            {
                                replace = ap.GetValue<EntityReference>("cams_customer").Name;
                            }
                            
                            break;
                        case "[CUSTOMER BRANCH]":
                           // tracer.Trace("looking up customer branch organisation");
                            var ccb = service.GetEntity("account", docPart.GetValue<EntityReference>("cams_customercapabilitybranch").Id);
                            replace = ccb.GetValue<string>("name");
                            break;
                        case "[DELIVERY AGENT]":
                            if (ap.Contains("cams_deliveryagent") && ap.GetEntityReferenceValue("cams_deliveryagent") != null)
                            {
                                replace = ap.GetValue<EntityReference>("cams_deliveryagent").Name;
                            }
                            
                            break;
                        case "[DELIVERY AGENT TEAM]":
                           // tracer.Trace("looking up delivery agent team organisation");
                            var dat = service.GetEntity("account", docPart.GetValue<EntityReference>("cams_deliveryagentteam").Id);
                            replace = dat.GetValue<string>("name");
                            break;
                        case "[ANNEX NAME]":
                            replace = docPart.GetValue<string>("cams_name");
                            break;
                        case "[APPENDIX NAME]":
                            replace = docPart.GetValue<string>("cams_name");
                            break;

                    }

                    if (!string.IsNullOrEmpty(replace))
                    {
                        test = test.Replace(match.Value, replace);
                    }

                    
                    tracer.Trace("c");

                }

                if (replaceNonEditableSection)
                {


                    tracer.Trace("\tupdating not editable section");

                    //serch for and replace the non-editable section
                    var reg2 = new Regex(@"(<p contenteditable=""false"">[a-zA-Z0-9,:;<>\[\](){}!#.\/\=\""\-\&\s]*<\/p>)");
                    tracer.Trace(reg2.ToString());
                   // tracer.Trace(entity.GetValue<string>(fieldName));

                    var matches2 = reg2.Matches(entity.GetValue<string>(fieldName));
                    if (matches2.Count == 1)
                    {
                        tracer.Trace("\t\t match found");
                    //    tracer.Trace(matches2[0].Value);
                        entity[fieldName] = entity.GetValue<string>(fieldName).Replace(matches2[0].Value, test);
                    }
                    else 
                    {
                        tracer.Trace("\t\t * No Matches Found * trying editable version");
                        var reg3 = new Regex(@"(<p>[a-zA-Z0-9,:;<>\[\](){}!#.\/\=\""\-\&\s]*<\/p>)");
                        tracer.Trace(reg3.ToString());
                        var matches3 = reg3.Matches(entity.GetValue<string>(fieldName));
                        if (matches3.Count == 1)
                        {
                            tracer.Trace("\t\t match found");
                            //    tracer.Trace(matches2[0].Value);
                            entity[fieldName] = entity.GetValue<string>(fieldName).Replace(matches3[0].Value, test);
                        }
                        else
                        {
                            tracer.Trace("\t\t * still No Matches Found * giving it up as a bad job");
                        }
                    }
                }
                else
                {
                    tracer.Trace("\treplacing whole field");

                    entity[fieldName] = test;
                }





            }

        }


        internal static void AddUserToAccessTeam(this IOrganizationService service, EntityReference record, Guid userId, Guid teamTemplateId,ITracingService trace,bool async,string process,bool useLock = false,bool audit = false)
        {
            //trace.Trace($"\tUserId : {userId}");
            //trace.Trace($"\tTeamTemplateId : {teamTemplateId}");
            //trace.Trace($"\tRecordId : {record.Id}");
            var sw = new Stopwatch();
            sw.Start();
            if (async)
            {
                StartAsyncProcess(service, record, userId, teamTemplateId, process);
            }
            else
            {

                AddUserToRecordTeamResponse resp = null;

                if (UserIsLicensed(service, userId, trace))
                {
                    if (!UserIsInAccessTeam(service, userId, teamTemplateId, record,trace))
                    {

                        if (useLock)
                        {
                            LockRecord(service, record, userId, teamTemplateId);
                        }

                        AddUserToRecordTeamRequest teamAddRequest2 = new AddUserToRecordTeamRequest();
                        teamAddRequest2.Record = record;
                        teamAddRequest2.SystemUserId = userId;
                        teamAddRequest2.TeamTemplateId = teamTemplateId;
                        resp = service.Execute(teamAddRequest2) as AddUserToRecordTeamResponse;
                        trace.Trace("\tuser added to access team");
                        if (audit)
                        {
                            CreateAuditRecord(service, record, userId, teamTemplateId, process);
                        }

                    }
                    else
                    {
                        trace.Trace("user already in access team");
                    }
                }
                else
                {
                    trace.Trace("user not licensed");
                }

                sw.Stop();
                trace.Trace($"\t\tadd user to access team took {sw.ElapsedMilliseconds.ToString()} ms");

            }
        }

       
        internal static bool UserIsLicensed(IOrganizationService service, Guid userId, ITracingService trace)
        {
            //trace.Trace("loading user");
            var user = service.GetEntity("systemuser", userId);
            //trace.Trace($"user is licensed {user.GetValue<bool>("islicensed").ToString()} ");
            return user.GetValue<bool>("islicensed");
        }

        private static void StartAsyncProcess(IOrganizationService service, EntityReference record, Guid userId, Guid teamTemplateId, string process)
        {
            var req = new OrganizationRequest($"cams_AddUserToAccessTeam");
            req["Target"] = new EntityReference("systemuser", userId);
            req["EntityName"] = record.LogicalName;
            req["EntityId"] = record.Id.ToString();
            req["TeamTemplateId"] = teamTemplateId.ToString();
            req["Process"] = process;
            var resp = (OrganizationResponse)service.Execute(req);
        }

        private static void CreateAuditRecord(IOrganizationService service, EntityReference record, Guid userId, Guid teamTemplateId, string process)
        {
            var req = new OrganizationRequest($"cams_CreateAccessTeamAudit");
            req["Target"] = new EntityReference("systemuser", userId);
            req["TeamTemplateId"] = teamTemplateId.ToString();
            req["EntityName"] = record.LogicalName;
            req["EntityId"] = record.Id.ToString().ToLower();
            req["Action"] = 448980000;
            req["Process"] = process;
            service.Execute(req);
        }

        private static void LockRecord(IOrganizationService service, EntityReference record, Guid userId, Guid teamTemplateId)
        {
            //create a lock record, or get if already exists
            var lockName = record.Id.ToString().ToLower() + "::" + userId.ToString().ToLower() + "::" + teamTemplateId.ToString().ToLower();

            var lockRec = service.GetEntityByName("cams_lock", "cams_name", lockName);

            if (lockRec == null)
            {
                lockRec = new Entity("cams_lock");
                lockRec["cams_name"] = lockName;
                lockRec.Id = service.Create(lockRec);
            }

            var lockToUpdate = new Entity(lockRec.LogicalName, lockRec.Id);
            lockToUpdate["cams_lockfield"] = DateTime.Now.ToString();
            service.Update(lockToUpdate);
        }

        internal static List<Entity> GetStakeholderRolesForDocPart(IOrganizationService service, EntityReference parentRef, EntityReference stakeholder = null)
        {
            var q2 = new QueryExpression($"{parentRef.LogicalName}stakeholderrole");
            q2.ColumnSet = new ColumnSet(true);
            q2.AddCriteria($"{parentRef.LogicalName}id", parentRef.Id);
            if (stakeholder != null)
            {
                q2.AddCriteria("cams_stakeholderid", stakeholder.Id);
            }

            var stakeholderRoles = service.GetMultiple(q2);
            return stakeholderRoles;
        }

        internal static List<Entity> GetStakeholderRolesForDocPart(IOrganizationService service, EntityReference parentRef, Guid userId)
        {
            return GetStakeholderRolesForDocPart(service, parentRef, new EntityReference("systemuser", userId));
        }

        internal static void RemoveUserFromAccessTeam(this IOrganizationService service, EntityReference record, Guid userId, string teamTemplateName, ITracingService trace, bool async, string process, bool useLock = false, bool audit = false)
        {
            RemoveUserFromAccessTeam(service, record, userId, GetAccessTeamTemplateByName(service,teamTemplateName).Id, trace, async, process,useLock,audit);
        }

        internal static void RemoveUserFromAccessTeam(this IOrganizationService service, EntityReference record, Guid userId, Guid teamTemplateId, ITracingService trace, bool async, string process, bool useLock = false, bool audit = false)
        {

            if (async)
            {
                var req = new OrganizationRequest($"cams_RemoveUserFromAccessTeam");
                req["Target"] = new EntityReference("systemuser", userId);
                req["EntityName"] = record.LogicalName;
                req["EntityId"] = record.Id.ToString();
                req["TeamTemplateId"] = teamTemplateId.ToString();
                req["Process"] = process;
                var resp = (OrganizationResponse)service.Execute(req);
                trace.Trace("async process triggered");
            }
            else
            {
                
                if (UserIsInAccessTeam(service, userId, teamTemplateId, record,trace))
                {
                    //create a lock record, or get if already exists
                    var sw = new Stopwatch();
                    sw.Start();

                    if (useLock)
                    {
                        var lockName = record.Id.ToString().ToLower() + "::" + userId.ToString().ToLower() + "::" + teamTemplateId.ToString().ToLower();

                        var lockRec = service.GetEntityByName("cams_lock", "cams_name", lockName);

                        if (lockRec == null)
                        {

                            lockRec = new Entity("cams_lock");
                            lockRec["cams_name"] = lockName;
                            lockRec.Id = service.Create(lockRec);
                        }

                        var lockToUpdate = new Entity(lockRec.LogicalName, lockRec.Id);
                        lockToUpdate["cams_lockfield"] = DateTime.Now.ToString();
                        service.Update(lockToUpdate);
                    }

                    RemoveUserFromRecordTeamRequest teamAddRequest = new RemoveUserFromRecordTeamRequest();
                    teamAddRequest.Record = record;
                    teamAddRequest.SystemUserId = userId;
                    teamAddRequest.TeamTemplateId = teamTemplateId;
                    service.Execute(teamAddRequest);

                    if (audit)
                    {
                        //audit this removal
                        var req = new OrganizationRequest($"cams_CreateAccessTeamAudit");
                        req["Target"] = new EntityReference("systemuser", userId);
                        req["TeamTemplateId"] = teamTemplateId.ToString();
                        req["EntityName"] = record.LogicalName;
                        req["EntityId"] = record.Id.ToString().ToLower();
                        req["Action"] = 448980001;
                        req["Process"] = process;
                        service.Execute(req);
                    }
             
                }
                else
                {
                    if (trace != null)
                    {
                        trace.Trace("user not found in access team");
                    }
                }

            }
        }

        internal static List<Entity> GetTeamTemplatesForRoleType(IOrganizationService service, ITracingService tracer, Guid roleId)
        {
            var q = new QueryExpression("cams_roletypeaccessteam");
            q.ColumnSet = new ColumnSet(true);
            q.Criteria.AddCondition("cams_roletype", ConditionOperator.Equal, roleId);
            return service.GetMultiple(q);

        }

        internal static Entity GetTeamTemplateByName(this IOrganizationService service, string attName)
        {
            var q = new QueryExpression("teamtemplate");
            q.ColumnSet = new ColumnSet(true);
            q.Criteria.AddCondition("teamtemplatename", ConditionOperator.Equal, attName);
            var tt = service.GetMultiple(q).Single();
            return tt;
        }


        /// <summary> 
        /// Find the Logical Name from the entity type code - this needs a reference to the Organization Service to look up metadata 
        /// </summary> 
        /// <param name="service"></param> 
        /// <returns></returns> 
        internal static string GetEntityLogicalName(IOrganizationService service,int entityTypeCode)
        {
            var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode ", MetadataConditionOperator.Equals, entityTypeCode));
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
            propertyExpression.PropertyNames.Add("LogicalName");
            var entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = entityFilter,
                Properties = propertyExpression
            };

            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);

            if (response.EntityMetadata.Count == 1)
            {
                return response.EntityMetadata[0].LogicalName;
            }
            return null;
        }
        internal static Entity GetEntity(IOrganizationService service, string entityName, string fieldName, object fieldValue)
        {
            var query = new QueryExpression(entityName);
            query.Criteria.AddCondition(fieldName, ConditionOperator.Equal, fieldValue);
            query.ColumnSet = new ColumnSet(true);
            return GetMultiple(service, query).SingleOrDefault();

        }
        internal static List<Entity> GetMultiple(this IOrganizationService service, string fetchXml)
        {
            var fetchReq = new FetchXmlToQueryExpressionRequest();
            fetchReq.FetchXml = fetchXml;

            var res = (FetchXmlToQueryExpressionResponse)service.Execute(fetchReq);

            return GetMultiple(service, res.Query);
        }

        internal static void UpdateMultipleAttributes(this IOrganizationService service, string entityName, Guid entityId, Dictionary<string, object> attributes)
        {

            UpdateMultipleAttributes(service, new EntityReference(entityName, entityId), attributes);
        }
        internal static void UpdateMultipleAttributes(this IOrganizationService service, Entity e, Dictionary<string, object> attributes)
        {
            UpdateMultipleAttributes(service, e.ToEntityReference(), attributes);
        }

        internal static void UpdateMultipleAttributes(this IOrganizationService service, EntityReference e, Dictionary<string, object> attributes)
        {
            //using this format rather than constructor for CRM2011 compatibility
            var entity = new Entity { LogicalName = e.LogicalName, Id = e.Id };
            foreach (var kvp in attributes)
            {
                entity[kvp.Key] = kvp.Value;
            }
            service.Update(entity);

        }

        internal static void UpdateSingleAttribute(this IOrganizationService service, string entityName, Guid entityId, string attributeName, object attributeValue)
        {
            UpdateSingleAttribute(service, new EntityReference(entityName, entityId), attributeName, attributeValue);
        }

        internal static void UpdateSingleAttribute(this IOrganizationService service, Entity e, string attributeName, object attributeValue)
        {
            UpdateSingleAttribute(service, e.ToEntityReference(), attributeName, attributeValue);
        }

        internal static void UpdateSingleAttribute(this IOrganizationService service, EntityReference e, string attributeName, object attributeValue)
        {
            //using this format rather than constructor for CRM2011 compatibility
            var entity = new Entity { LogicalName = e.LogicalName, Id = e.Id };
            entity[attributeName] = attributeValue;
            service.Update(entity);

        }

        internal static List<Entity> GetMultiple(this IOrganizationService service, QueryBase query)
        {
            
            var resp = service.RetrieveMultiple(query);
            if (resp != null && resp.Entities != null)
            {
                return resp.Entities.ToList();
            }
            else
            {
                return new List<Entity>();
            }
        }

        internal static void AddCriteria(this QueryExpression q, string fieldName, object value)
        {
            q.Criteria.AddCondition(fieldName, ConditionOperator.Equal, value);
        }

        internal static List<T> GetMultiple<T>(this IOrganizationService service, QueryBase query)
        {
            var resp = service.RetrieveMultiple(query);
            if (resp != null && resp.Entities != null)
            {
                return resp.Entities.Cast<T>().ToList();
            }
            else
            {
                return new List<T>();
            }
        }

        internal static string GetOptionsetText(IOrganizationService service, string optionSetName, int optionSetValue, string entityName = "")
        {
            try
            {
                var options = GetOptionSetMetadata(service, optionSetName, entityName);
                IList<OptionMetadata> optionsList = (from o in options.Options
                                                     where o.Value != null && o.Value.Value == optionSetValue
                                                     select o).ToList();
                var optionSetLabel = (optionsList.Count > 0) ? optionsList.First().Label.UserLocalizedLabel.Label : "(Value No Found)";
                return optionSetLabel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        static Dictionary<Tuple<string, string>, int> osValueCache;

        internal static int GetOptionSetValue(IOrganizationService service, string optionSetName, string optionSetText, string entityName = "")
        {

            if (osValueCache == null)
            {
                osValueCache = new Dictionary<Tuple<string, string>, int>();
            }

            if (osValueCache.ContainsKey(new Tuple<string, string>(optionSetName, optionSetText)))
            {
                return osValueCache[new Tuple<string, string>(optionSetName, optionSetText)];
            }

            try
            {
                var options = GetOptionSetMetadata(service, optionSetName, entityName);
                IList<OptionMetadata> optionsList = (from o in options.Options
                                                     where o.Value != null && o.Label.UserLocalizedLabel.Label == optionSetText
                                                     select o).ToList();
                var optionSetLabel = (optionsList.Count > 0) ? optionsList.First().Value.Value : 0;
                
                osValueCache[new Tuple<string, string>(optionSetName, optionSetText)] = optionSetLabel;
                
                return optionSetLabel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static Guid Create(IOrganizationService service, Entity entity)
        {
            return service.Create(entity);
        }

        internal static void Delete(this IOrganizationService service, Entity entity)
        {
            service.Delete(entity.LogicalName,entity.Id );
        }

        /// <summary>
        /// Executes a query expression and return a List of Entities
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="query">query expression</param>
        /// <returns>list of entities</returns>
        
        internal static List<Entity> GetAll(this IOrganizationService service, QueryExpression query)
        {
            var entities = new List<Entity>();

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;
            query.PageInfo.PageNumber = 1;
            var res = service.RetrieveMultiple(query);
            entities.AddRange(res.Entities);
            while (res.MoreRecords == true)
            {
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = res.PagingCookie;
                res = service.RetrieveMultiple(query);
                entities.AddRange(res.Entities);
            }

            return entities;
        }
        internal static List<Entity> GetAll(this IOrganizationService service, string entityName)
        {
            var entities = new List<Entity>();

            var query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(true);

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;
            query.PageInfo.PageNumber = 1;
            var res = service.RetrieveMultiple(query);
            entities.AddRange(res.Entities);
            while (res.MoreRecords == true)
            {
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = res.PagingCookie;
                res = service.RetrieveMultiple(query);
                entities.AddRange(res.Entities);
            }

            return entities;
        }

        /// <summary>
        /// Get the text of the OptionSet
        /// </summary>
        /// <param name="service">the instance of the CRM Organization service</param>
        /// <param name="optionSetName">the name of the OptionSet attribute</param>
        /// <param name="optionSetValue">the value of the OptionSet attribute</param>
        /// <param name="entityName">the name of the entity (optional parameter)</param>
        /// <returns>The label of the OptionSet field</returns>


        /// <summary>
        /// Get the value of the OptionSet field
        /// </summary>
        /// <param name="service">the instance of the CRM Organization service</param>
        /// <param name="optionSetName">the name of the OptionSet attribute</param>
        /// <param name="optionSetText">the text of the OptionSet attribute</param>
        /// <param name="entityName">the name of the entity (optional parameter)</param>
        /// <returns>The value of the OptionSet field</returns>


        private static string GetRoleName(XmlDocument doc)
        {

            string result = string.Empty;
            XmlNode node = doc.SelectSingleNode(ROLENAME);

            if (node != null)
            {
                result = node.InnerText;
            }

            return result;

        }

        internal static void DeactivateEntity(IOrganizationService service, Entity entity)
        {
            DeactivateEntity(service, entity.ToEntityReference());
        }

        internal static void DeactivateEntity(IOrganizationService service, EntityReference entityRef)
        {
            SetStateRequest setState = new SetStateRequest();
            setState.EntityMoniker = entityRef;
            setState.State = new OptionSetValue();
            setState.State.Value = 1;
            setState.Status = new OptionSetValue();
            setState.Status.Value = 2;
            SetStateResponse setStateResponse = (SetStateResponse)service.Execute(setState);
        }


        internal static void ActivateEntity(IOrganizationService service, Entity entity)
        {
            ActivateEntity(service, entity.ToEntityReference());
        }

        internal static void ActivateEntity(IOrganizationService service, EntityReference entityRef)
        {
            SetStateRequest setState = new SetStateRequest();
            setState.EntityMoniker = entityRef;
            setState.State = new OptionSetValue();
            setState.State.Value = 0;
            setState.Status = new OptionSetValue();
            setState.Status.Value = 1;
            SetStateResponse setStateResponse = (SetStateResponse)service.Execute(setState);
        }

        /// <summary>
        /// Get the Metadata of the OptionSet
        /// </summary>
        /// <param name="service">the instance of the service</param>
        /// <param name="optionsetName">the name of attribute</param>
        /// <param name="entityName">the name of the entity (optional parameter, default as empty if not passed)</param>
        /// <returns>the MetaData of the OptionSet</returns>
        internal static OptionSetMetadata GetOptionSetMetadata(IOrganizationService service, string optionsetName, string entityName = "")
        {

            try
            {
                OptionSetMetadata optionSetMetadata = null;

                if (string.IsNullOrEmpty(entityName))
                {
                    var retrieveOptionSetRequest = new RetrieveOptionSetRequest
                    {
                        Name = optionsetName,
                        RetrieveAsIfPublished = true
                    };

                    // Execute the request.
                    var retrieveOptionSetResponse = (RetrieveOptionSetResponse)service.Execute(retrieveOptionSetRequest);

                    // Access the retrieved OptionSetMetadata.
                    optionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;
                }
                else
                {
                    var request = new RetrieveAttributeRequest
                    {
                        EntityLogicalName = entityName,
                        LogicalName = optionsetName,
                        RetrieveAsIfPublished = true
                    };

                    var resp = (RetrieveAttributeResponse)service.Execute(request);

                    if (optionsetName.Contains("familystatuscode"))
                    {
                        var retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)resp.AttributeMetadata;
                        optionSetMetadata = retrievedPicklistAttributeMetadata.OptionSet;
                    }
                    else if (optionsetName.Contains("statecode"))
                    {
                        var retrievedPicklistAttributeMetadata = (StateAttributeMetadata)resp.AttributeMetadata;
                        optionSetMetadata = retrievedPicklistAttributeMetadata.OptionSet;
                    }
                    else if (optionsetName.Contains("statuscode"))
                    {
                        var retrievedPicklistAttributeMetadata = (StatusAttributeMetadata)resp.AttributeMetadata;
                        optionSetMetadata = retrievedPicklistAttributeMetadata.OptionSet;
                    }
                    else
                    {
                        try
                        {
                            var retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)resp.AttributeMetadata;
                            optionSetMetadata = retrievedPicklistAttributeMetadata.OptionSet;
                        }
                        catch (Exception)
                        {

                            //return nothing
                        }
                        
                    }
                }

                return optionSetMetadata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static object GetSharedVariable(IPluginExecutionContext context, string variableName, bool traverseParentContext = true)
        {
            IPluginExecutionContext checkContext = context;
            while (checkContext != null)
            {
                if (checkContext.SharedVariables.Contains(variableName))
                {
                    return checkContext.SharedVariables[variableName];
                }
                else if (traverseParentContext && checkContext.ParentContext != null)
                {
                    checkContext = checkContext.ParentContext;
                }
                else
                {
                    checkContext = null;
                }
            }
            return null;
        }

        public static T GetSharedVariable<T>(IPluginExecutionContext context, string variableName, bool traverseParentContext = true)
        {
            object boxed = GetSharedVariable(context, variableName, traverseParentContext);
            if (boxed != null)
            {
                try
                {
                    T converted = (T)boxed;
                    return converted;
                }
                catch { }
            }
            return default;
        }
    }
}