#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(dirname "$(dirname "$SCRIPT_DIR")")"
cd "$PROJECT_ROOT"

echo "Starting API in Development mode..."
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --project src/DotnetTemplate.Api/DotnetTemplate.Api.csproj
