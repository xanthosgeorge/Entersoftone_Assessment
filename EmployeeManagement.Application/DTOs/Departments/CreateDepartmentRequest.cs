using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs.Departments
{

    

   

    /// <summary>
    /// Represents the validated data required to create a department.
    /// </summary>
    public class CreateDepartmentRequest
    {
        [Required]
        [StringLength(150)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }

}
