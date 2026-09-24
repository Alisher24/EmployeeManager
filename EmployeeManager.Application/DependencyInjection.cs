using EmployeeManager.Application.Employees;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<EmployeeService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        ValidatorOptions.Global.LanguageManager.Enabled = false;

        return services;
    }
}
