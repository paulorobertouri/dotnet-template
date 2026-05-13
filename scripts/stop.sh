#!/usr/bin/env bash
set -euo pipefail
echo "Stopping .NET services..."
docker compose down
