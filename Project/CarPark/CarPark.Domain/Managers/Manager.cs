using CarPark.Enterprises;
using CarPark.Managers.Errors;
using FluentResults;

namespace CarPark.Managers;

public sealed class Manager
{
    public Guid Id { get; private init; }

    public string IdentityUserId { get; private set; }

    private List<Enterprise> _enterprises;
    public IReadOnlyList<Enterprise> Enterprises => _enterprises;

    #pragma warning disable CS8618
    [Obsolete("Only for ORM and deserialization! Do not use!")]
    private Manager()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    private Manager(
        Guid id,
        string identityUserId,
        IEnumerable<Enterprise> enterprises)
    {
        Id = id;
        IdentityUserId = identityUserId;
        _enterprises = new List<Enterprise>(enterprises);
    }

    internal static Result<Manager> Create(ManagerCreateData data)
    {
        // Check for duplicate enterprises
        bool hasEnterpriseDuplicates =  data.Enterprises.GroupBy(e => e.Id)
            .Where(group => group.Count() > 1)
            .Any();

        if (hasEnterpriseDuplicates)
        {
            return Result.Fail(new ManagerDomainError(ManagerDomainErrorCodes.DuplicateEnterprises));
        }

        Manager manager = new Manager(
            data.Id,
            data.IdentityUserId,
            data.Enterprises);

        return Result.Ok(manager);
    }
}