# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

COPY DotnetTemplate.sln ./
COPY src/DotnetTemplate.Domain/DotnetTemplate.Domain.csproj       src/DotnetTemplate.Domain/
COPY src/DotnetTemplate.Application/DotnetTemplate.Application.csproj  src/DotnetTemplate.Application/
COPY src/DotnetTemplate.Api/DotnetTemplate.Api.csproj             src/DotnetTemplate.Api/

RUN dotnet restore DotnetTemplate.sln

COPY src/ src/
RUN dotnet publish src/DotnetTemplate.Api/DotnetTemplate.Api.csproj \
    -c Release -o /publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

COPY --from=build /publish .

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "DotnetTemplate.Api.dll"]
