param (
[string]$folderName,
[string]$mapName,
[string]$skipExport,
[bool] $canvas,
[string] $appName,
[bool] $flow,
[string] $prefix

)


$tempFolderName = $PSScriptRoot+"\temp"

$solutionName = $prefix + $folderName

$folderName = "../Schema/" + $folderName + $prefix

#$connString = "RequireNewInstance=True;AuthType=ClientSecret;ClientId=83af4fce-10c3-4409-a43d-d67e300424aa;ClientSecret=H.ARok4~mmJl_lfU~nfE9JqE.7pUE26u.t;Url=https://policeproductdev.crm11.dynamics.com"
$connString = "RequireNewInstance=True;AuthType=OAuth;UserName=pfp.service@tisski.com;Password=Direct.Drop.Bribe.Earn.2;Url=https://policeproductdev.crm11.dynamics.com;AppId=83af4fce-10c3-4409-a43d-d67e300424aa;RedirectUri=app://83af4fce-10c3-4409-a43d-d67e300424aa;LoginPrompt=Never"


if (!$skipExport)
{
    $skipExport="dontskip"
}

function incrementVersionNumber
{
    Param([string] $solutionVersion)
    $versions = $solutionVersion.Split(".")
    $last = ([int]$versions[3])
    $last +=1
    $version = $versions[0]+'.' + $versions[1]+'.' + $versions[2]+'.' + $last
    return $version

}

function Format-Json([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
  $indent = 0;
  ($json -Split '\n' |
    % {
      if ($_ -match '[\}\]]') {
        # This line contains  ] or }, decrement the indentation level
        $indent--
      }
      $line = (' ' * $indent * 2) + $_.TrimStart().Replace(':  ', ': ')
      if ($_ -match '[\{\[]') {
        # This line contains [ or {, increment the indentation level
        $indent++
      }
      $line
  }) -Join "`n"
}


Install-Module Newtonsoft.Json -Scope CurrentUser
Import-Module Newtonsoft.Json 


function prettifyJson ($jsonfile) {
  (get-content $jsonfile) | convertfrom-json | convertto-json -depth 100 | Format-Json |    set-content $jsonfile
  
}


remove-item "$tempFolderName\$solutionName*.zip"



$solution = Get-XrmSolution -UniqueSolutionName $solutionName -ConnectionString $connString
Write-Host $solution.Version
$newVersionNumber = incrementVersionNumber ($solution.Version)
Write-host $newVersionNumber
Set-XrmSolutionVersion -ConnectionString $connString -SolutionName $solutionName -Version $newVersionNumber

Export-XrmSolution -UniqueSolutionName $solutionName -Managed $true -ConnectionString $connString -OutputFolder $tempFolderName -Timeout 240

$solutionFile = Get-Item -Path $tempFolderName\$solutionName*.zip


$mapFile = $PSScriptRoot + "\" + $folderName + "Map.xml"
$d = $skipExport.CompareTo("skip")
Write-Host $d
if ($d -ne 0)
{
    remove-item $PSScriptRoot\$folderName -Force -Recurse

    #unpack the solution

    if (Test-Path $mapFile)
    {
    & "$PSScriptRoot\..\365Tools\Tools\CoreTools\SolutionPackager.exe" /a:extract /p:both /map:"$mapFile" /z:"$tempFolderName\$solutionName"_$version.zip"" /f:"$PSScriptRoot\$folderName" /c /ad:yes /e:verbose
    }
    else
    {
    & "$PSScriptRoot\..\365Tools\Tools\CoreTools\SolutionPackager.exe" /a:extract /p:managed  /z:"$solutionFile" /f:"$PSScriptRoot\$folderName\unpacked" /c /ad:yes /e:verbose
    }
}


#unpack the canvas app msapp file

if ($canvas)
{
    Write-Host $appName
    $appFile =  "$PSScriptRoot\$folderName\unpacked\CanvasApps\cp_" + $appName +"_DocumentUri.msapp"
    Write-Host $appFile
    & "$PSScriptRoot\..\365Tools\Tools\pasopa\pasopa.exe" -unpack $appFile "$PSScriptRoot\$folderName\unpackedapp"



    Write-Host $PSScriptRoot\$folderName\unpackedapp\Src\EditorState\*.json
    
   # remove-item $PSScriptRoot\$folderName\unpackedapp\Src\EditorState\* -Include *.json #-Verbose
}

#format workflow json

if ($flow)
{
    Get-Item $PSScriptRoot\$folderName\unpacked\Workflows\* -Include *.json |ForEach-Object -Process {prettifyJson $_}
}


remove-item $solutionFile

#read-host
