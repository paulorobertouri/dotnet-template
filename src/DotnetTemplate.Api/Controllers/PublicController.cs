using Microsoft.AspNetCore.Mvc;

namespace DotnetTemplate.Api.Controllers;

[ApiController]
[Route("v1/public")]
public sealed class PublicController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PublicResponse), StatusCodes.Status200OK)]
    public IActionResult Get() =>
        Ok(new PublicResponse("This is a public endpoint"));
}

public sealed record PublicResponse(string Message);
