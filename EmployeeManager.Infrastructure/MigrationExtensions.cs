using EmployeeManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManager.Infrastructure;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
