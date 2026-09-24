using EmployeeManager.Application.Employees;
using EmployeeManager.Web.Api.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace EmployeeManager.Web.Api.UnitTests.Common;

public sealed class ValidationFilterTests : IDisposable
{
    private readonly IValidator<EmployeeRequest> _validator = Substitute.For<IValidator<EmployeeRequest>>();
    private readonly ValidationFilter<EmployeeRequest> _filter;
    private readonly UpdateEmployeeRequest _request = new() { FirstName = "Jane" };
    private readonly EndpointFilterInvocationContext _context;
    private readonly CancellationTokenSource _requestAborted = new();
    private bool _nextWasCalled;

    public ValidationFilterTests()
    {
        _filter = new ValidationFilter<EmployeeRequest>(_validator);

        var httpContext = new DefaultHttpContext { RequestAborted = _requestAborted.Token };
        _context = new DefaultEndpointFilterInvocationContext(httpContext, Guid.NewGuid(), _request, "other argument");
    }

    public void Dispose() => _requestAborted.Dispose();

    [Fact]
    public async Task InvokeAsync_WhenRequestIsValid_ReturnsResultOfNextFilter()
    {
        _validator.ValidateAsync(_request, _requestAborted.Token).Returns(new ValidationResult());

        var result = await _filter.InvokeAsync(_context, Next);

        Assert.True(_nextWasCalled);
        Assert.Equal("next result", result);
    }

    [Fact]
    public async Task InvokeAsync_WhenRequestIsInvalid_ReturnsValidationProblemWithoutCallingNext()
    {
        _validator.ValidateAsync(_request, _requestAborted.Token).Returns(new ValidationResult(
        [
            new ValidationFailure(nameof(EmployeeRequest.LastName), "Last name is required."),
            new ValidationFailure(nameof(EmployeeRequest.Email), "Email is required."),
            new ValidationFailure(nameof(EmployeeRequest.Email), "Email is invalid.")
        ]));

        var result = await _filter.InvokeAsync(_context, Next);

        Assert.False(_nextWasCalled);
        var problem = Assert.IsType<ValidationProblem>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal(["Last name is required."], problem.ProblemDetails.Errors[nameof(EmployeeRequest.LastName)]);
        Assert.Equal(["Email is required.", "Email is invalid."], problem.ProblemDetails.Errors[nameof(EmployeeRequest.Email)]);
    }

    private ValueTask<object?> Next(EndpointFilterInvocationContext context)
    {
        _nextWasCalled = true;

        return ValueTask.FromResult<object?>("next result");
    }
}
