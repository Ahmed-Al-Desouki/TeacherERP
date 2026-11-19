param(
    [string]$ApiBaseUrl = "https://smart-teachererp.runasp.net",
    [switch]$OpenBrowser
)

$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$baseUrl = $ApiBaseUrl.TrimEnd('/')
$normalUrl = "$baseUrl/api/paymob/return"
$tamperedUrl = "$normalUrl`?success=true&status=paid&amount_cents=1"

function Invoke-ReturnPageRequest {
    param([Parameter(Mandatory = $true)][string]$Uri)

    try {
        $response = Invoke-WebRequest -Uri $Uri -Method Get -UseBasicParsing -TimeoutSec 30
        return [pscustomobject]@{
            StatusCode = [int]$response.StatusCode
            ContentType = [string]$response.Headers["Content-Type"]
            Body = [string]$response.Content
        }
    }
    catch {
        $webResponse = $_.Exception.Response
        if ($null -eq $webResponse) {
            throw "Could not reach the API: $($_.Exception.Message)"
        }

        $body = ""
        $stream = $webResponse.GetResponseStream()
        if ($null -ne $stream) {
            $reader = New-Object System.IO.StreamReader($stream)
            try {
                $body = $reader.ReadToEnd()
            }
            finally {
                $reader.Dispose()
                $stream.Dispose()
            }
        }

        return [pscustomobject]@{
            StatusCode = [int]$webResponse.StatusCode
            ContentType = [string]$webResponse.ContentType
            Body = $body
        }
    }
}

function Show-Response {
    param(
        [Parameter(Mandatory = $true)][string]$Title,
        [Parameter(Mandatory = $true)]$Response
    )

    Write-Host "`n--- $Title ---" -ForegroundColor Cyan
    Write-Host "HTTP: $($Response.StatusCode)"
    Write-Host "Content-Type: $($Response.ContentType)"
    Write-Host "Response body:"
    Write-Output $Response.Body
}

Write-Host "Paymob browser-return endpoint check"
Write-Host "API: $baseUrl"
Write-Host "No login, payment, or credentials are used by this test."

try {
    $normal = Invoke-ReturnPageRequest -Uri $normalUrl
    Show-Response -Title "Normal return request" -Response $normal

    $tampered = Invoke-ReturnPageRequest -Uri $tamperedUrl
    Show-Response -Title "Return request with forged success parameters" -Response $tampered

    $failures = [System.Collections.Generic.List[string]]::new()
    if ($normal.StatusCode -ne 200) {
        $failures.Add("Expected HTTP 200 from $normalUrl but received $($normal.StatusCode). The updated API may not be deployed yet.")
    }
    if ($tampered.StatusCode -ne 200) {
        $failures.Add("Expected HTTP 200 for the query-parameter check but received $($tampered.StatusCode).")
    }
    if ($normal.ContentType -notmatch "text/html") {
        $failures.Add("Expected a text/html response but received '$($normal.ContentType)'.")
    }
    if ($normal.Body -notmatch "Paymob") {
        $failures.Add("The page is missing its Paymob payment-return message.")
    }
    if ($normal.Body.Trim() -ne $tampered.Body.Trim()) {
        $failures.Add("The response changed when success/status query parameters were added; inspect the return page before trusting it.")
    }

    if ($failures.Count -gt 0) {
        Write-Host "`nTEST FAILED" -ForegroundColor Red
        foreach ($failure in $failures) { Write-Host "- $failure" -ForegroundColor Red }
        exit 1
    }

    Write-Host "`nTEST PASSED: both requests returned the same informational page; query parameters did not prove payment success." -ForegroundColor Green
    if ($OpenBrowser) {
        Start-Process $normalUrl
        Write-Host "Opened the normal return page in your default browser."
    }
}
catch {
    Write-Host "`nTEST COULD NOT COMPLETE: $($_.Exception.Message)" -ForegroundColor Red
    exit 2
}
