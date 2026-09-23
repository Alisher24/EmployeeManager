using EmployeeManager.Application.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EmployeeManager.Web.Api.Common;

internal static class ErrorExtensions
{
    public static ProblemHttpResult ToProblem(this Error error) =>
        TypedResults.Problem(
            detail: error.Message,
            statusCode: error.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            });
}
