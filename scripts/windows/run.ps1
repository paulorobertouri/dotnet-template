$ErrorActionPreference = "Stop"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$ScriptDirectory\..\.."

Write-Host "Starting API in Development mode..."
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src\DotnetTemplate.Api\DotnetTemplate.Api.csproj
