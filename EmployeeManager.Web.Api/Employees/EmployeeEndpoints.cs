using EmployeeManager.Application.Employees;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EmployeeManager.Web.Api.Employees;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees");

        group.MapGet("/", GetAllAsync)
            .WithName("GetEmployees");

        return app;
    }

    private static async Task<Ok<IReadOnlyList<EmployeeDto>>> GetAllAsync(
        EmployeeService employeeService,
        CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetAllAsync(cancellationToken);

        return TypedResults.Ok(employees);
    }
}
