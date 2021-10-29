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
    public class EmailSendAction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            const string pluginName = "EmailSendAction";
            const string targetEntityName = "email";

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace($"{pluginName} Execute");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            EntityReference targetEmailRef = null;
            if (context.InputParameters.Contains("Target"))
            {
                targetEmailRef = context.InputParameters["Target"] as EntityReference;
            }
            if (targetEmailRef == null || targetEmailRef.LogicalName != targetEntityName)
            {
                throw new InvalidPluginExecutionException($"{pluginName} requires a Target Entity Reference parameter of type {targetEntityName}.");
            }

            bool createContacts = false;
            if (context.InputParameters.Contains("CreateContacts"))
            {
                createContacts = (bool)context.InputParameters["CreateContacts"];
            }

            bool temporaryContacts = false;
            if (context.InputParameters.Contains("TemporaryContacts"))
            {
                temporaryContacts = (bool)context.InputParameters["TemporaryContacts"];
            }

            string appendToAddressesCsv = null;
            if (context.InputParameters.Contains("ToAddressesCsv"))
            {
                appendToAddressesCsv = (string)context.InputParameters["ToAddressesCsv"];
            }

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService serviceAsAdmin = serviceFactory.CreateOrganizationService(null);

            if (!string.IsNullOrWhiteSpace(appendToAddressesCsv))
            {
                string[] appendtoAddresses = appendToAddressesCsv.Split(',');
                if (appendtoAddresses.Length > 0)
                {
                    Entity email = serviceAsAdmin.Retrieve(targetEmailRef.LogicalName, targetEmailRef.Id, new ColumnSet("to"));
                    EntityCollection to = email.Contains("to") ? email.GetAttributeValue<EntityCollection>("to") : new EntityCollection();

                    const string contactEmailAddressAttributeName = "emailaddress1";
                    QueryExpression queryContactByEmail = new QueryExpression("contact");
                    queryContactByEmail.ColumnSet = new ColumnSet(contactEmailAddressAttributeName);
                    queryContactByEmail.Criteria.AddCondition(contactEmailAddressAttributeName, ConditionOperator.In, appendtoAddresses);
                    EntityCollection matchingContacts = serviceAsAdmin.RetrieveMultiple(queryContactByEmail);

                    const string userEmailAddressAttributeName = "internalemailaddress";
                    QueryExpression queryUserByEmail = new QueryExpression("systemuser");
                    queryUserByEmail.ColumnSet = new ColumnSet(userEmailAddressAttributeName);
                    queryUserByEmail.Criteria.AddCondition(userEmailAddressAttributeName, ConditionOperator.In, appendtoAddresses);
                    EntityCollection matchingUsers = serviceAsAdmin.RetrieveMultiple(queryUserByEmail);

                    foreach (string toAddress in appendtoAddresses)
                    {
                        Entity activityParty = new Entity("activityparty");
                        bool isExisting = false;

                        if (matchingContacts != null && matchingContacts.Entities != null && matchingContacts.Entities.Count > 0)
                        {
                            foreach (Entity existingContact in matchingContacts.Entities)
                            {
                                if (existingContact.GetAttributeValue<string>(contactEmailAddressAttributeName) == toAddress)
                                {
                                    activityParty["partyid"] = new EntityReference(existingContact.LogicalName, existingContact.Id);
                                    isExisting = true;
                                    break;
                                }
                            }
                        }
                        if (!isExisting &&
                            matchingUsers != null && matchingUsers.Entities != null && matchingUsers.Entities.Count > 0)
                        {
                            foreach (Entity existingUser in matchingUsers.Entities)
                            {
                                if (existingUser.GetAttributeValue<string>(userEmailAddressAttributeName) == toAddress)
                                {
                                    activityParty["partyid"] = new EntityReference(existingUser.LogicalName, existingUser.Id);
                                    isExisting = true;
                                    break;
                                }
                            }
                        }
                        if (!isExisting)
                        {
                            if (createContacts || temporaryContacts)
                            {
                                //Create a Contact
                                Entity contact = new Entity("contact");
                                contact[contactEmailAddressAttributeName] = toAddress;
                                contact["firstname"] = toAddress;
                                if (temporaryContacts)
                                {
                                    contact["lastname"] = "[TEMPORARY CONTACT]";
                                    contact["cp_temporarycontactforemailid"] = targetEmailRef;
                                }
                                Guid contactId = serviceAsAdmin.Create(contact);
                                activityParty["partyid"] = new EntityReference("contact", contactId);
                            }
                            else
                            {
                                //Assume Org is configured to allow unresolved emails (in System Settings)
                                activityParty["addressused"] = toAddress;
                            }
                        }
                        to.Entities.Add(activityParty);
                    }

                    email["to"] = to;
                    serviceAsAdmin.Update(email);
                }
            }

            SendEmailRequest request = new SendEmailRequest();
            request.EmailId = targetEmailRef.Id;
            request.TrackingToken = String.Empty;
            request.IssueSend = true;
            serviceAsAdmin.Execute(request);
        }
    }
}
