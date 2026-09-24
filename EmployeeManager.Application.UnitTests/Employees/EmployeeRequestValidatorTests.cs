using EmployeeManager.Application.Employees;
using EmployeeManager.Domain.Employees;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Time.Testing;

namespace EmployeeManager.Application.UnitTests.Employees;

public sealed class EmployeeRequestValidatorTests
{
    private static readonly DateOnly Today = new(2026, 9, 24);

    private readonly EmployeeRequestValidator _validator =
        new(new FakeTimeProvider(new DateTimeOffset(Today, new TimeOnly(12, 0), TimeSpan.Zero)));

    private readonly CreateEmployeeRequest _validRequest = EmployeeFixtures.CreateRequest<CreateEmployeeRequest>();

    public static MatrixTheoryData<string, string> BlankRequiredTexts => new(
        [
            nameof(EmployeeRequest.FirstName),
            nameof(EmployeeRequest.LastName),
            nameof(EmployeeRequest.Address1),
            nameof(EmployeeRequest.City),
            nameof(EmployeeRequest.PostalCode),
            nameof(EmployeeRequest.Country),
            nameof(EmployeeRequest.Email),
            nameof(EmployeeRequest.Phone)
        ],
        ["", "   "]);

    public static TheoryData<string, int> TextMaxLengths => new()
    {
        { nameof(EmployeeRequest.FirstName), EmployeeConstraints.NameMaxLength },
        { nameof(EmployeeRequest.LastName), EmployeeConstraints.NameMaxLength },
        { nameof(EmployeeRequest.Address1), EmployeeConstraints.AddressMaxLength },
        { nameof(EmployeeRequest.Address2), EmployeeConstraints.AddressMaxLength },
        { nameof(EmployeeRequest.City), EmployeeConstraints.CityMaxLength },
        { nameof(EmployeeRequest.PostalCode), EmployeeConstraints.PostalCodeMaxLength },
        { nameof(EmployeeRequest.Country), EmployeeConstraints.CountryMaxLength }
    };

    [Fact]
    public void Validate_WhenCreateRequestIsValid_HasNoErrors()
    {
        var result = _validator.TestValidate(_validRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUpdateRequestIsValid_HasNoErrors()
    {
        var result = _validator.TestValidate(EmployeeFixtures.CreateRequest<UpdateEmployeeRequest>());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(BlankRequiredTexts))]
    public void Validate_WhenRequiredTextIsBlank_HasOnlyNotEmptyError(string propertyName, string value)
    {
        var result = _validator.TestValidate(RequestWithText(propertyName, value));

        result.ShouldHaveValidationErrorFor(propertyName)
            .WithErrorCode("NotEmptyValidator")
            .Only();
    }

    [Theory]
    [MemberData(nameof(TextMaxLengths))]
    public void Validate_WhenTextIsAtMaxLength_HasNoErrors(string propertyName, int maxLength)
    {
        var result = _validator.TestValidate(RequestWithText(propertyName, new string('a', maxLength)));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(TextMaxLengths))]
    public void Validate_WhenTextExceedsMaxLength_HasMaximumLengthError(string propertyName, int maxLength)
    {
        var result = _validator.TestValidate(RequestWithText(propertyName, new string('a', maxLength + 1)));

        result.ShouldHaveValidationErrorFor(propertyName)
            .WithErrorCode("MaximumLengthValidator")
            .Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WhenAddress2IsEmpty_HasNoErrors(string? address2)
    {
        var result = _validator.TestValidate(_validRequest with { Address2 = address2 });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDateOfBirthIsMissing_HasNotNullError()
    {
        var result = _validator.TestValidate(_validRequest with { DateOfBirth = null });

        result.ShouldHaveValidationErrorFor(request => request.DateOfBirth)
            .WithErrorCode("NotNullValidator")
            .Only();
    }

    [Fact]
    public void Validate_WhenEmployeeIsExactlyMinAge_HasNoErrors()
    {
        var request = _validRequest with { DateOfBirth = Today.AddYears(-EmployeeConstraints.MinAge) };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmployeeIsYoungerThanMinAge_HasMinAgeError()
    {
        var request = _validRequest with { DateOfBirth = Today.AddYears(-EmployeeConstraints.MinAge).AddDays(1) };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.DateOfBirth)
            .WithErrorMessage($"Employee must be at least {EmployeeConstraints.MinAge} years old.")
            .Only();
    }

    [Fact]
    public void Validate_WhenEmployeeIsExactlyMaxAge_HasNoErrors()
    {
        var request = _validRequest with { DateOfBirth = Today.AddYears(-EmployeeConstraints.MaxAge) };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmployeeIsOlderThanMaxAge_HasMaxAgeError()
    {
        var request = _validRequest with { DateOfBirth = Today.AddYears(-EmployeeConstraints.MaxAge).AddDays(-1) };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.DateOfBirth)
            .WithErrorMessage($"Date of birth can't be more than {EmployeeConstraints.MaxAge} years ago.")
            .Only();
    }

    [Fact]
    public void Validate_WhenGenderIsMissing_HasNotNullError()
    {
        var result = _validator.TestValidate(_validRequest with { Gender = null });

        result.ShouldHaveValidationErrorFor(request => request.Gender)
            .WithErrorCode("NotNullValidator")
            .Only();
    }

    [Fact]
    public void Validate_WhenGenderIsUndefined_HasEnumError()
    {
        var result = _validator.TestValidate(_validRequest with { Gender = (Gender)42 });

        result.ShouldHaveValidationErrorFor(request => request.Gender)
            .WithErrorCode("EnumValidator")
            .Only();
    }

    [Fact]
    public void Validate_WhenEmailIsAtMaxLength_HasNoErrors()
    {
        const string domain = "@example.com";
        var email = new string('a', EmployeeConstraints.EmailMaxLength - domain.Length) + domain;

        var result = _validator.TestValidate(_validRequest with { Email = email });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmailExceedsMaxLength_HasMaximumLengthError()
    {
        const string domain = "@example.com";
        var email = new string('a', EmployeeConstraints.EmailMaxLength - domain.Length + 1) + domain;

        var result = _validator.TestValidate(_validRequest with { Email = email });

        result.ShouldHaveValidationErrorFor(request => request.Email)
            .WithErrorCode("MaximumLengthValidator")
            .Only();
    }

    [Theory]
    [InlineData("jane.smith")]
    [InlineData("@example.com")]
    [InlineData("jane.smith@")]
    [InlineData("jane@smith@example.com")]
    public void Validate_WhenEmailIsMalformed_HasEmailError(string email)
    {
        var result = _validator.TestValidate(_validRequest with { Email = email });

        result.ShouldHaveValidationErrorFor(request => request.Email)
            .WithErrorCode("EmailValidator")
            .Only();
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("+123456789012345")]
    [InlineData("+7 (700) 123-45-67")]
    [InlineData("+123 456 789 012 345")]
    public void Validate_WhenPhoneIsWellFormed_HasNoErrors(string phone)
    {
        var result = _validator.TestValidate(_validRequest with { Phone = phone });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("1234567890123456")]
    [InlineData("+7 700 CALL NOW")]
    [InlineData("7+7001234567")]
    [InlineData("++77001234567")]
    public void Validate_WhenPhoneIsMalformed_HasRegularExpressionError(string phone)
    {
        var result = _validator.TestValidate(_validRequest with { Phone = phone });

        result.ShouldHaveValidationErrorFor(request => request.Phone)
            .WithErrorCode("RegularExpressionValidator")
            .Only();
    }

    [Fact]
    public void Validate_WhenPhoneExceedsMaxLength_HasMaximumLengthError()
    {
        var phone = "+" + new string('1', EmployeeConstraints.PhoneMaxLength);

        var result = _validator.TestValidate(_validRequest with { Phone = phone });

        result.ShouldHaveValidationErrorFor(request => request.Phone)
            .WithErrorCode("MaximumLengthValidator")
            .Only();
    }

    [Fact]
    public void Validate_WhenIsActiveIsMissing_HasNotNullError()
    {
        var result = _validator.TestValidate(_validRequest with { IsActive = null });

        result.ShouldHaveValidationErrorFor(request => request.IsActive)
            .WithErrorCode("NotNullValidator")
            .Only();
    }

    private CreateEmployeeRequest RequestWithText(string propertyName, string value) => propertyName switch
    {
        nameof(EmployeeRequest.FirstName) => _validRequest with { FirstName = value },
        nameof(EmployeeRequest.LastName) => _validRequest with { LastName = value },
        nameof(EmployeeRequest.Address1) => _validRequest with { Address1 = value },
        nameof(EmployeeRequest.Address2) => _validRequest with { Address2 = value },
        nameof(EmployeeRequest.City) => _validRequest with { City = value },
        nameof(EmployeeRequest.PostalCode) => _validRequest with { PostalCode = value },
        nameof(EmployeeRequest.Country) => _validRequest with { Country = value },
        nameof(EmployeeRequest.Email) => _validRequest with { Email = value },
        nameof(EmployeeRequest.Phone) => _validRequest with { Phone = value },
        _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null)
    };
}
