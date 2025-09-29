using CarPark.TimeZones;

namespace CarPark.Services.TimeZones;

public interface ITimeZoneConversionService
{
    DateTimeOffset ConvertToEnterpriseTimeZone(DateTimeOffset dateTimeOffset, TzInfo? enterpriseTimeZone);

    DateTimeOffset ConvertForClientDisplay(DateTimeOffset dateTimeOffset, TzInfo? enterpriseTimeZone, int? clientTimeZoneOffset = null);

    TimeZoneDisplayInfo GetTimeZoneDisplayInfo(TzInfo? enterpriseTimeZone, string locale = "ru-RU");
}