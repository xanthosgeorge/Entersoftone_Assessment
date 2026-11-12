
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs.Projects;

/// <summary>
/// Represents the validated data required to create a project.
/// </summary>
public class CreateProjectRequest : IValidatableObject
{
    [Required]
    [StringLength(150)]
    public required string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Validates project dates and required values.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult(
                "Project name cannot be empty or whitespace.",
                [nameof(Name)]);
        }

        if (StartDate == default)
        {
            yield return new ValidationResult(
                "Start date is required.",
                [nameof(StartDate)]);
        }

        if (EndDate.HasValue && EndDate.Value < StartDate)
        {
            yield return new ValidationResult(
                "End date cannot be earlier than start date.",
                [nameof(EndDate)]);
        }
    }
}
