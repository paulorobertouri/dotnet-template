#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(dirname "$(dirname "$SCRIPT_DIR")")"
cd "$PROJECT_ROOT"

echo "Verifying formatting (no changes allowed)..."
dotnet format DotnetTemplate.sln --verify-no-changes
echo "Lint passed."
