using EmployeeManager.Application.Employees;
using EmployeeManager.Web.Api.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EmployeeManager.Web.Api.Employees;

public static class EmployeeEndpoints
{
    private const string GetEmployeeByIdRouteName = "GetEmployeeById";

    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees");

        group.MapGet("/", GetAllAsync)
            .WithName("GetEmployees");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName(GetEmployeeByIdRouteName);

        group.MapPost("/", CreateAsync)
            .WithName("CreateEmployee");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateEmployee");

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

    private static async Task<Results<CreatedAtRoute<EmployeeDto>, ProblemHttpResult>> CreateAsync(
        CreateEmployeeRequest request,
        EmployeeService employeeService,
        CancellationToken cancellationToken)
    {
        var result = await employeeService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        var employee = result.Value;

        return TypedResults.CreatedAtRoute(employee, GetEmployeeByIdRouteName, new { id = employee.Id });
    }

    private static async Task<Results<Ok<EmployeeDto>, ProblemHttpResult>> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        EmployeeService employeeService,
        CancellationToken cancellationToken)
    {
        var result = await employeeService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}
