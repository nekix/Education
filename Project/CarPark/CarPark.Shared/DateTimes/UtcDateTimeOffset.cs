namespace CarPark.Shared.DateTimes;

public readonly record struct UtcDateTimeOffset
{
    public UtcDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        Value = dateTimeOffset;
    }

    private readonly DateTimeOffset _value;
    public DateTimeOffset Value
    {
        get => _value;
        init
        {
            // Проверка на UTC
            _value = value.TotalOffsetMinutes != 0 
                ? value.ToUniversalTime() 
                : value;
        }
    }

    public static implicit operator DateTimeOffset(UtcDateTimeOffset d) => d.Value;
}