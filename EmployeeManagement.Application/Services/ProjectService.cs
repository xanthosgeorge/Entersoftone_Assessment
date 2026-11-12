
using EmployeeManagement.Application.DTOs.Projects;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Services;

/// <summary>
/// Provides application operations for retrieving and managing projects.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IApplicationDbContext _context;

    public ProjectService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all projects with their assigned employee counts.
    /// </summary>
    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                EmployeeCount = p.Employees.Count
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// </summary>
    public async Task<ProjectDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                EmployeeCount = p.Employees.Count
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    public async Task<ProjectDto> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _context.Projects.Add(project);

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(
            project.Id, cancellationToken))!;
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project is null)
            return false;

        project.Name = request.Name.Trim();
        project.Description = request.Description?.Trim();
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Deletes a project and its employee assignments.
    /// Employees themselves are preserved.
    /// </summary>
    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project is null)
            return false;

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
