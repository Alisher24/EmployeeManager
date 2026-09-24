using EmployeeManager.Application.Common;

namespace EmployeeManager.Application.UnitTests.Common;

public sealed class ResultTests
{
    [Fact]
    public void ImplicitConversionFromValue_CreatesSuccessfulResult()
    {
        Result<string> result = "value";

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Equal("value", result.Value);
    }

    [Fact]
    public void ImplicitConversionFromError_CreatesFailedResult()
    {
        var error = new Error(ErrorType.NotFound, "Not found.");

        Result<string> result = error;

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Value_WhenResultIsFailed_ThrowsInvalidOperationException()
    {
        Result<string> result = new Error(ErrorType.Conflict, "Conflict.");

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }
}
