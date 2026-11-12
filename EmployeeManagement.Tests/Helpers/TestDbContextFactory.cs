
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests.Helpers;

/// <summary>
/// Provides isolated SQLite in-memory database contexts
/// for integration testing.
/// </summary>
public static class TestDbContextFactory
{
    public static async Task<(
        ApplicationDbContext Context,
        SqliteConnection Connection)> CreateAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        try
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new ApplicationDbContext(options);

            await context.Database.EnsureCreatedAsync();

            return (context, connection);
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}
