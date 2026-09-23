using EmployeeManager.Application.Common;

namespace EmployeeManager.Application.Employees;

public static class EmployeeErrors
{
    public static readonly Error NotFound =
        new(ErrorType.NotFound, "The employee was not found.");

    public static readonly Error EmailAlreadyExists =
        new(ErrorType.Conflict, "An employee with this email already exists.");
}
