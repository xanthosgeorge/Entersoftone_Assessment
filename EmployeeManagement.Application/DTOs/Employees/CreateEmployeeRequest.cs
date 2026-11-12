
using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs.Employees;

/// <summary>
/// Represents the validated request data required
/// to create an employee.
/// </summary>
public class CreateEmployeeRequest : IValidatableObject
{
    [Required]
    [StringLength(100)]
    public required string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public required string Email { get; set; }

    [EnumDataType(typeof(EmployeeStatus))]
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public DateTime HireDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public Guid DepartmentId { get; set; }

    public List<Guid> ProjectIds { get; set; } = [];

    /// <summary>
    /// Performs cross-property and domain-specific request validation.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(FirstName))
            yield return new ValidationResult(
                "First name cannot be empty or whitespace.",
                [nameof(FirstName)]);

        if (string.IsNullOrWhiteSpace(LastName))
            yield return new ValidationResult(
                "Last name cannot be empty or whitespace.",
                [nameof(LastName)]);

        if (DepartmentId == Guid.Empty)
            yield return new ValidationResult(
                "A valid department ID is required.",
                [nameof(DepartmentId)]);

        if (HireDate == default)
            yield return new ValidationResult(
                "Hire date is required.",
                [nameof(HireDate)]);

        if (ProjectIds is null || ProjectIds.Any(id => id == Guid.Empty))
            yield return new ValidationResult(
                "Project IDs must contain valid GUID values.",
                [nameof(ProjectIds)]);
    }
}
