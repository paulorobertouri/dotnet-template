#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/../../"

IMAGE="dotnet-template:e2e"
CONTAINER="dotnet-template-e2e"
PORT=19080
JWT_SECRET_FOR_TESTS="test-secret-key-at-least-32-characters"

# Ensure idempotency if a previous run left a container behind
docker rm -f "$CONTAINER" >/dev/null 2>&1 || true

# Build production image
docker build -f docker/build.Dockerfile -t "$IMAGE" .

# Start container
container_id=$(docker run -d --name "$CONTAINER" \
  -e "Jwt__Secret=${JWT_SECRET_FOR_TESTS}" \
  -e "Jwt__Issuer=dotnet-template" \
  -e "Jwt__Audience=dotnet-template" \
  -p "${PORT}:8080" "$IMAGE")
trap "docker rm -f $container_id" EXIT

# Wait for the server to be ready
ready=0
for _ in $(seq 1 60); do
  if curl -sf "http://127.0.0.1:${PORT}/health" > /dev/null; then
    ready=1
    break
  fi
  sleep 1
done

if [ "$ready" -ne 1 ]; then
  echo "Server did not become ready at /health within 60s"
  docker logs "$container_id" || true
  exit 1
fi

# ── health ───────────────────────────────────────────────────────────────────
echo "Testing /health..."
curl -fsS "http://127.0.0.1:${PORT}/health" > /dev/null

# ── docs ─────────────────────────────────────────────────────────────────────
echo "Testing /docs..."
curl -fsS "http://127.0.0.1:${PORT}/docs" > /dev/null

# ── public ───────────────────────────────────────────────────────────────────
echo "Testing /v1/public..."
curl -fsS "http://127.0.0.1:${PORT}/v1/public" > /dev/null

# ── customers ────────────────────────────────────────────────────────────────
echo "Testing /v1/customer..."
curl -fsS "http://127.0.0.1:${PORT}/v1/customer" > /dev/null

echo "Testing /v1/customer/1..."
curl -fsS "http://127.0.0.1:${PORT}/v1/customer/1" > /dev/null

echo "Testing /v1/customer/999 (expecting 404)..."
status=$(curl -o /dev/null -s -w "%{http_code}" "http://127.0.0.1:${PORT}/v1/customer/999")
if [ "$status" != "404" ]; then
  echo "Expected 404 for unknown customer, got $status"
  exit 1
fi

# ── login ─────────────────────────────────────────────────────────────────────
echo "Testing /v1/auth/login..."
raw_response=$(curl -isS "http://127.0.0.1:${PORT}/v1/auth/login")

if ! echo "$raw_response" | grep -iq "^x-jwt-token:"; then
  echo "X-JWT-Token header was not returned"
  exit 1
fi

token=$(curl -fsS "http://127.0.0.1:${PORT}/v1/auth/login" \
  | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

if [ -z "$token" ]; then
  echo "JWT token not found in response body"
  exit 1
fi

# ── private ───────────────────────────────────────────────────────────────────
echo "Testing /v1/private without token (expecting 401)..."
status401=$(curl -o /dev/null -s -w "%{http_code}" "http://127.0.0.1:${PORT}/v1/private")
if [ "$status401" != "401" ]; then
  echo "Expected 401 for unauthorized private access, got $status401"
  exit 1
fi

echo "Testing /v1/private with token..."
curl -fsS -H "Authorization: Bearer ${token}" \
  "http://127.0.0.1:${PORT}/v1/private" > /dev/null

echo "All Docker+curl tests passed!"
