$ErrorActionPreference = "Stop"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$ScriptDirectory\..\.."

Write-Host "Applying dotnet format..."
dotnet format DotnetTemplate.sln
Write-Host "Format applied."
