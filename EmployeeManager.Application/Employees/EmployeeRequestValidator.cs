using System.Text.RegularExpressions;
using EmployeeManager.Domain.Employees;
using FluentValidation;

namespace EmployeeManager.Application.Employees;

public sealed partial class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.NameMaxLength);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.NameMaxLength);

        RuleFor(request => request.DateOfBirth)
            .NotNull()
            .LessThanOrEqualTo(_ => YearsAgo(timeProvider, EmployeeConstraints.MinAge))
            .WithMessage($"Employee must be at least {EmployeeConstraints.MinAge} years old.")
            .GreaterThanOrEqualTo(_ => YearsAgo(timeProvider, EmployeeConstraints.MaxAge))
            .WithMessage($"Date of birth can't be more than {EmployeeConstraints.MaxAge} years ago.");

        RuleFor(request => request.Gender)
            .NotNull()
            .IsInEnum();

        RuleFor(request => request.Address1)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.AddressMaxLength);

        RuleFor(request => request.Address2)
            .MaximumLength(EmployeeConstraints.AddressMaxLength);

        RuleFor(request => request.City)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.CityMaxLength);

        RuleFor(request => request.PostalCode)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.PostalCodeMaxLength);

        RuleFor(request => request.Country)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.CountryMaxLength);

        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.EmailMaxLength)
            .EmailAddress();

        RuleFor(request => request.Phone)
            .NotEmpty()
            .MaximumLength(EmployeeConstraints.PhoneMaxLength)
            .Matches(PhoneRegex());

        RuleFor(request => request.IsActive)
            .NotNull();
    }

    private static DateOnly YearsAgo(TimeProvider timeProvider, int years) =>
        DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime).AddYears(-years);

    [GeneratedRegex(@"^\+?(?:[ ()-]*[0-9]){7,15}[ ()-]*$")]
    private static partial Regex PhoneRegex();
}
