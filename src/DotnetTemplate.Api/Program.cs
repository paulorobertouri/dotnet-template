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
    ?? builder.Configuration["JWT_SECRET"]
    ?? throw new InvalidOperationException("Jwt:Secret is required.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? builder.Configuration["JWT_ISSUER"]
    ?? "dotnet-template";
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? builder.Configuration["JWT_AUDIENCE"]
    ?? "dotnet-template";

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
        int.Parse(
            builder.Configuration["Jwt:ExpirationSeconds"]
                ?? builder.Configuration["JWT_EXPIRATION_SECONDS"]
                ?? "3600")));

builder.Services.AddSingleton<ICustomerRepository, SQLiteCustomerRepository>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();

// ── API infrastructure ────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.MapOpenApi();
app.MapScalarApiReference("/docs");

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "..", "..", "public")),
    RequestPath = "/static"
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check (no auth required)
app.MapGet("/health", () => Results.Ok(new { status = "ok", version = "1.0.0" }))
    .AllowAnonymous();

app.Run();

// Expose Program for integration test WebApplicationFactory
public partial class Program { }
