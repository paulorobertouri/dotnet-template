# Usage: .\rename-template.ps1 MyNewProject
# Renames every occurrence of "DotnetTemplate" and "dotnet-template" to your project name.
param(
    [Parameter(Mandatory = $true)]
    [string]$NewName
)
$ErrorActionPreference = "Stop"

# Build Pascal-case version (kebab-case -> PascalCase)
$Pascal = ($NewName -split '-' | ForEach-Object {
    $_.Substring(0,1).ToUpper() + $_.Substring(1)
}) -join ''

$ScriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
Set-Location $ProjectRoot

Write-Host "Renaming project to: $NewName (Pascal: $Pascal)"

$extensions = @('*.cs','*.csproj','*.sln','*.json','*.yml','*.yaml',
                 '*.md','Makefile','Dockerfile','*.sh','*.ps1')

$excludeDirs = @('.git','bin','obj','node_modules')

function ShouldExclude($path) {
    foreach ($d in $excludeDirs) {
        if ($path -match [regex]::Escape($d)) { return $true }
    }
    return $false
}

# Replace in file contents
foreach ($ext in $extensions) {
    Get-ChildItem -Recurse -Filter $ext -ErrorAction SilentlyContinue | ForEach-Object {
        if (ShouldExclude $_.FullName) { return }
        $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
        if ($null -eq $content) { return }
        $updated = $content -replace 'DotnetTemplate', $Pascal -replace 'dotnet-template', $NewName
        if ($updated -ne $content) {
            Set-Content $_.FullName $updated -NoNewline
            Write-Host "  Updated: $($_.FullName)"
        }
    }
}

# Rename files containing "DotnetTemplate"
Get-ChildItem -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -match 'DotnetTemplate' -and -not (ShouldExclude $_.FullName) } |
    Sort-Object -Property FullName -Descending |
    ForEach-Object {
        $newBase = $_.Name -replace 'DotnetTemplate', $Pascal
        $newPath = Join-Path $_.DirectoryName $newBase
        Rename-Item $_.FullName $newPath
        Write-Host "  Renamed: $($_.FullName) -> $newPath"
    }

Write-Host ""
Write-Host "Done. Verify the solution still builds:"
Write-Host "  dotnet build ${Pascal}.sln"
