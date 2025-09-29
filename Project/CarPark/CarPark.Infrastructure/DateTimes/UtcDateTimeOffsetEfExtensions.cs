using CarPark.Shared.DateTimes;
using Microsoft.EntityFrameworkCore;

namespace CarPark.DateTimes;

public static class UtcDateTimeOffsetEfExtensions
{
    public static void UseUtcDateTimeOffset(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<UtcDateTimeOffset>()
            .HaveConversion<UtcDateTimeOffsetConverter>();
    }
}