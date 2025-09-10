using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CarPark.Shared.DateTimes;

public class UtcDateTimeOffsetConverter : ValueConverter<UtcDateTimeOffset, DateTimeOffset>
{
    public UtcDateTimeOffsetConverter() 
        : base(dt => dt.Value, dt => new UtcDateTimeOffset(dt))
    {
    }
}