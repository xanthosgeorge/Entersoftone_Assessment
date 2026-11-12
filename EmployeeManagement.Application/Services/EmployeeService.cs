
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Application.Exceptions;

namespace EmployeeManagement.Application.Services;

/// <summary>
/// Provides business operations for retrieving and managing employees.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IApplicationDbContext _context;

    public EmployeeService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves employees with optional search and pagination.
    /// </summary>
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(
        string? search = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Employees.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(e =>
                e.FirstName.Contains(search) ||
                e.LastName.Contains(search) ||
                e.Email.Contains(search));
        }

        return await query
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Status = e.Status,
                HireDate = e.HireDate,
                Notes = e.Notes,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                ProjectIds = e.Projects.Select(p => p.Id).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves an employee by their unique identifier.
    /// </summary>
    public async Task<EmployeeDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Status = e.Status,
                HireDate = e.HireDate,
                Notes = e.Notes,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                ProjectIds = e.Projects.Select(p => p.Id).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmployeeDto> CreateAsync(
    CreateEmployeeRequest request,
    CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        if (await _context.Employees.AnyAsync(
            e => e.Email == email, cancellationToken))
        {
            throw new ConflictException(
                    "An employee with this email already exists.");
        }

        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (!departmentExists)
        {
            throw new ResourceNotFoundException(
                "The specified department does not exist.");
        }

        var projectIds = request.ProjectIds.Distinct().ToList();

        var projects = await _context.Projects
            .Where(p => projectIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (projects.Count != projectIds.Count)
        {
            throw new ResourceNotFoundException(
                "One or more specified projects do not exist.");
        }

        var employee = new Employee
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Status = request.Status,
            HireDate = request.HireDate,
            Notes = request.Notes,
            DepartmentId = request.DepartmentId,
            Projects = projects
        };

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(employee.Id, cancellationToken))!;
    }


    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .Include(e => e.Projects)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
            return false;

        var email = request.Email.Trim();

        var emailExists = await _context.Employees.AnyAsync(
            e => e.Email == email && e.Id != id,
            cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "An employee with this email already exists.");
        }

        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (!departmentExists)
        {
            throw new ResourceNotFoundException(
                "The specified department does not exist.");
        }

        var projectIds = request.ProjectIds.Distinct().ToList();

        var projects = await _context.Projects
            .Where(p => projectIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (projects.Count != projectIds.Count)
        {
            throw new ResourceNotFoundException(
                "One or more specified projects do not exist.");
        }

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = email;
        employee.Status = request.Status;
        employee.HireDate = request.HireDate;
        employee.Notes = request.Notes;
        employee.DepartmentId = request.DepartmentId;

        employee.Projects.Clear();

        foreach (var project in projects)
        {
            employee.Projects.Add(project);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }



    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
            return false;

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

}
