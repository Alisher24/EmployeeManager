using EmployeeManager.Domain.Employees;

namespace EmployeeManager.Domain.UnitTests.Employees;

public sealed class EmployeeTests
{
    private static readonly DateOnly DateOfBirth = new(1990, 5, 17);

    public static MatrixTheoryData<string, string?> BlankRequiredValues => new(
        ["firstName", "lastName", "address1", "city", "postalCode", "country", "email", "phone"],
        [null, "", "   "]);

    [Fact]
    public void Create_WithValidData_SetsAllProperties()
    {
        var employee = Employee.Create(
            "John",
            "Doe",
            DateOfBirth,
            Gender.Male,
            "221B Baker Street",
            "Flat B",
            "London",
            "NW1 6XE",
            "United Kingdom",
            "john.doe@example.com",
            "+44 20 7224 3688",
            true);

        Assert.Equal("John", employee.FirstName);
        Assert.Equal("Doe", employee.LastName);
        Assert.Equal(DateOfBirth, employee.DateOfBirth);
        Assert.Equal(Gender.Male, employee.Gender);
        Assert.Equal("221B Baker Street", employee.Address1);
        Assert.Equal("Flat B", employee.Address2);
        Assert.Equal("London", employee.City);
        Assert.Equal("NW1 6XE", employee.PostalCode);
        Assert.Equal("United Kingdom", employee.Country);
        Assert.Equal("john.doe@example.com", employee.Email);
        Assert.Equal("+44 20 7224 3688", employee.Phone);
        Assert.True(employee.IsActive);
    }

    [Fact]
    public void Create_TrimsTextValues()
    {
        var employee = Employee.Create(
            "  John ",
            " Doe  ",
            DateOfBirth,
            Gender.Male,
            " 221B Baker Street ",
            "  Flat B ",
            " London ",
            " NW1 6XE ",
            " United Kingdom ",
            "  john.doe@example.com ",
            " +44 20 7224 3688 ",
            true);

        Assert.Equal("John", employee.FirstName);
        Assert.Equal("Doe", employee.LastName);
        Assert.Equal("221B Baker Street", employee.Address1);
        Assert.Equal("Flat B", employee.Address2);
        Assert.Equal("London", employee.City);
        Assert.Equal("NW1 6XE", employee.PostalCode);
        Assert.Equal("United Kingdom", employee.Country);
        Assert.Equal("john.doe@example.com", employee.Email);
        Assert.Equal("+44 20 7224 3688", employee.Phone);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenAddress2IsBlank_SetsAddress2ToNull(string? address2)
    {
        var employee = CreateEmployee(address2: address2);

        Assert.Null(employee.Address2);
    }

    [Theory]
    [MemberData(nameof(BlankRequiredValues))]
    public void Create_WhenRequiredValueIsBlank_ThrowsArgumentException(string parameterName, string? value)
    {
        var exception = Assert.ThrowsAny<ArgumentException>(() => CreateEmployeeWith(parameterName, value!));

        Assert.Equal(parameterName, exception.ParamName);
    }

    [Fact]
    public void Create_WhenGenderIsUndefined_ThrowsArgumentOutOfRangeException()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => CreateEmployee(gender: (Gender)42));

        Assert.Equal("gender", exception.ParamName);
    }

    [Fact]
    public void Update_WithValidData_ReplacesAllProperties()
    {
        var employee = CreateEmployee();
        var newDateOfBirth = new DateOnly(1985, 11, 3);

        employee.Update(
            " Jane ",
            " Smith ",
            newDateOfBirth,
            Gender.Female,
            " 10 Downing Street ",
            null,
            " Westminster ",
            " SW1A 2AA ",
            " England ",
            " jane.smith@example.com ",
            " +44 20 7925 0918 ",
            false);

        Assert.Equal("Jane", employee.FirstName);
        Assert.Equal("Smith", employee.LastName);
        Assert.Equal(newDateOfBirth, employee.DateOfBirth);
        Assert.Equal(Gender.Female, employee.Gender);
        Assert.Equal("10 Downing Street", employee.Address1);
        Assert.Null(employee.Address2);
        Assert.Equal("Westminster", employee.City);
        Assert.Equal("SW1A 2AA", employee.PostalCode);
        Assert.Equal("England", employee.Country);
        Assert.Equal("jane.smith@example.com", employee.Email);
        Assert.Equal("+44 20 7925 0918", employee.Phone);
        Assert.False(employee.IsActive);
    }

    [Fact]
    public void Update_WhenAnyValueIsInvalid_LeavesEmployeeUnchanged()
    {
        var employee = CreateEmployee();
        var original = CreateEmployee();

        Assert.ThrowsAny<ArgumentException>(() => employee.Update(
            "Jane",
            "Smith",
            new DateOnly(1985, 11, 3),
            Gender.Female,
            "10 Downing Street",
            null,
            "Westminster",
            "SW1A 2AA",
            "England",
            "jane.smith@example.com",
            " ",
            false));

        Assert.Equivalent(original, employee, strict: true);
    }

    private static Employee CreateEmployeeWith(string parameterName, string value) => parameterName switch
    {
        "firstName" => CreateEmployee(firstName: value),
        "lastName" => CreateEmployee(lastName: value),
        "address1" => CreateEmployee(address1: value),
        "city" => CreateEmployee(city: value),
        "postalCode" => CreateEmployee(postalCode: value),
        "country" => CreateEmployee(country: value),
        "email" => CreateEmployee(email: value),
        "phone" => CreateEmployee(phone: value),
        _ => throw new ArgumentOutOfRangeException(nameof(parameterName), parameterName, null)
    };

    private static Employee CreateEmployee(
        string firstName = "John",
        string lastName = "Doe",
        Gender gender = Gender.Male,
        string address1 = "221B Baker Street",
        string? address2 = "Flat B",
        string city = "London",
        string postalCode = "NW1 6XE",
        string country = "United Kingdom",
        string email = "john.doe@example.com",
        string phone = "+44 20 7224 3688") =>
        Employee.Create(
            firstName,
            lastName,
            DateOfBirth,
            gender,
            address1,
            address2,
            city,
            postalCode,
            country,
            email,
            phone,
            true);
}
