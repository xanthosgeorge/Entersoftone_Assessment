
namespace EmployeeManagement.Application.DTOs.Departments;

/// <summary>
/// Represents department information returned by the API.
/// </summary>
public class DepartmentDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int EmployeeCount { get; set; }
}
