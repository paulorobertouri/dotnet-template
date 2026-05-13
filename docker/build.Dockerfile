# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copy new .slnx and projects
COPY DotnetTemplate.slnx ./
COPY src/DotnetTemplate.Domain/DotnetTemplate.Domain.csproj src/DotnetTemplate.Domain/
COPY src/DotnetTemplate.Application/DotnetTemplate.Application.csproj src/DotnetTemplate.Application/
COPY src/DotnetTemplate.Infrastructure/DotnetTemplate.Infrastructure.csproj src/DotnetTemplate.Infrastructure/
COPY src/DotnetTemplate.Api/DotnetTemplate.Api.csproj src/DotnetTemplate.Api/

RUN dotnet restore src/DotnetTemplate.Api/DotnetTemplate.Api.csproj

COPY src/ src/
RUN dotnet publish src/DotnetTemplate.Api/DotnetTemplate.Api.csproj \
    -c Release -o /publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Security: Set up non-root user
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
RUN chown appuser:appgroup /app
USER appuser

COPY --from=build /publish .

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

EXPOSE 8080
ENTRYPOINT ["dotnet", "DotnetTemplate.Api.dll"]
