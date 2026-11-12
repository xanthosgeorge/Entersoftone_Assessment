using EmployeeManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Application.DTOs.Employees
{
    /// <summary>
    /// Represents the employee data returned by the API.
    /// </summary>
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime HireDate { get; set; }
        public string? Notes { get; set; }
        public Guid DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public List<Guid> ProjectIds { get; set; } = [];
    }
}
