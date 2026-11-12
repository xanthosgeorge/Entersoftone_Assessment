
using EmployeeManagement.Api.Exceptions;
using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Services;

/// <summary>
/// Provides application operations for managing departments.
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IApplicationDbContext _context;

    public DepartmentService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                EmployeeCount = d.Employees.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<DepartmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                EmployeeCount = d.Employees.Count
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();

        if (await _context.Departments.AnyAsync(
            d => d.Name == name, cancellationToken))
        {
            throw new ConflictException(
                "A department with this name already exists.");
        }

        var department = new Department
        {
            Name = name,
            Description = request.Description?.Trim()
        };

        _context.Departments.Add(department);

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(
            department.Id, cancellationToken))!;
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department is null)
            return false;

        var name = request.Name.Trim();

        if (await _context.Departments.AnyAsync(
            d => d.Name == name && d.Id != id,
            cancellationToken))
        {
            throw new ConflictException(
                "A department with this name already exists.");
        }

        department.Name = name;
        department.Description = request.Description?.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department is null)
            return false;

        var hasEmployees = await _context.Employees
            .AnyAsync(e => e.DepartmentId == id, cancellationToken);

        if (hasEmployees)
        {
            throw new ConflictException(
                "Cannot delete a department that has assigned employees.");
        }

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
