
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests.Services;

/// <summary>
/// Tests project management operations and database relationships.
/// </summary>
public class ProjectServiceTests
{
    [Fact]
    public async Task DeleteAsync_ShouldPreserveEmployees_WhenProjectIsDeleted()
    {
        // Arrange
        var (context, connection) =
            await TestDbContextFactory.CreateAsync();

        await using var dbContext = context;
        await using var dbConnection = connection;

        var department = new Department { Name = "IT" };

        var project = new Project
        {
            Name = "Employee Portal",
            StartDate = new DateTime(2026, 1, 1)
        };

        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            HireDate = new DateTime(2026, 1, 1),
            Department = department,
            Projects = new List<Project> { project }
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        // Act
        var deleted = await service.DeleteAsync(project.Id);

        // Assert
        Assert.True(deleted);

        context.ChangeTracker.Clear();

        Assert.Empty(await context.Projects.ToListAsync());

        var savedEmployee = await context.Employees
            .Include(e => e.Projects)
            .SingleAsync(e => e.Id == employee.Id);

        Assert.Empty(savedEmployee.Projects);
        Assert.Equal("John", savedEmployee.FirstName);
    }
}
