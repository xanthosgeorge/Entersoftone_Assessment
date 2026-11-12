using EmployeeManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Application.DTOs.Employees
{
    /// <summary>
    /// Represents the data required to create a new employee.
    /// </summary>
    public class CreateEmployeeRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public DateTime HireDate { get; set; }
        public string? Notes { get; set; }
        public Guid DepartmentId { get; set; }
        public List<Guid> ProjectIds { get; set; } = [];
    }
}
