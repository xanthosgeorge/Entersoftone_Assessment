
using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Application.DTOs.Projects;

namespace EmployeeManagement.Tests.Validation;

/// <summary>
/// Tests validation rules for project requests.
/// </summary>
public class ProjectValidationTests
{
    [Fact]
    public void Validate_ShouldRejectEndDateBeforeStartDate()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            Name = "Invalid Project",
            StartDate = new DateTime(2026, 11, 12),
            EndDate = new DateTime(2026, 1, 1)
        };

        var validationContext = new ValidationContext(request);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            request,
            validationContext,
            results,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);

        Assert.Contains(results, result =>
            result.MemberNames.Contains(nameof(request.EndDate)));
    }
}
