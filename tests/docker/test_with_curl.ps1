$ErrorActionPreference = "Stop"

$image     = "dotnet-template:e2e"
$container = "dotnet-template-e2e"
$port      = 19080

try {
    docker build -f docker/build.Dockerfile -t $image .

    docker run -d --name $container -p "${port}:8080" $image | Out-Null

    # Wait for server to be ready
    $ready = $false
    for ($i = 0; $i -lt 30; $i++) {
        try {
            Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/health" | Out-Null
            $ready = $true
            break
        } catch {
            Start-Sleep -Seconds 1
        }
    }
    if (-not $ready) { throw "Server did not become ready in time" }

    # health
    Write-Host "Testing /health..."
    Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/health" | Out-Null

    # public
    Write-Host "Testing /v1/public..."
    Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/v1/public" | Out-Null

    # customers
    Write-Host "Testing /v1/customer..."
    Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/v1/customer" | Out-Null

    Write-Host "Testing /v1/customer/1..."
    Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/v1/customer/1" | Out-Null

    Write-Host "Testing /v1/customer/999 (expecting 404)..."
    try {
        Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/v1/customer/999" | Out-Null
        throw "Expected 404 for unknown customer"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 404) { throw }
    }

    # login
    Write-Host "Testing /v1/auth/login..."
    $login = Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/v1/auth/login"

    $headerToken = $login.Headers["X-JWT-Token"]
    if (-not $headerToken) { throw "X-JWT-Token header was not returned" }

    $token = ($login.Content | ConvertFrom-Json).token
    if (-not $token) { throw "JWT token not found in response body" }

    # private without token
    Write-Host "Testing /v1/private without token (expecting 401)..."
    try {
        Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:${port}/v1/private" | Out-Null
        throw "Expected 401 for unauthenticated private access"
    } catch {
        if ($_.Exception.Response.StatusCode -ne 401) { throw }
    }

    # private with token
    Write-Host "Testing /v1/private with token..."
    Invoke-WebRequest -UseBasicParsing `
        -Headers @{ Authorization = "Bearer $token" } `
        "http://127.0.0.1:${port}/v1/private" | Out-Null

    Write-Host "All Docker+curl tests passed!"
}
finally {
    docker rm -f $container 2>$null | Out-Null
}
