using EmployeeManager.Application.Employees;
using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Application.UnitTests.Employees;

public sealed class EmployeeMappingsTests
{
    [Fact]
    public void ToDto_MapsAllEmployeeFields()
    {
        var employee = EmployeeFixtures.CreateEmployee();

        var dto = employee.ToDto();

        var expected = new EmployeeDto(
            employee.Id,
            "John",
            "Doe",
            new DateOnly(1990, 5, 17),
            Gender.Male,
            "221B Baker Street",
            "Flat B",
            "London",
            "NW1 6XE",
            "United Kingdom",
            "john.doe@example.com",
            "+44 20 7224 3688",
            true);
        Assert.Equal(expected, dto);
    }

    [Fact]
    public void ToDto_MapsAllStatsFields()
    {
        var stats = new EmployeeStats(Total: 10, Active: 7, Male: 6, Female: 4);

        var dto = stats.ToDto();

        Assert.Equal(new EmployeeStatsDto(Total: 10, Active: 7, Male: 6, Female: 4), dto);
    }
}
