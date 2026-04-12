using System.Text;
using DotnetTemplate.Application.Interfaces;
using DotnetTemplate.Application.Services;
using DotnetTemplate.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── JWT settings ──────────────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is required.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "dotnet-template";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "dotnet-template";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

// ── Application services ──────────────────────────────────────────────────────
builder.Services.AddSingleton<IAuthService>(provider =>
    new AuthService(
        jwtSecret,
        jwtIssuer,
        jwtAudience,
        int.Parse(builder.Configuration["Jwt:ExpirationSeconds"] ?? "3600")));

builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();

// ── API infrastructure ────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check (no auth required)
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .AllowAnonymous();

app.Run();

// Expose Program for integration test WebApplicationFactory
public partial class Program { }
