namespace EmployeeManager.Domain.Common;

public interface IAuditableEntity
{
    DateTimeOffset CreatedOn { get; }

    DateTimeOffset ModifiedOn { get; }
}