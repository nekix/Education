namespace CarPark.TimeZones;

public sealed class TzInfo
{
    public int Id { get; private init; }

    public string IanaTzId { get; private init; }

    public string WindowsTzId { get; private init; }

    public TzInfo(string ianaTzId, string windowsTzId)
    {
        IanaTzId = ianaTzId;
        WindowsTzId = windowsTzId;
    }
}