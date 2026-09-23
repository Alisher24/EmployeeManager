using EmployeeManager.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManager.Infrastructure.Data.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    private const int GenderMaxLength = 10;

    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(e => e.FirstName).HasMaxLength(EmployeeConstraints.NameMaxLength);
        builder.Property(e => e.LastName).HasMaxLength(EmployeeConstraints.NameMaxLength);
        builder.Property(e => e.Gender).HasConversion<string>().HasMaxLength(GenderMaxLength);
        builder.Property(e => e.Address1).HasMaxLength(EmployeeConstraints.AddressMaxLength);
        builder.Property(e => e.Address2).HasMaxLength(EmployeeConstraints.AddressMaxLength);
        builder.Property(e => e.City).HasMaxLength(EmployeeConstraints.CityMaxLength);
        builder.Property(e => e.PostalCode).HasMaxLength(EmployeeConstraints.PostalCodeMaxLength);
        builder.Property(e => e.Country).HasMaxLength(EmployeeConstraints.CountryMaxLength);
        builder.Property(e => e.Email).HasMaxLength(EmployeeConstraints.EmailMaxLength);
        builder.Property(e => e.Phone).HasMaxLength(EmployeeConstraints.PhoneMaxLength);

        builder.HasIndex(e => e.Email).IsUnique();
    }
}