using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Application.Employees;

public abstract record EmployeeRequest
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public DateOnly? DateOfBirth { get; init; }

    public Gender? Gender { get; init; }

    public string Address1 { get; init; } = string.Empty;

    public string? Address2 { get; init; }

    public string City { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public string Country { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public bool? IsActive { get; init; }
}
