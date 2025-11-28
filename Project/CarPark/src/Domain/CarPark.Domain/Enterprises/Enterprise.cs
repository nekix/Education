using CarPark.Managers;
using CarPark.TimeZones;
using FluentResults;

namespace CarPark.Enterprises;

public sealed class Enterprise
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string LegalAddress { get; private set; }

    public List<Manager> Managers { get; private set; }

    public TzInfo? TimeZone { get; private set; }

    #pragma warning disable CS8618
    [Obsolete("Only for ORM and deserialization! Do not use!")]
    private Enterprise()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    private Enterprise(
        Guid id,
        string name,
        string legalAddress,
        TzInfo? timeZone)
    {
        Id = id;
        Name = name;
        LegalAddress = legalAddress;
        TimeZone = timeZone;
        Managers = new List<Manager>();
    }

    internal static Result<Enterprise> Create(EnterpriseCreateData data)
    {
        Enterprise enterprise = new Enterprise(
            data.Id,
            data.Name,
            data.LegalAddress,
            data.TimeZone);

        return Result.Ok(enterprise);
    }

    internal static Result Update(Enterprise enterprise, EnterpriseUpdateData data)
    {
        enterprise.Name = data.Name;
        enterprise.LegalAddress = data.LegalAddress;
        enterprise.TimeZone = data.TimeZone;

        return Result.Ok();
    }
}