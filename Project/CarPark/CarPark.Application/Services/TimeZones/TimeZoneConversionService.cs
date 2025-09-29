using CarPark.TimeZones;

namespace CarPark.Services.TimeZones;

public class TimeZoneConversionService : ITimeZoneConversionService
{
    private readonly LocalIcuTimezoneService _timezoneService;

    public TimeZoneConversionService(LocalIcuTimezoneService timezoneService)
    {
        _timezoneService = timezoneService;
    }

    /// <summary>
    /// Converts DateTimeOffset to enterprise timezone or UTC if enterprise timezone is not specified
    /// </summary>
    /// <param name="dateTimeOffset">The original DateTimeOffset</param>
    /// <param name="enterpriseTimeZone">Enterprise timezone info, can be null</param>
    /// <returns>DateTimeOffset converted to enterprise timezone or UTC</returns>
    public DateTimeOffset ConvertToEnterpriseTimeZone(DateTimeOffset dateTimeOffset, TzInfo? enterpriseTimeZone)
    {
        if (enterpriseTimeZone == null)
        {
            // Return as UTC if no enterprise timezone is specified
            return dateTimeOffset.ToUniversalTime();
        }

        try
        {
            // Convert to enterprise timezone using Windows timezone ID
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(enterpriseTimeZone.WindowsTzId);
            return TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo);
        }
        catch (TimeZoneNotFoundException)
        {
            // Fallback to UTC if timezone conversion fails
            return dateTimeOffset.ToUniversalTime();
        }
    }

    /// <summary>
    /// Converts DateTimeOffset for UI display considering client timezone preference
    /// Logic: Show in client timezone if available, otherwise in enterprise timezone, otherwise UTC
    /// </summary>
    /// <param name="dateTimeOffset">The original DateTimeOffset</param>
    /// <param name="enterpriseTimeZone">Enterprise timezone info, can be null</param>
    /// <param name="clientTimeZoneOffset">Client timezone offset in minutes from UTC (from JavaScript)</param>
    /// <returns>DateTimeOffset prepared for client display</returns>
    public DateTimeOffset ConvertForClientDisplay(DateTimeOffset dateTimeOffset, TzInfo? enterpriseTimeZone, int? clientTimeZoneOffset = null)
    {
        // If client timezone offset is provided, convert to client timezone
        if (clientTimeZoneOffset.HasValue)
        {
            TimeSpan clientOffset = TimeSpan.FromMinutes(-clientTimeZoneOffset.Value); // JavaScript offset is inverted
            return dateTimeOffset.ToOffset(clientOffset);
        }

        // If no client timezone but enterprise timezone exists, convert to enterprise timezone
        if (enterpriseTimeZone != null)
        {
            return ConvertToEnterpriseTimeZone(dateTimeOffset, enterpriseTimeZone);
        }

        // Default to UTC
        return dateTimeOffset.ToUniversalTime();
    }

    /// <summary>
    /// Gets timezone display information for UI
    /// </summary>
    /// <param name="enterpriseTimeZone">Enterprise timezone info</param>
    /// <param name="locale">Locale for display name</param>
    /// <returns>Timezone display information</returns>
    public TimeZoneDisplayInfo GetTimeZoneDisplayInfo(TzInfo? enterpriseTimeZone, string locale = "ru-RU")
    {
        if (enterpriseTimeZone == null)
        {
            return new TimeZoneDisplayInfo
            {
                DisplayName = "UTC",
                Offset = TimeSpan.Zero,
                IanaId = "UTC"
            };
        }

        try
        {
            string? displayName = _timezoneService.GetTimeZoneDisplayName(enterpriseTimeZone.IanaTzId, DisplayNameType.Standard, locale);
            TimeSpan offset = _timezoneService.GetTimeZoneOffset(enterpriseTimeZone.IanaTzId);

            return new TimeZoneDisplayInfo
            {
                DisplayName = displayName ?? enterpriseTimeZone.IanaTzId,
                Offset = offset,
                IanaId = enterpriseTimeZone.IanaTzId
            };
        }
        catch
        {
            return new TimeZoneDisplayInfo
            {
                DisplayName = "UTC",
                Offset = TimeSpan.Zero,
                IanaId = "UTC"
            };
        }
    }
}

public class TimeZoneDisplayInfo
{
    public required string DisplayName { get; set; }
    public required TimeSpan Offset { get; set; }
    public required string IanaId { get; set; }
}