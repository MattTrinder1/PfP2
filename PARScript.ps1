$pwd="viWUCJJD4YgAlUXpaVJ@R54C"
$userName = "cumbriapolice"
$apiKey = "OeJHkYTswJK8Rlun5T5X6pIequYj0lC5XlsaT1xj"

$token = Invoke-WebRequest `
            -Method POST `
            -Uri  https://uat.driver-vehicle-licensing.api.gov.uk/thirdparty-access/v1/authenticate `
            -Body (@{"userName"=$userName;"password"=$pwd}|ConvertTo-Json) `


$jwt = (ConvertFrom-Json( $token.Content)).psobject.properties.value

Write-Host $jwt

$driverNumber = "AMRSH652170JX8PD"

Invoke-WebRequest `
            -Method POST `
            -Uri  https://uat.driver-vehicle-licensing.api.gov.uk/driver-photo-at-the-roadside/v1/drivers/driver-details/s `
            -Headers @{"Authorization" = $jwt;"x-api-key" =$apiKey} `
            -Body (@{"driverNumber"=$driverNumber}|ConvertTo-Json) `
            -ContentType application/json


