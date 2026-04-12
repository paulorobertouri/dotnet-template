using DotnetTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotnetTemplate.Api.Controllers;

[ApiController]
[Route("v1/customer")]
public sealed class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List()
    {
        var customers = await customerService.ListCustomersAsync();
        return Ok(customers.Select(c => new CustomerResponse(c.Id, c.Name, c.Email)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var customer = await customerService.GetCustomerAsync(id);
        if (customer is null)
            return NotFound();
        return Ok(new CustomerResponse(customer.Id, customer.Name, customer.Email));
    }
}

public sealed record CustomerResponse(string Id, string Name, string Email);
