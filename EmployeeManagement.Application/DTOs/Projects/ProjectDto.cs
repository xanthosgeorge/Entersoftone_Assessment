
namespace EmployeeManagement.Application.DTOs.Projects;

/// <summary>
/// Represents project information returned by the API.
/// </summary>
public class ProjectDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int EmployeeCount { get; set; }
}
