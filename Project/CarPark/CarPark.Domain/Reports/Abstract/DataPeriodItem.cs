using CarPark.Shared.DateTimes;

namespace CarPark.Reports.Abstract;

public class DataPeriodItem<TData>
{
    public UtcDateTimeOffset Date { get; private init; }

    public TData Data { get; private init; }

    public DataPeriodItem(UtcDateTimeOffset date, TData data)
    {
        Data = data;
        Date = date;
    }
}