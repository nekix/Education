﻿using Microsoft.EntityFrameworkCore;

namespace CarPark.Shared.DateTimes;

public static class UtcDateTimeOffsetEfExtensions
{
    public static void UseUtcDateTimeOffset(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<UtcDateTimeOffset>()
            .HaveConversion<UtcDateTimeOffsetConverter>();
    }
}