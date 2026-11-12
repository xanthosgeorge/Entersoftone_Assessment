using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Configurations
{
    /// <summary>
    /// Defines the Entity Framework Core configuration for the Project entity,
    /// including property constraints, primary key, and date column mappings
    /// using the Fluent API.
    /// </summary>
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.StartDate)
                .HasColumnType("date");

            builder.Property(p => p.EndDate)
                .HasColumnType("date");
        }
    }
}
