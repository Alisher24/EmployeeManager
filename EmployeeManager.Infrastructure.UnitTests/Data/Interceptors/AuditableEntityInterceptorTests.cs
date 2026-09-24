using EmployeeManager.Domain.Employees;
using EmployeeManager.Infrastructure.Data;
using EmployeeManager.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace EmployeeManager.Infrastructure.UnitTests.Data.Interceptors;

public sealed class AuditableEntityInterceptorTests : IDisposable
{
    private static readonly DateTimeOffset StartTime = new(2026, 9, 24, 8, 30, 0, TimeSpan.Zero);

    private readonly FakeTimeProvider _timeProvider = new(StartTime);
    private readonly AppDbContext _dbContext;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public AuditableEntityInterceptorTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditableEntityInterceptor(_timeProvider))
            .Options;

        _dbContext = new AppDbContext(options);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task SaveChangesAsync_WhenEntityIsAdded_SetsCreatedOnAndModifiedOn()
    {
        var employee = CreateEmployee();
        _dbContext.Employees.Add(employee);

        await _dbContext.SaveChangesAsync(_cancellationToken);

        Assert.Equal(StartTime, employee.CreatedOn);
        Assert.Equal(StartTime, employee.ModifiedOn);
    }

    [Fact]
    public void SaveChanges_WhenEntityIsAdded_SetsCreatedOnAndModifiedOn()
    {
        var employee = CreateEmployee();
        _dbContext.Employees.Add(employee);

        _dbContext.SaveChanges();

        Assert.Equal(StartTime, employee.CreatedOn);
        Assert.Equal(StartTime, employee.ModifiedOn);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityIsModified_UpdatesOnlyModifiedOn()
    {
        var employee = CreateEmployee();
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(_cancellationToken);
        _timeProvider.Advance(TimeSpan.FromHours(1));

        UpdateCity(employee, "Manchester");
        await _dbContext.SaveChangesAsync(_cancellationToken);

        Assert.Equal(StartTime, employee.CreatedOn);
        Assert.Equal(StartTime.AddHours(1), employee.ModifiedOn);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityIsUnchanged_KeepsAuditDates()
    {
        var employee = CreateEmployee();
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(_cancellationToken);
        _timeProvider.Advance(TimeSpan.FromHours(1));

        await _dbContext.SaveChangesAsync(_cancellationToken);

        Assert.Equal(StartTime, employee.CreatedOn);
        Assert.Equal(StartTime, employee.ModifiedOn);
    }

    private static Employee CreateEmployee() =>
        Employee.Create(
            "John",
            "Doe",
            new DateOnly(1990, 5, 17),
            Gender.Male,
            "221B Baker Street",
            null,
            "London",
            "NW1 6XE",
            "United Kingdom",
            "john.doe@example.com",
            "+44 20 7224 3688",
            true);

    private static void UpdateCity(Employee employee, string city) =>
        employee.Update(
            employee.FirstName,
            employee.LastName,
            employee.DateOfBirth,
            employee.Gender,
            employee.Address1,
            employee.Address2,
            city,
            employee.PostalCode,
            employee.Country,
            employee.Email,
            employee.Phone,
            employee.IsActive);
}
