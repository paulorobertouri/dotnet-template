#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(dirname "$(dirname "$SCRIPT_DIR")")"
cd "$PROJECT_ROOT"

echo "Running all tests..."
dotnet test DotnetTemplate.sln --logger "console;verbosity=normal"
echo "All tests passed."
