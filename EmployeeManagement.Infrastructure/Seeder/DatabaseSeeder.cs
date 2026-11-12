
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Seeding;

/// <summary>
/// Seeds the database with initial departments, projects, and employees.
/// Ensures that seed data is not duplicated on subsequent executions.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Initializes the database with sample data for development and testing.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed departments.
        var departments = await context.Departments.ToListAsync();

        foreach (var name in new[] { "IT", "Human Resources", "Finance" })
        {
            if (departments.Any(d => d.Name == name))
                continue;

            var department = new Department { Name = name };
            context.Departments.Add(department);
            departments.Add(department);
        }

        // Seed projects.
        var projects = await context.Projects.ToListAsync();

        var projectData = new[]
        {
            ("Employee Portal", new DateTime(2026, 1, 10)),
            ("Inventory Management", new DateTime(2026, 3, 15)),
            ("Reporting Dashboard", new DateTime(2026, 6, 1))
        };

        foreach (var (name, startDate) in projectData)
        {
            if (projects.Any(p => p.Name == name))
                continue;

            var project = new Project
            {
                Name = name,
                StartDate = startDate
            };

            context.Projects.Add(project);
            projects.Add(project);
        }

        // Seed employees and assign departments and projects.
        var employeeData = new[]
        {
            ("John", "Smith", "john.smith@example.com",
                "IT", "Employee Portal"),

            ("Maria", "Johnson", "maria.johnson@example.com",
                "IT", "Inventory Management"),

            ("Alex", "Brown", "alex.brown@example.com",
                "IT", "Reporting Dashboard"),

            ("Emma", "Wilson", "emma.wilson@example.com",
                "Human Resources", "Employee Portal"),

            ("David", "Taylor", "david.taylor@example.com",
                "Finance", "Reporting Dashboard"),

            ("Sophia", "Davis", "sophia.davis@example.com",
                "Finance", "Inventory Management")
        };

        var seedEmails = employeeData.Select(e => e.Item3).ToArray();

        var existingEmployees = await context.Employees
            .Include(e => e.Projects)
            .Where(e => seedEmails.Contains(e.Email))
            .ToListAsync();

        foreach (var (firstName, lastName, email, departmentName, projectName)
            in employeeData)
        {
            var employee = existingEmployees
                .FirstOrDefault(e => e.Email == email);

            if (employee is null)
            {
                employee = new Employee
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2026, 1, 1),
                    Department = departments.Single(d => d.Name == departmentName)
                };

                context.Employees.Add(employee);
            }

            var project = projects.Single(p => p.Name == projectName);

            if (!employee.Projects.Any(p => p.Id == project.Id))
            {
                employee.Projects.Add(project);
            }
        }

        await context.SaveChangesAsync();
    }
}
