#!/usr/bin/env bash
set -euo pipefail
echo "Starting .NET services..."
docker compose up -d
sleep 10
curl -f http://localhost:8000/health && echo "Healthy!" || echo "Check failed."
