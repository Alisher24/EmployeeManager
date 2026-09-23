using EmployeeManager.Application.Employees;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EmployeeService>();

        return services;
    }
}
