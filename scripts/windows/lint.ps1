$ErrorActionPreference = "Stop"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$ScriptDirectory\..\.."

Write-Host "Verifying formatting (no changes allowed)..."
dotnet format DotnetTemplate.sln --verify-no-changes
Write-Host "Lint passed."
