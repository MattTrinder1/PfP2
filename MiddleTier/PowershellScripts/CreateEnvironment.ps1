
#Install-Module -Name Az -Scope CurrentUser -Repository PSGallery -Force 
#connect-azaccount 

#set the environment name here
$environmentName = Read-Host -Prompt "Enter the name for the new environment"
$environmentType = Read-Host -Prompt "Enter the new environment type (dev,systemtest etc)"

New-AzResourceGroup -Name $environmentName -Location "UK South"
New-AzStorageAccount -ResourceGroupName $environmentName -Name $environmentName.ToLower() -SkuName "Standard_GRS" -Location "UK South"
$Context = New-AzStorageContext -StorageAccountName $environmentName -UseConnectedAccount
New-AzStorageQueue -Name "onlinecheckqueue" -Context $Context
New-AzStorageQueue -Name "pocketnotebookqueue" -Context $Context
New-AzStorageContainer -Name "pnb" -Context $Context

New-AzAppServicePlan -ResourceGroupName $environmentName -Name $EnvironmentName"AppServicePlan" -Location "UK South"

New-AzWebApp -ResourceGroupName $environmentName -Name $EnvironmentName"APIWebApp" -Location "UK South" -AppServicePlan $environmentName"AppServicePlan" 
Set-AzWebApp -ResourceGroupName $environmentName -Name $EnvironmentName"APIWebApp" -NetFrameworkVersion 5.0 -AppSettings @{"ASPNETCORE_ENVIRONMENT" ="$environmentType"}


New-AzFunctionAppPlan -ResourceGroupName $environmentName -Name $environmentName"FunctionAppPlan" -Sku "EP2" -Location "UK South" -WorkerType Windows
New-AzFunctionApp -ResourceGroupName $environmentName -Name $environmentName"QueueListeners" -StorageAccountName $environmentName.ToLower()  -Runtime "dotnet" -planName $environmentName"FunctionAppPlan" -FunctionsVersion 3 -RuntimeVersion 3

Update-AzFunctionAppSetting  -ResourceGroupName $environmentName -Name $environmentName"QueueListeners" -AppSetting @{"FUNCTIONS_WORKER_RUNTIME" ="dontnet-isolated"} -Force
