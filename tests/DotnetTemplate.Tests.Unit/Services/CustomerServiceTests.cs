using DotnetTemplate.Application.Interfaces;
using DotnetTemplate.Application.Services;
using DotnetTemplate.Domain.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DotnetTemplate.Tests.Unit.Services;

[TestFixture]
public sealed class CustomerServiceTests
{
    private Mock<ICustomerRepository> _repositoryMock = null!;
    private CustomerService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _sut = new CustomerService(_repositoryMock.Object);
    }

    [Test]
    public async Task ListCustomersAsync_ShouldReturnAllCustomers()
    {
        var expected = new List<Customer>
        {
            new("1", "Alice", "alice@example.com"),
            new("2", "Bob", "bob@example.com"),
        };
        _repositoryMock.Setup(r => r.ListAsync()).ReturnsAsync(expected);

        var result = await _sut.ListCustomersAsync();

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task GetCustomerAsync_WithExistingId_ShouldReturnCustomer()
    {
        var customer = new Customer("1", "Alice", "alice@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(customer);

        var result = await _sut.GetCustomerAsync("1");

        result.Should().Be(customer);
    }

    [Test]
    public async Task GetCustomerAsync_WithUnknownId_ShouldReturnNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync("99")).ReturnsAsync((Customer?)null);

        var result = await _sut.GetCustomerAsync("99");

        result.Should().BeNull();
    }
}
