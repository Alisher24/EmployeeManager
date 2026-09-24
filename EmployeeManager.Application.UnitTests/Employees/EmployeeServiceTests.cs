using EmployeeManager.Application.Common;
using EmployeeManager.Application.Employees;
using EmployeeManager.Domain.Employees;
using NSubstitute;

namespace EmployeeManager.Application.UnitTests.Employees;

public sealed class EmployeeServiceTests
{
    private readonly IEmployeeRepository _employeeRepository = Substitute.For<IEmployeeRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly EmployeeService _employeeService;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public EmployeeServiceTests()
    {
        _employeeService = new EmployeeService(_employeeRepository, _unitOfWork);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmployeesMappedToDtosInRepositoryOrder()
    {
        IReadOnlyList<Employee> employees =
        [
            EmployeeFixtures.CreateEmployee("Ann", "Adams", "ann.adams@example.com"),
            EmployeeFixtures.CreateEmployee("Bob", "Brown", "bob.brown@example.com")
        ];
        _employeeRepository.GetAllAsync(_cancellationToken).Returns(employees);

        var result = await _employeeService.GetAllAsync(_cancellationToken);

        Assert.Equal(employees.Select(employee => employee.ToDto()), result);
    }

    [Fact]
    public async Task GetAllAsync_WhenThereAreNoEmployees_ReturnsEmptyList()
    {
        _employeeRepository.GetAllAsync(_cancellationToken).Returns([]);

        var result = await _employeeService.GetAllAsync(_cancellationToken);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var employee = EmployeeFixtures.CreateEmployee();
        _employeeRepository.GetByIdAsync(id, _cancellationToken).Returns(employee);

        var result = await _employeeService.GetByIdAsync(id, _cancellationToken);

        Assert.Equal(employee.ToDto(), result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _employeeRepository.GetByIdAsync(id, _cancellationToken).Returns((Employee?)null);

        var result = await _employeeService.GetByIdAsync(id, _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsMappedStats()
    {
        _employeeRepository.GetStatsAsync(_cancellationToken).Returns(new EmployeeStats(10, 7, 6, 4));

        var result = await _employeeService.GetStatsAsync(_cancellationToken);

        Assert.Equal(new EmployeeStatsDto(10, 7, 6, 4), result);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailIsFree_AddsEmployeeAndSavesChanges()
    {
        var request = EmployeeFixtures.CreateRequest<CreateEmployeeRequest>();
        Employee? addedEmployee = null;
        _employeeRepository.Add(Arg.Do<Employee>(employee => addedEmployee = employee));

        var result = await _employeeService.CreateAsync(request, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(addedEmployee);
        Assert.Equivalent(request, addedEmployee);
        Assert.Equal(addedEmployee.ToDto(), result.Value);
        Received.InOrder(() =>
        {
            _employeeRepository.EmailExistsAsync(request.Email, null, _cancellationToken);
            _employeeRepository.Add(addedEmployee);
            _unitOfWork.SaveChangesAsync(_cancellationToken);
        });
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ReturnsConflictWithoutSaving()
    {
        var request = EmployeeFixtures.CreateRequest<CreateEmployeeRequest>();
        _employeeRepository.EmailExistsAsync(request.Email, null, _cancellationToken).Returns(true);

        var result = await _employeeService.CreateAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeErrors.EmailAlreadyExists, result.Error);
        _employeeRepository.DidNotReceive().Add(Arg.Any<Employee>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenRequestIsValid_UpdatesEmployeeAndSavesChanges()
    {
        var id = Guid.NewGuid();
        var employee = EmployeeFixtures.CreateEmployee();
        var request = EmployeeFixtures.CreateRequest<UpdateEmployeeRequest>();
        _employeeRepository.GetByIdAsync(id, _cancellationToken).Returns(employee);

        var result = await _employeeService.UpdateAsync(id, request, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equivalent(request, employee);
        Assert.Equal(employee.ToDto(), result.Value);
        Received.InOrder(() =>
        {
            _employeeRepository.EmailExistsAsync(request.Email, id, _cancellationToken);
            _unitOfWork.SaveChangesAsync(_cancellationToken);
        });
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var request = EmployeeFixtures.CreateRequest<UpdateEmployeeRequest>();
        _employeeRepository.GetByIdAsync(id, _cancellationToken).Returns((Employee?)null);

        var result = await _employeeService.UpdateAsync(id, request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeErrors.NotFound, result.Error);
        await _employeeRepository.DidNotReceive()
            .EmailExistsAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenEmailBelongsToAnotherEmployee_ReturnsConflictAndKeepsEmployeeUnchanged()
    {
        var id = Guid.NewGuid();
        var employee = EmployeeFixtures.CreateEmployee();
        var originalDto = employee.ToDto();
        var request = EmployeeFixtures.CreateRequest<UpdateEmployeeRequest>();
        _employeeRepository.GetByIdAsync(id, _cancellationToken).Returns(employee);
        _employeeRepository.EmailExistsAsync(request.Email, id, _cancellationToken).Returns(true);

        var result = await _employeeService.UpdateAsync(id, request, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeErrors.EmailAlreadyExists, result.Error);
        Assert.Equal(originalDto, employee.ToDto());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
