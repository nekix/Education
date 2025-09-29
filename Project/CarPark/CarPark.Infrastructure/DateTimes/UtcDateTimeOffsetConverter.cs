using CarPark.Shared.DateTimes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CarPark.DateTimes;

public class UtcDateTimeOffsetConverter : ValueConverter<UtcDateTimeOffset, DateTimeOffset>
{
    public UtcDateTimeOffsetConverter() 
        : base(dt => dt.Value, dt => new UtcDateTimeOffset(dt))
    {
    }
}