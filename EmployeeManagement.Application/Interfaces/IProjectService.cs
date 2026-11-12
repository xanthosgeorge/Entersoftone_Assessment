
using EmployeeManagement.Application.DTOs.Projects;

namespace EmployeeManagement.Application.Interfaces;

/// <summary>
/// Defines application operations for managing projects.
/// </summary>
public interface IProjectService
{
    Task<IReadOnlyList<ProjectDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProjectDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProjectDto> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
