namespace EmployeeManager.Domain.Employees;

public sealed class Employee
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public DateOnly DateOfBirth { get; private set; }

    public Gender Gender { get; private set; }

    public string Address1 { get; private set; } = null!;

    public string? Address2 { get; private set; }

    public string City { get; private set; } = null!;

    public string PostalCode { get; private set; } = null!;

    public string Country { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string Phone { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedOn { get; private set; }

    public DateTimeOffset ModifiedOn { get; private set; }

    private Employee()
    {
    }

    public static Employee Create(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        Gender gender,
        string address1,
        string? address2,
        string city,
        string postalCode,
        string country,
        string email,
        string phone,
        bool isActive)
    {
        var employee = new Employee();

        employee.Update(
            firstName,
            lastName,
            dateOfBirth,
            gender,
            address1,
            address2,
            city,
            postalCode,
            country,
            email,
            phone,
            isActive);

        return employee;
    }

    public void Update(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        Gender gender,
        string address1,
        string? address2,
        string city,
        string postalCode,
        string country,
        string email,
        string phone,
        bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(address1);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        if (!Enum.IsDefined(gender))
        {
            throw new ArgumentOutOfRangeException(nameof(gender), gender, "Unknown gender.");
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Address1 = address1.Trim();
        Address2 = string.IsNullOrWhiteSpace(address2) ? null : address2.Trim();
        City = city.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        IsActive = isActive;
    }
}