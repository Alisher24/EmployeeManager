namespace EmployeeManager.Application.Employees;

public sealed record EmployeeStats(int Total, int Active, int Male, int Female)
{
    public static readonly EmployeeStats Empty = new(0, 0, 0, 0);
}
