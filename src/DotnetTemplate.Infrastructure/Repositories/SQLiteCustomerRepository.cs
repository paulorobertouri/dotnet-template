using System.Data.SQLite;
using DotnetTemplate.Domain.Models;
using DotnetTemplate.Application.Interfaces;

namespace DotnetTemplate.Infrastructure.Repositories;

public class SQLiteCustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;

    public SQLiteCustomerRepository(string dbPath = "customers.db")
    {
        _connectionString = $"Data Source={dbPath};Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string createTableSql = @"
            CREATE TABLE IF NOT EXISTS Customers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL
            )";

        using var command = new SQLiteCommand(createTableSql, connection);
        command.ExecuteNonQuery();

        // Seed
        using var checkCommand = new SQLiteCommand("SELECT COUNT(*) FROM Customers", connection);
        long count = (long)checkCommand.ExecuteScalar();

        if (count == 0)
        {
            using var seedCommand = new SQLiteCommand(@"
                INSERT INTO Customers (Name, Email) VALUES ('Ana .NET', 'ana@dotnet.com');
                INSERT INTO Customers (Name, Email) VALUES ('Bruno .NET', 'bruno@dotnet.com');", connection);
            seedCommand.ExecuteNonQuery();
        }
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SQLiteCommand(
            "SELECT Id, Name, Email FROM Customers WHERE Id = @Id LIMIT 1",
            connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Customer(
            reader.GetInt32(0).ToString(),
            reader.GetString(1),
            reader.GetString(2));
    }

    public async Task<IEnumerable<Customer>> ListAsync()
    {
        var customers = new List<Customer>();
        using var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SQLiteCommand("SELECT Id, Name, Email FROM Customers", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(new Customer(
                reader.GetInt32(0).ToString(),
                reader.GetString(1),
                reader.GetString(2)));
        }

        return customers;
    }
}
