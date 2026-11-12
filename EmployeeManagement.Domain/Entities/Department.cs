using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Domain.Entities
{
    public  class Department
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
