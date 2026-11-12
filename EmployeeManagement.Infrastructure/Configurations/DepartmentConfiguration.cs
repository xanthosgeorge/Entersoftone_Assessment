using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Infrastructure.Configurations
{

    /// <summary>
    /// Defines the Entity Framework Core configuration for the Department entity,
    /// including property constraints, primary key, and unique name index
    /// using the Fluent API.
    /// </summary>
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(d => d.Name)
                .IsUnique();

            builder.Property(d => d.Description)
                .HasMaxLength(500);
        }
    }
}
