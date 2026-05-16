$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)

Set-Location $ProjectRoot

Write-Output "Starting frontend dev server..."
docker compose up -d
Write-Output "Starting frontend dev server..."
sleep 10
Write-Output "Starting frontend dev server..."
curl -f http://localhost:8000/health && echo "Healthy!" || echo "Check failed."
