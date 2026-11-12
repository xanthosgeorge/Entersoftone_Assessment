
using EmployeeManagement.Application.DTOs.Employees;

namespace EmployeeManagement.Application.Interfaces;

/// <summary>
/// Defines the operations available for managing employees.
/// </summary>
public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync(
        string? search = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<EmployeeDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EmployeeDto> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
