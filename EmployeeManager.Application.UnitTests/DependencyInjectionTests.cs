using EmployeeManager.Application.Employees;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManager.Application.UnitTests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_RegistersValidatorForEmployeeRequests()
    {
        using var serviceProvider = new ServiceCollection().AddApplication().BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var validator = scope.ServiceProvider.GetRequiredService<IValidator<EmployeeRequest>>();

        Assert.IsType<EmployeeRequestValidator>(validator);
    }
}
