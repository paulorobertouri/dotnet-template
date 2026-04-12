using DotnetTemplate.Domain.Models;

namespace DotnetTemplate.Application.Interfaces;

public interface IAuthService
{
    string IssueToken(string subject);
    string? ValidateToken(string token);
}

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string id);
    Task<IEnumerable<Customer>> ListAsync();
}

public interface ICustomerService
{
    Task<Customer?> GetCustomerAsync(string id);
    Task<IEnumerable<Customer>> ListCustomersAsync();
}
