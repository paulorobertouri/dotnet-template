$ErrorActionPreference = "Stop"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$ScriptDirectory\..\.."

Write-Host "Building solution in Release..."
dotnet build DotnetTemplate.sln -c Release
Write-Host "Build succeeded."
