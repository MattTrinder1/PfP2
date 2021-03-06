
#Install-Module -Name Az -Scope CurrentUser -Repository PSGallery -Force 
#connect-azaccount 
$guid = New-Guid
$guid = $guid.ToString().Substring(32)
#set the environment name here
$environmentName = Read-Host -Prompt "Enter the name for the new environment"
$environmentType = Read-Host -Prompt "Enter the new environment type (dev,systemtest etc)"

New-AzResourceGroup -Name $environmentName-$guid -Location "UK South"
$storageAccountName = -join($environmentName.ToLower(),$guid,"stg")
New-AzStorageAccount -ResourceGroupName $environmentName-$guid -Name $storageAccountName  -SkuName "Standard_GRS" -Location "UK South"
$Context = New-AzStorageContext -StorageAccountName $storageAccountName -UseConnectedAccount
New-AzStorageQueue -Name "onlinecheckqueue" -Context $Context
New-AzStorageQueue -Name "pocketnotebookqueue" -Context $Context
New-AzStorageQueue -Name "pocketnotebookqueue-archive" -Context $Context
New-AzStorageQueue -Name "pocketnotebookblobqueue" -Context $Context
New-AzStorageQueue -Name "pocketnotebookblobqueue-archive" -Context $Context
New-AzStorageQueue -Name "suddendeathqueue" -Context $Context
New-AzStorageQueue -Name "suddendeathqueue-archive" -Context $Context
New-AzStorageQueue -Name "suddendeathblobqueue" -Context $Context
New-AzStorageQueue -Name "suddendeathblobqueue-archive" -Context $Context

New-AzStorageContainer -Name "pnb" -Context $Context
New-AzStorageContainer -Name "suddendeath" -Context $Context

New-AzAppServicePlan -ResourceGroupName $environmentName-$guid -Name $EnvironmentName"-"$guid"-AppServicePlan" -Location "UK South"

New-AzWebApp -ResourceGroupName $environmentName-$guid -Name $EnvironmentName"-"$guid"-APIWebApp" -Location "UK South" -AppServicePlan $EnvironmentName"-"$guid"-AppServicePlan"
Set-AzWebApp -ResourceGroupName $environmentName-$guid -Name $EnvironmentName"-"$guid"-APIWebApp" -NetFrameworkVersion 5.0 -AppSettings @{"ASPNETCORE_ENVIRONMENT" ="$environmentType";"DvlaConnection:ApiKey"="OeJHkYTswJK8Rlun5T5X6pIequYj0lC5XlsaT1xj";"DvlaConnection:AuthenticateUri"="https://uat.driver-vehicle-licensing.api.gov.uk/thirdparty-access/v1/authenticate";"DvlaConnection:Password"="viWUCJJD4YgAlUXpaVJ@R54C";"DvlaConnection:RetrieveUri"="https://uat.driver-vehicle-licensing.api.gov.uk/driver-photo-at-the-roadside/v1/drivers/driver-details/s";"DvlaConnection:TestDriverNumber"="AMRSH652170JX8PD";"DvlaConnection:TokenValidMinutes"="55";"DvlaConnection:UserName"="cumbriapolice"}

New-AzWebApp -ResourceGroupName $environmentName-$guid -Name $EnvironmentName"-"$guid"-MockNDIPNCAPI" -Location "UK South" -AppServicePlan $EnvironmentName"-"$guid"-AppServicePlan"

New-AzFunctionAppPlan -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-FunctionAppPlan" -Sku "EP2" -Location "UK South" -WorkerType Windows

New-AzFunctionApp  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -StorageAccountName $storageAccountName  -Runtime "dotnet" -planName $environmentName-$guid"-FunctionAppPlan" -FunctionsVersion 3 -RuntimeVersion 3
#New-AzFunctionApp  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -StorageAccountName $storageAccountName  -Runtime "dotnet" -Location "UK South"  -FunctionsVersion 3 -RuntimeVersion 3

Update-AzFunctionAppSetting  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -AppSetting @{"FUNCTIONS_WORKER_RUNTIME" ="dotnet-isolated"} -Force
Update-AzFunctionAppSetting  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -AppSetting @{"APIUrl" ="https://$environmentName-$guid-apiwebapp.azurewebsites.net/api"} -Force
Update-AzFunctionAppSetting  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -AppSetting @{"APIIntegrationKey" ="[IntegrationKeyGUID]"} -Force
