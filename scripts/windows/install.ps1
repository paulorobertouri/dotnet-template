$ErrorActionPreference = "Stop"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$ScriptDirectory\..\.."

Write-Host "Restoring NuGet packages..."
dotnet restore DotnetTemplate.sln
Write-Host "Done."
