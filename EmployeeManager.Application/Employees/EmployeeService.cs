using EmployeeManager.Application.Common;
using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Application.Employees;

public sealed class EmployeeService(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken);

        return employees.Select(employee => employee.ToDto()).ToList();
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);

        return employee?.ToDto();
    }

    public async Task<Result<EmployeeDto>> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await employeeRepository.EmailExistsAsync(request.Email, cancellationToken: cancellationToken))
        {
            return EmployeeErrors.EmailAlreadyExists;
        }

        var employee = Employee.Create(
            request.FirstName,
            request.LastName,
            request.DateOfBirth!.Value,
            request.Gender!.Value,
            request.Address1,
            request.Address2,
            request.City,
            request.PostalCode,
            request.Country,
            request.Email,
            request.Phone,
            request.IsActive!.Value);

        employeeRepository.Add(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.ToDto();
    }

    public async Task<Result<EmployeeDto>> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        if (await employeeRepository.EmailExistsAsync(request.Email, id, cancellationToken))
        {
            return EmployeeErrors.EmailAlreadyExists;
        }

        employee.Update(
            request.FirstName,
            request.LastName,
            request.DateOfBirth!.Value,
            request.Gender!.Value,
            request.Address1,
            request.Address2,
            request.City,
            request.PostalCode,
            request.Country,
            request.Email,
            request.Phone,
            request.IsActive!.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.ToDto();
    }
}
