using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace DotnetTemplate.Tests.Integration.Api;

[TestFixture]
public sealed class ApiIntegrationTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("Jwt__Secret", "integration-test-secret-32-chars-min!!");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "dotnet-template");
        Environment.SetEnvironmentVariable("Jwt__Audience", "dotnet-template");
        Environment.SetEnvironmentVariable("Jwt__ExpirationSeconds", "3600");

        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    // ─── Health ─────────────────────────────────────────────────────────────
    [Test]
    public async Task HealthEndpoint_ShouldReturn200()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ─── Public ─────────────────────────────────────────────────────────────
    [Test]
    public async Task PublicEndpoint_ShouldReturn200WithMessage()
    {
        var response = await _client.GetAsync("/v1/public");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("public");
    }

    // ─── Auth / Login ────────────────────────────────────────────────────────
    [Test]
    public async Task LoginEndpoint_ShouldReturn200AndJwtToken()
    {
        var response = await _client.GetAsync("/v1/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.TryGetProperty("token", out var tokenProp).Should().BeTrue();
        tokenProp.GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task LoginEndpoint_ShouldIncludeXJwtTokenHeader()
    {
        var response = await _client.GetAsync("/v1/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-JWT-Token");
    }

    // ─── Private ─────────────────────────────────────────────────────────────
    [Test]
    public async Task PrivateEndpoint_WithoutToken_ShouldReturn401()
    {
        var response = await _client.GetAsync("/v1/private");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task PrivateEndpoint_WithValidToken_ShouldReturn200()
    {
        var token = await ObtainTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/v1/private");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("private");

        _client.DefaultRequestHeaders.Authorization = null;
    }

    // ─── Customers ───────────────────────────────────────────────────────────
    [Test]
    public async Task CustomerListEndpoint_ShouldReturn200WithItems()
    {
        var response = await _client.GetAsync("/v1/customer");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
        doc.RootElement.GetArrayLength().Should().BeGreaterThan(0);
    }

    [Test]
    public async Task CustomerGetById_WithExistingId_ShouldReturn200()
    {
        var response = await _client.GetAsync("/v1/customer/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("id").GetString().Should().Be("1");
    }

    [Test]
    public async Task CustomerGetById_WithUnknownId_ShouldReturn404()
    {
        var response = await _client.GetAsync("/v1/customer/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────
    private async Task<string> ObtainTokenAsync()
    {
        var response = await _client.GetAsync("/v1/auth/login");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("token").GetString()!;
    }
}
