using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Application.Employees;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmployeeStats> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        Guid? excludedEmployeeId = null,
        CancellationToken cancellationToken = default);

    void Add(Employee employee);
}
