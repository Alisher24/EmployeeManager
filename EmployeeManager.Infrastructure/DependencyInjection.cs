using EmployeeManager.Infrastructure.Data;
using EmployeeManager.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeManager.Infrastructure;

public static class DependencyInjection
{
    private const string ConnectionStringName = "EmployeeManager";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured. For local development set it with 'dotnet user-secrets'.");
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) => options
            .UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure())
            .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>()));

        return services;
    }
}