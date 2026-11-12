
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Tests.Helpers;

namespace EmployeeManagement.Tests.Services;

/// <summary>
/// Tests department management operations and business rules.
/// </summary>
public class DepartmentServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldThrowConflict_WhenDepartmentNameExists()
    {
        // Arrange
        var (context, connection) =
            await TestDbContextFactory.CreateAsync();

        await using var dbContext = context;
        await using var dbConnection = connection;

        var service = new DepartmentService(context);

        var request = new CreateDepartmentRequest
        {
            Name = "IT",
            Description = "Information Technology"
        };

        await service.CreateAsync(request);

        // Act
        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateAsync(request));

        // Assert
        Assert.Equal(
            "A department with this name already exists.",
            exception.Message);

        Assert.Single(context.Departments);
    }




    [Fact]
    public async Task DeleteAsync_ShouldThrowConflict_WhenDepartmentHasEmployees()
    {
        // Arrange
        var (context, connection) =
            await TestDbContextFactory.CreateAsync();

        await using var dbContext = context;
        await using var dbConnection = connection;

        var department = new Department
        {
            Name = "IT"
        };

        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.test@example.com",
            Status = EmployeeStatus.Active,
            HireDate = DateTime.UtcNow,
            Department = department
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var service = new DepartmentService(context);

        // Act
        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.DeleteAsync(department.Id));

        // Assert
        Assert.Equal(
            "Cannot delete a department that has assigned employees.",
            exception.Message);

        Assert.Single(context.Departments);
        Assert.Single(context.Employees);
    }




    [Fact]
    public async Task DeleteAsync_ShouldRemoveDepartment_WhenNoEmployeesExist()
    {
        // Arrange
        var (context, connection) =
            await TestDbContextFactory.CreateAsync();

        await using var dbContext = context;
        await using var dbConnection = connection;

        var department = new Department
        {
            Name = "Research"
        };

        context.Departments.Add(department);
        await context.SaveChangesAsync();

        var service = new DepartmentService(context);

        // Act
        var result = await service.DeleteAsync(department.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(context.Departments);
    }


}
