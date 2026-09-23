namespace EmployeeManager.Application.Employees;

public sealed class EmployeeService(IEmployeeRepository employeeRepository)
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken);

        return employees.Select(employee => employee.ToDto()).ToList();
    }
}
