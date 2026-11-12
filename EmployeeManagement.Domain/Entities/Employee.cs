using EmployeeManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public DateTime HireDate { get; set; }
        public string? Notes { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
