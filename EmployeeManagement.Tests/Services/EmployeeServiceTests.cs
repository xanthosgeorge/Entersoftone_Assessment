
using EmployeeManagement.Api.Exceptions;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests.Services;

/// <summary>
/// Tests employee management operations and business rules.
/// </summary>
public class EmployeeServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldThrowConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var (context, connection) =
            await TestDbContextFactory.CreateAsync();

        await using var dbContext = context;
        await using var dbConnection = connection;

        var department = new Department { Name = "IT" };
        context.Departments.Add(department);

        context.Employees.Add(new Employee
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            HireDate = new DateTime(2026, 1, 1),
            Status = EmployeeStatus.Active,
            Department = department
        });

        await context.SaveChangesAsync();

        var service = new EmployeeService(context);

        var request = new CreateEmployeeRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "john.smith@example.com",
            HireDate = new DateTime(2026, 2, 1),
            DepartmentId = department.Id
        };

        // Act
        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateAsync(request));

        // Assert
        Assert.Equal(
            "An employee with this email already exists.",
            exception.Message);

        Assert.Single(context.Employees);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFound_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var (context, connection) =
            await TestDbContextFactory.CreateAsync();

        await using var dbContext = context;
        await using var dbConnection = connection;

        var service = new EmployeeService(context);

        var request = new CreateEmployeeRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            HireDate = new DateTime(2026, 2, 1),
            DepartmentId = Guid.NewGuid()
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.CreateAsync(request));

        // Assert
        Assert.Equal(
            "The specified department does not exist.",
            exception.Message);

        Assert.Empty(context.Employees);
    }



    [Fact]
    public async Task UpdateAsync_ShouldReplaceEmployeeProjects()
    {
        // Arrange
        var (context, connection) =
            await TestDbContextFactory.CreateAsync();

        await using var dbContext = context;
        await using var dbConnection = connection;

        var department = new Department { Name = "IT" };

        var oldProject = new Project
        {
            Name = "Legacy System",
            StartDate = new DateTime(2026, 1, 1)
        };

        var newProject = new Project
        {
            Name = "Modernization",
            StartDate = new DateTime(2026, 6, 1)
        };

        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            HireDate = new DateTime(2026, 1, 1),
            Department = department,
            Projects = new List<Project> { oldProject }
        };

        context.Employees.Add(employee);
        context.Projects.Add(newProject);

        await context.SaveChangesAsync();

        var service = new EmployeeService(context);

        var request = new UpdateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            Status = EmployeeStatus.Active,
            HireDate = employee.HireDate,
            DepartmentId = department.Id,
            ProjectIds = [newProject.Id]
        };

        // Act
        var updated = await service.UpdateAsync(employee.Id, request);

        // Assert
        Assert.True(updated);

        // Clear tracked entities to verify persisted relationships.
        context.ChangeTracker.Clear();

        var savedEmployee = await context.Employees
            .Include(e => e.Projects)
            .SingleAsync(e => e.Id == employee.Id);

        Assert.Single(savedEmployee.Projects);
        Assert.Contains(savedEmployee.Projects,
            p => p.Id == newProject.Id);

        Assert.DoesNotContain(savedEmployee.Projects,
            p => p.Id == oldProject.Id);

        // Projects must remain in the database.
        Assert.Equal(2, await context.Projects.CountAsync());
    }





}
