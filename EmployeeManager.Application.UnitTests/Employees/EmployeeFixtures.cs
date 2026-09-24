using EmployeeManager.Application.Employees;
using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Application.UnitTests.Employees;

internal static class EmployeeFixtures
{
    public static Employee CreateEmployee(
        string firstName = "John",
        string lastName = "Doe",
        string email = "john.doe@example.com") =>
        Employee.Create(
            firstName,
            lastName,
            new DateOnly(1990, 5, 17),
            Gender.Male,
            "221B Baker Street",
            "Flat B",
            "London",
            "NW1 6XE",
            "United Kingdom",
            email,
            "+44 20 7224 3688",
            true);

    public static TRequest CreateRequest<TRequest>()
        where TRequest : EmployeeRequest, new() =>
        new()
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1985, 11, 3),
            Gender = Gender.Female,
            Address1 = "10 Downing Street",
            Address2 = null,
            City = "Westminster",
            PostalCode = "SW1A 2AA",
            Country = "England",
            Email = "jane.smith@example.com",
            Phone = "+44 20 7925 0918",
            IsActive = false
        };
}
