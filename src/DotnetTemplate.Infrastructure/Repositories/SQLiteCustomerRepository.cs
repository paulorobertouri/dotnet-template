using System.Data.SQLite;
using DotnetTemplate.Domain.Models;
using DotnetTemplate.Domain.Repositories;

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

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customers = new List<Customer>();
        using var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SQLiteCommand("SELECT Id, Name, Email FROM Customers", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2)
            });
        }

        return customers;
    }
}
