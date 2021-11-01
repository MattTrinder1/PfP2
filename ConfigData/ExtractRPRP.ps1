#Install-Module -Name Microsoft.Xrm.DevOps.Data.PowerShell

param (
[string]$customer
)

$conn = Get-CrmConnection -ConnectionString "RequireNewInstance=True;AuthType=ClientSecret;ClientId=83af4fce-10c3-4409-a43d-d67e300424aa;ClientSecret=H.ARok4~mmJl_lfU~nfE9JqE.7pUE26u.t;Url=https://policeproductdev.crm11.dynamics.com"




$package = Get-CrmDataPackage -Conn $conn -Fetches @("
<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_rprptype'>
    <attribute name='cp_rprptypeid' />
    <attribute name='cp_name' />
    <attribute name='cp_timeoutdaysschedulemeeting' />
    <attribute name='cp_timeoutdaysparticipantresponse' />
    <attribute name='cp_timeoutdaysmeetinglapsed' />
    <attribute name='cp_timeoutdaysfollowuplapsed' />
    <attribute name='cp_timeoutdaysdraft' />
    <attribute name='cp_rolerequiredtocreate' />
    <attribute name='cp_reviewerguidanceurl' />
    <attribute name='cp_reviewercancomplete' />
    <attribute name='cp_referencelabel' />
    <attribute name='cp_referencedefault' />
    <attribute name='cp_referencebehaviour' />
    <attribute name='cp_participantguidanceurl' />
    <attribute name='cp_participantacceptancereminderhours' />
    <attribute name='cp_owningteam' />
    <attribute name='cp_notificationemailaddresses' />
    <attribute name='cp_emailfromqueue' />
    <order attribute='cp_name' descending='false' />
  </entity>
</fetch>"
,
"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cp_rprpcategory'>
    <attribute name='cp_rprpcategoryid' />
    <attribute name='cp_name' />
    <attribute name='cp_type' />
    <order attribute='cp_name' descending='false' />
  </entity>
</fetch>"
)

$customerfile = $customer + "RPRP.zip"
Export-CrmDataPackage -Package $package -ZipPath $PSScriptRoot\$customerfile

$fileName = $customer+"RPRPVersionDate.txt"
Remove-Item $fileName
New-Item $fileName
(Get-Date).ToString("yyyy-MM-dd HH:mm:ss") | Out-File $PSScriptRoot\$fileName 