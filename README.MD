# dotnet-template

> Production-ready .NET 10 Web API template with Clean Architecture, JWT auth, and a full test suite.

![CI](https://github.com/your-org/dotnet-template/actions/workflows/ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-blue)

## Features

- **REST API** — versioned routes (`/v1/*`) with public, authenticated, and customer endpoints
- **JWT authentication** — HMAC-SHA256, configurable expiry, validated on every protected request
- **Clean Architecture** — Domain → Application → Api layers with dependency inversion
- **In-memory repository** — swap for any persistence layer without touching business logic
- **Scalar API docs** — interactive API reference at `/docs`
- **Unit + integration tests** — NUnit 4, FluentAssertions, Moq, WebApplicationFactory
- **Docker** — multi-stage Alpine build, non-root user, `docker-compose` included
- **Curl E2E tests** — full endpoint coverage via shell/PS1 scripts
- **GitHub Actions CI** — format check, build, unit tests, integration tests on every push
- **dotnet format** — consistent code style enforced in CI

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Docker & docker compose (optional, for container workflow)
- `make` (optional, for Makefile shortcuts)

## Quick Start

```bash
git clone https://github.com/your-org/dotnet-template.git
cd dotnet-template
cp .env.example .env   # edit JWT_SECRET (min 32 chars)
```

### Run locally

```bash
make install            # dotnet restore
make run                # dotnet run (http://localhost:8080)
```

Or without make:

```bash
dotnet restore
Jwt__Secret="a-secret-at-least-32-characters-long" \
  dotnet run --project src/DotnetTemplate.Api
```

### Run with Docker

```bash
cp .env.example .env    # set JWT_SECRET
make docker-build       # build image
docker compose up       # start on http://localhost:8080
```

## API Reference

| Method | Route               | Auth     | Description              |
|--------|---------------------|----------|--------------------------|
| GET    | `/health`           | None     | Health check             |
| GET    | `/v1/public`        | None     | Public endpoint          |
| GET    | `/v1/auth/login`    | None     | Issue a JWT token        |
| GET    | `/v1/private`       | Bearer   | Protected endpoint       |
| GET    | `/v1/customer`      | None     | List all customers       |
| GET    | `/v1/customer/{id}` | None     | Get customer by ID       |

Interactive docs: [http://localhost:8080/docs](http://localhost:8080/docs)

### Example: Login

```bash
TOKEN=$(curl -s http://localhost:8080/v1/auth/login | jq -r .token)
```

### Example: Protected route

```bash
curl -H "Authorization: Bearer $TOKEN" http://localhost:8080/v1/private
```

## Configuration

All JWT settings can be provided as environment variables (recommended) or in `appsettings.json`
(development only — never commit secrets).

| Variable                   | Default              | Description                    |
|----------------------------|----------------------|--------------------------------|
| `JWT_SECRET` / `Jwt__Secret` | *(required)*       | Signing key, min 32 characters |
| `Jwt__Issuer`              | `dotnet-template`    | JWT issuer claim               |
| `Jwt__Audience`            | `dotnet-template`    | JWT audience claim             |
| `Jwt__ExpirationSeconds`   | `3600`               | Token lifetime in seconds      |

Copy `.env.example` to `.env` and fill in `JWT_SECRET` before running.

## Testing

```bash
make test               # unit + integration tests
make test-coverage      # with HTML coverage report
```

Or without make:

```bash
dotnet test DotnetTemplate.sln
```

### Docker curl E2E tests

```bash
docker compose up -d
make docker-curl-test   # runs tests/docker/test_with_curl.sh
docker compose down
```

## Project Structure

```text
dotnet-template/
├── src/
│   ├── DotnetTemplate.Api/          # ASP.NET Core Web API
│   │   ├── Controllers/             # PublicController, AuthController, CustomerController
│   │   └── Infrastructure/          # InMemoryCustomerRepository
│   ├── DotnetTemplate.Application/  # Use cases, interfaces, services
│   └── DotnetTemplate.Domain/       # Entities, value objects
├── tests/
│   ├── DotnetTemplate.Tests.Unit/        # NUnit unit tests
│   ├── DotnetTemplate.Tests.Integration/ # WebApplicationFactory integration tests
│   └── docker/                          # Curl-based E2E tests
├── docker/                          # Dockerfiles (build + test)
├── scripts/
│   ├── ubuntu/                      # Bash scripts
│   └── windows/                     # PowerShell equivalents
├── .github/workflows/ci.yml         # GitHub Actions CI
├── docker-compose.yml
├── Makefile
└── DotnetTemplate.sln
```

## Renaming the Template

```bash
bash scripts/rename-template.sh my-new-project
# or on Windows:
.\scripts\rename-template.ps1 my-new-project
```

The script replaces `DotnetTemplate` → `MyNewProject` and `dotnet-template` → `my-new-project`
in all file contents and file names.

## Contributing

1. Fork and clone
2. Create a feature branch
3. Run `make lint` before pushing
4. Open a pull request — CI must be green

## License

[MIT](LICENSE)
