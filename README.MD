# dotnet-template

> Production-ready .NET 10 Web API template with Clean Architecture, JWT auth, and a full test suite.

## Stack

- .NET 10 / ASP.NET Core
- Scalar API docs
- NUnit 4 + FluentAssertions + Moq
- dotnet format

## Endpoints

- `GET /health`
- `GET /docs`
- `GET /v1/public`
- `GET /v1/customer`
- `GET /v1/customer/{id}`
- `GET /v1/auth/login`
- `GET /v1/private`

## JWT Contract

Login returns:

- Body: `{"token":"..."}`
- Header: `Authorization: Bearer <token>`
- Header: `X-JWT-Token: <token>`

## Commands

- `make install` — restore dependencies
- `make build` — build the solution
- `make run` — run the API
- `make test` — run all tests
- `make test-unit` — run unit tests
- `make test-e2e` — run browser e2e tests
- `make lint` — run format check
- `make format` — format the code
- `make clean` — clean build artifacts

## Environment Variables

- `JWT_SECRET=your-super-secret-jwt-key-at-least-32-characters-long`
- `Jwt__Issuer=dotnet-template`
- `Jwt__Audience=dotnet-template`
- `Jwt__ExpirationSeconds=3600`

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
│   └── DotnetTemplate.Tests.Integration/ # WebApplicationFactory integration tests
├── docker/
├── scripts/
│   ├── ubuntu/
│   └── windows/
└── Makefile
```

## Architecture

```text
HTTP -> API Routes -> JWT Middleware -> Services -> Repositories -> Domain
```

## Quality Gates

- Format check must pass.
- Unit and integration tests must pass.
- Playwright e2e tests must pass.

## License

MIT — see [LICENSE](LICENSE).
