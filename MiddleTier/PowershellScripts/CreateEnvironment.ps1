
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
New-AzStorageQueue -Name "suddendeathqueue" -Context $Context
New-AzStorageContainer -Name "pnb" -Context $Context

New-AzAppServicePlan -ResourceGroupName $environmentName-$guid -Name $EnvironmentName"-"$guid"-AppServicePlan" -Location "UK South"

New-AzWebApp -ResourceGroupName $environmentName-$guid -Name $EnvironmentName"-"$guid"-APIWebApp" -Location "UK South" -AppServicePlan $EnvironmentName"-"$guid"-AppServicePlan"
Set-AzWebApp -ResourceGroupName $environmentName-$guid -Name $EnvironmentName"-"$guid"-APIWebApp" -NetFrameworkVersion 5.0 -AppSettings @{"ASPNETCORE_ENVIRONMENT" ="$environmentType"}


New-AzFunctionAppPlan -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-FunctionAppPlan" -Sku "EP2" -Location "UK South" -WorkerType Windows

New-AzFunctionApp  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -StorageAccountName $storageAccountName  -Runtime "dotnet" -planName $environmentName-$guid"-FunctionAppPlan" -FunctionsVersion 3 -RuntimeVersion 3
#New-AzFunctionApp  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -StorageAccountName $storageAccountName  -Runtime "dotnet" -Location "UK South"  -FunctionsVersion 3 -RuntimeVersion 3

Update-AzFunctionAppSetting  -ResourceGroupName $environmentName-$guid -Name $environmentName-$guid"-QueueListeners" -AppSetting @{"FUNCTIONS_WORKER_RUNTIME" ="dotnet-isolated"} -Force
