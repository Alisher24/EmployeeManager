namespace EmployeeManager.Domain.Employees;

public static class EmployeeConstraints
{
    public const int NameMaxLength = 100;

    public const int AddressMaxLength = 200;

    public const int CityMaxLength = 100;

    public const int PostalCodeMaxLength = 20;

    public const int CountryMaxLength = 100;

    public const int EmailMaxLength = 254;

    public const int PhoneMaxLength = 20;

    public const int MinAge = 16;

    public const int MaxAge = 100;
}