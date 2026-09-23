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

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetEmployeeById");

        return app;
    }

    private static async Task<Ok<IReadOnlyList<EmployeeDto>>> GetAllAsync(
        EmployeeService employeeService,
        CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetAllAsync(cancellationToken);

        return TypedResults.Ok(employees);
    }

    private static async Task<Results<Ok<EmployeeDto>, NotFound>> GetByIdAsync(
        Guid id,
        EmployeeService employeeService,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(id, cancellationToken);

        return employee is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(employee);
    }
}
