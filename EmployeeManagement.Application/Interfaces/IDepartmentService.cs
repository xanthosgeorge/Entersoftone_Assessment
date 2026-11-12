
using EmployeeManagement.Application.DTOs.Departments;

namespace EmployeeManagement.Application.Interfaces;

/// <summary>
/// Defines application operations for managing departments.
/// </summary>
public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DepartmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
