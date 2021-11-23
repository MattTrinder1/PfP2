#Install-Module -Name Microsoft.Xrm.DevOps.Data.PowerShell

param (
[string]$customer
)

$conn = Get-CrmConnection -ConnectionString "RequireNewInstance=True;AuthType=ClientSecret;ClientId=83af4fce-10c3-4409-a43d-d67e300424aa;ClientSecret=H.ARok4~mmJl_lfU~nfE9JqE.7pUE26u.t;Url=https://policeproductdev.crm11.dynamics.com"



$package = Get-CrmDataPackage -Conn $conn `
-Identifiers  @{ "cp_customersetting" = @("cp_customername", "cp_policingareatext", "cp_incidentprefix","cp_areacode","cp_produceatstation") } `
-Fetches @("
<fetch>
  <entity name='cp_customersetting'>
    <attribute name='cp_customersettingid' />
    <attribute name='cp_customername' />
    <attribute name='cp_policingareatext' />
    <attribute name='cp_incidentprefix' />
    <attribute name='cp_active' />
    <attribute name='cp_areacode' />
    <attribute name='cp_produceatstation' />
    <order attribute='cp_customername' descending='false' />
    <filter type='and'>
      <condition attribute='cp_customername' operator='eq' value='$customer' />
    </filter>
  </entity>
</fetch>"
,
"<fetch >
  <entity name='cp_contactroletype'>
    <attribute name='cp_contactroletypeid' />
    <attribute name='cp_name' />
    <attribute name='cp_applicationused' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_territorialpolicingarea'>
    <attribute name='cp_territorialpolicingareaid' />
    <attribute name='cp_name' />
    <attribute name='cp_customer' />
    <order attribute='cp_name' descending='false' />
    <link-entity name='cp_customersetting' from='cp_customersettingid' to='cp_customer' link-type='inner' alias='ab'>
      <filter type='and'>
        <condition attribute='cp_customername' operator='eq' value='$customer' />
      </filter>
    </link-entity>
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='account'>
    <attribute name='name' />
    <attribute name='accountid' />
    <attribute name='address1_line1' />
    <attribute name='cp_isrecoverygaragecompany' />
    <attribute name='cp_isrecoverygarage' />
    <attribute name='parentaccountid' />
    <attribute name='cp_customer' />
    <attribute name='address1_line2' />
    <attribute name='address1_city' />
    <attribute name='address1_postalcode' />
    <attribute name='address1_stateorprovince' />
    <order attribute='name' descending='false' />
    <filter type='and'>
      <condition attribute='statecode' operator='eq' value='0' />
      <filter type='or'>
        <condition attribute='cp_isrecoverygarage' operator='eq' value='1' />
        <condition attribute='cp_isrecoverygaragecompany' operator='eq' value='1' />
      </filter>
    </filter>
    <link-entity name='cp_customersetting' from='cp_customersettingid' to='cp_customer' link-type='inner' alias='ab'>
      <filter type='and'>
        <condition attribute='cp_customername' operator='eq' value='$customer' />
      </filter>
    </link-entity>
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_incidenttype'>
    <attribute name='cp_incidenttypeid' />
    <attribute name='cp_incidenttypename' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_investigationstatus'>
    <attribute name='cp_investigationstatusid' />
    <attribute name='cp_name' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_lookupfield'>
    <attribute name='cp_lookupfieldid' />
    <attribute name='cp_name' />
    <attribute name='cp_usedin' />
    <attribute name='cp_id' />
    <attribute name='cp_displayname' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_lookupvalue'>
    <attribute name='cp_lookupvalueid' />
    <attribute name='cp_name' />
    <attribute name='cp_lookupfield' />
    <attribute name='cp_displaysequenceno' />
    <attribute name='cp_alternativedisplayname' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_offencecategory'>
    <attribute name='cp_offencecategoryid' />
    <attribute name='cp_name' />
    <attribute name='cp_tickettypeid' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_offencecode'>
    <attribute name='cp_offencecodeid' />
    <attribute name='cp_name' />
    <attribute name='cp_offencecategoryid' />
    <attribute name='cp_offencedescription' />
    <attribute name='cp_personnotmandatory' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='position'>
    <attribute name='name' />
    <attribute name='parentpositionid' />
    <attribute name='description' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_statementtype'>
    <attribute name='cp_statementtypeid' />
    <attribute name='cp_name' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_tactictype'>
    <attribute name='cp_tactictypeid' />
    <attribute name='cp_name' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_useforceoutcome'>
    <attribute name='cp_useforceoutcomeid' />
    <attribute name='cp_name' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_useforcereason'>
    <attribute name='cp_useforcereasonid' />
    <attribute name='cp_name' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_covidoffences'>
    <attribute name='cp_covidoffencesid' />
    <attribute name='cp_offencecode' />
    <attribute name='cp_offencedescription' />
    <attribute name='cp_offenceact' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_statementwitnessneed'>
    <attribute name='cp_statementwitnessneedid' />
    <attribute name='cp_name' />
    <attribute name='cp_displaysequenceno' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_tickettype'>
    <attribute name='cp_tickettypeid' />
    <attribute name='cp_name' />
    <attribute name='cp_displaysequenceno' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_licencecategory'>
    <attribute name='cp_licencecategoryid' />
    <attribute name='cp_licencecategoryname' />
    <attribute name='cp_displaysequenceno' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_taseruseareaback'>
    <attribute name='cp_taseruseareabackid' />
    <attribute name='cp_areaname' />
    <attribute name='cp_displaysequenceno' />
    <attribute name='cp_alternativedisplayname' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_taseruseareafront'>
    <attribute name='cp_taseruseareafrontid' />
    <attribute name='cp_areaname' />
    <attribute name='cp_displaysequenceno' />
    <attribute name='cp_alternativedisplayname' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_useofforceimpactfactor'>
    <attribute name='cp_useofforceimpactfactorid' />
    <attribute name='cp_name' />
    <attribute name='cp_displaysequenceno' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_suddendeathidentificationtype'>
    <attribute name='cp_suddendeathidentificationtypeid' />
    <attribute name='cp_name' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_environment'>
    <attribute name='cp_environmentid' />
    <attribute name='cp_name' />
    <attribute name='cp_workflowenvironmentid' />
    <attribute name='cp_orgurlname' />
    <attribute name='cp_customer' />
    <link-entity name='cp_customersetting' from='cp_customersettingid' to='cp_customer' link-type='inner' alias='ab'>
      <filter type='and'>
        <condition attribute='cp_customername' operator='eq' value='$customer' />
      </filter>
    </link-entity>
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_systemsetting'>
    <attribute name='cp_systemsettingid' />
    <attribute name='cp_name' />
    <attribute name='cp_value' />
    <attribute name='cp_environment' />
    <link-entity name='cp_environment' from='cp_environmentid' to='cp_environment' link-type='inner' alias='ac'>
      <link-entity name='cp_customersetting' from='cp_customersettingid' to='cp_customer' link-type='inner' alias='ad'>
        <filter type='and'>
          <condition attribute='cp_customername' operator='eq' value='$customer' />
        </filter>
      </link-entity>
    </link-entity>
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_itsyourchoicesetup'>
    <attribute name='cp_itsyourchoicesetupid' />
    <attribute name='cp_name' />
    <attribute name='cp_westwarninglettertemplate3' />
    <attribute name='cp_westwarninglettertemplate2' />
    <attribute name='cp_westwarninglettertemplate1' />
    <attribute name='cp_templatesdocumentlibrary' />
    <attribute name='cp_southwarninglettertemplate3' />
    <attribute name='cp_southwarninglettertemplate2' />
    <attribute name='cp_southwarninglettertemplate1' />
    <attribute name='cp_northwarninglettertemplate3' />
    <attribute name='cp_northwarninglettertemplate2' />
    <attribute name='cp_northwarninglettertemplate1' />
    <attribute name='cp_environment' />
    <link-entity name='cp_environment' from='cp_environmentid' to='cp_environment' link-type='inner' alias='ac'>
      <link-entity name='cp_customersetting' from='cp_customersettingid' to='cp_customer' link-type='inner' alias='ad'>
        <filter type='and'>
          <condition attribute='cp_customername' operator='eq' value='$customer' />
        </filter>
      </link-entity>
    </link-entity>
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_statementsetup'>
    <attribute name='cp_statementsetupid' />
    <attribute name='cp_name' />
    <attribute name='cp_templatesdocumentlibrary' />
    <attribute name='cp_statementfronttemplate' />
    <attribute name='cp_statementbacktemplate' />
    <attribute name='cp_environment' />
    <link-entity name='cp_environment' from='cp_environmentid' to='cp_environment' link-type='inner' alias='ac'>
      <link-entity name='cp_customersetting' from='cp_customersettingid' to='cp_customer' link-type='inner' alias='ad'>
        <filter type='and'>
          <condition attribute='cp_customername' operator='eq' value='$customer' />
        </filter>
      </link-entity>
    </link-entity>
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_vehicledocument'>
    <attribute name='cp_vehicledocumentid' />
    <attribute name='cp_name' />
    <attribute name='cp_displaysequenceno' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_covidsetup'>
    <attribute name='cp_covidsetupid' />
    <attribute name='cp_name' />
    <attribute name='cp_environment' />
    <attribute name='cp_covidpdfrecipient' />
    <link-entity name='cp_environment' from='cp_environmentid' to='cp_environment' link-type='inner' alias='ac'>
      <link-entity name='cp_customersetting' from='cp_customersettingid' to='cp_customer' link-type='inner' alias='ad'>
        <filter type='and'>
          <condition attribute='cp_customername' operator='eq' value='$customer' />
        </filter>
      </link-entity>
    </link-entity>
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_covidoffenceverificationtext'>
    <attribute name='cp_covidoffenceverificationtextid' />
    <attribute name='cp_labeltext' />
    <attribute name='cp_order' />
  </entity>
</fetch>"


)


$customerfile = $customer + "CP.zip"
Export-CrmDataPackage -Package $package -ZipPath $PSScriptRoot\$customerfile

$fileName = $customer+"CPVersionDate.txt"
Remove-Item $fileName
New-Item $fileName
(Get-Date).ToString("yyyy-MM-dd HH:mm:ss") | Out-File $PSScriptRoot\$fileName 