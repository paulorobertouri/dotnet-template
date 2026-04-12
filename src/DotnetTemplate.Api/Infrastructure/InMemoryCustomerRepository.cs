using DotnetTemplate.Application.Interfaces;
using DotnetTemplate.Domain.Models;

namespace DotnetTemplate.Infrastructure.Repositories;

public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private static readonly IReadOnlyList<Customer> _store =
    [
        new("1", "Alice Smith", "alice@example.com"),
        new("2", "Bob Jones", "bob@example.com"),
        new("3", "Carol White", "carol@example.com"),
    ];

    public Task<Customer?> GetByIdAsync(string id) =>
        Task.FromResult(_store.FirstOrDefault(c => c.Id == id));

    public Task<IEnumerable<Customer>> ListAsync() =>
        Task.FromResult<IEnumerable<Customer>>(_store);
}
