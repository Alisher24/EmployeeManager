using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Application.Employees;

public sealed record EmployeeDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    Gender Gender,
    string Address1,
    string? Address2,
    string City,
    string PostalCode,
    string Country,
    string Email,
    string Phone,
    bool IsActive);
