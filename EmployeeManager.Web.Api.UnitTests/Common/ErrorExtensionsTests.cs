using EmployeeManager.Application.Common;
using EmployeeManager.Web.Api.Common;
using Microsoft.AspNetCore.Http;

namespace EmployeeManager.Web.Api.UnitTests.Common;

public sealed class ErrorExtensionsTests
{
    [Theory]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    public void ToProblem_MapsErrorTypeToStatusCode(ErrorType errorType, int expectedStatusCode)
    {
        var problem = new Error(errorType, "Something went wrong.").ToProblem();

        Assert.Equal(expectedStatusCode, problem.StatusCode);
        Assert.Equal(expectedStatusCode, problem.ProblemDetails.Status);
        Assert.Equal("Something went wrong.", problem.ProblemDetails.Detail);
    }

    [Fact]
    public void ToProblem_WhenErrorTypeIsUnknown_ReturnsInternalServerError()
    {
        var problem = new Error((ErrorType)42, "Something went wrong.").ToProblem();

        Assert.Equal(StatusCodes.Status500InternalServerError, problem.StatusCode);
    }
}
