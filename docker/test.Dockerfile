# ── Test stage ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS test
WORKDIR /app

COPY DotnetTemplate.sln ./
COPY src/DotnetTemplate.Domain/DotnetTemplate.Domain.csproj       src/DotnetTemplate.Domain/
COPY src/DotnetTemplate.Application/DotnetTemplate.Application.csproj  src/DotnetTemplate.Application/
COPY src/DotnetTemplate.Api/DotnetTemplate.Api.csproj             src/DotnetTemplate.Api/
COPY tests/DotnetTemplate.Tests.Unit/DotnetTemplate.Tests.Unit.csproj               tests/DotnetTemplate.Tests.Unit/
COPY tests/DotnetTemplate.Tests.Integration/DotnetTemplate.Tests.Integration.csproj tests/DotnetTemplate.Tests.Integration/

RUN dotnet restore DotnetTemplate.sln

COPY src/ src/
COPY tests/ tests/

ENV Jwt__Secret=docker-test-secret-key-at-least-32-characters \
    Jwt__Issuer=dotnet-template \
    Jwt__Audience=dotnet-template \
    Jwt__ExpirationSeconds=3600

CMD ["dotnet", "test", "DotnetTemplate.sln", "--no-restore", "--logger", "console;verbosity=normal"]
