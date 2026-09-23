using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Application.Employees;

public static class EmployeeMappings
{
    public static EmployeeDto ToDto(this Employee employee) =>
        new(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.DateOfBirth,
            employee.Gender,
            employee.Address1,
            employee.Address2,
            employee.City,
            employee.PostalCode,
            employee.Country,
            employee.Email,
            employee.Phone,
            employee.IsActive);

    public static EmployeeStatsDto ToDto(this EmployeeStats stats) =>
        new(stats.Total, stats.Active, stats.Male, stats.Female);
}
