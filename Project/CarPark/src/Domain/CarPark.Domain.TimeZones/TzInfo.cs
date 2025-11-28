namespace CarPark.TimeZones;

public sealed class TzInfo
{
    public Guid Id { get; private init; }

    public string IanaTzId { get; private init; }

    public string WindowsTzId { get; private init; }

    #pragma warning disable CS8618
    [Obsolete("Only for ORM, fabric method and deserialization! Do not use!")]
    private TzInfo()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    public TzInfo(Guid id, string ianaTzId, string windowsTzId)
    {
        Id = id;
        IanaTzId = ianaTzId;
        WindowsTzId = windowsTzId;
    }
}