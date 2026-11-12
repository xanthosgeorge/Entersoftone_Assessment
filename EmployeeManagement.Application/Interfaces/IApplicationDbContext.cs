
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Interfaces
{
    /// <summary>
    /// Defines the database operations required by the application layer,
    /// abstracting the underlying persistence implementation.
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<Employee> Employees { get; }
        DbSet<Department> Departments { get; }
        DbSet<Project> Projects { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
