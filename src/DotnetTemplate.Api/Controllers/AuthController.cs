using DotnetTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetTemplate.Api.Controllers;

[ApiController]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpGet("v1/auth/login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public IActionResult Login()
    {
        var token = authService.IssueToken("user@example.com");
        Response.Headers["X-JWT-Token"] = token;
        return Ok(new LoginResponse(token));
    }

    [HttpGet("v1/private")]
    [Authorize]
    [ProducesResponseType(typeof(PrivateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Private()
    {
        var user = User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "unknown";
        return Ok(new PrivateResponse("This is a private endpoint", user));
    }
}

public sealed record LoginResponse(string Token);
public sealed record PrivateResponse(string Message, string User);
