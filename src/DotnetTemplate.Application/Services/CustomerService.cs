using DotnetTemplate.Application.Interfaces;
using DotnetTemplate.Domain.Models;

namespace DotnetTemplate.Application.Services;

public sealed class CustomerService(ICustomerRepository repository) : ICustomerService
{
    public Task<Customer?> GetCustomerAsync(string id) =>
        repository.GetByIdAsync(id);

    public Task<IEnumerable<Customer>> ListCustomersAsync() =>
        repository.ListAsync();
}
