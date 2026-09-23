using EmployeeManager.Application.Employees;
using EmployeeManager.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Infrastructure.Data.Repositories;

internal sealed class EmployeeRepository(AppDbContext dbContext) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Employees
            .AsNoTracking()
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<EmployeeStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await dbContext.Employees
            .GroupBy(_ => 1)
            .Select(g => new EmployeeStats(
                g.Count(),
                g.Count(e => e.IsActive),
                g.Count(e => e.Gender == Gender.Male),
                g.Count(e => e.Gender == Gender.Female)))
            .SingleOrDefaultAsync(cancellationToken);

        return stats ?? EmployeeStats.Empty;
    }

    public Task<bool> EmailExistsAsync(
        string email,
        Guid? excludedEmployeeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim();

        return dbContext.Employees.AnyAsync(
            employee => employee.Email == normalizedEmail && employee.Id != excludedEmployeeId,
            cancellationToken);
    }

    public void Add(Employee employee) => dbContext.Employees.Add(employee);
}
