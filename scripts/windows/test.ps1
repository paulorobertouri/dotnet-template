$ErrorActionPreference = "Stop"
$ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$ScriptDirectory\..\.."

Write-Host "Running all tests..."
dotnet test DotnetTemplate.sln --logger "console;verbosity=normal"
Write-Host "All tests passed."
