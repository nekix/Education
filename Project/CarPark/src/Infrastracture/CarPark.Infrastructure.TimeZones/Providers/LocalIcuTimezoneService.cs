using System.Reflection;
using System.Runtime.InteropServices;
using Icu;

namespace CarPark.TimeZones.Providers;

public class LocalIcuTimezoneService : IDisposable
{
    private bool _disposedValue;
    
    // Кеш для display names: IANA ID -> DisplayType -> Locale -> DisplayName
    private readonly Dictionary<string, Dictionary<DisplayNameType, Dictionary<string, string>>> _displayNameCache;
    
    // Делегаты для работы с ICU
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private delegate int ucal_getTimeZoneDisplayNameDelegate(
        nint cal,
        int type,
        [MarshalAs(UnmanagedType.LPStr)] string locale,
        nint result,
        int resultLength,
        out ErrorCode status);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private delegate nint ucal_openDelegate(
        nint zoneID,
        int len,
        [MarshalAs(UnmanagedType.LPStr)] string locale,
        int type,
        out ErrorCode status);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void ucal_closeDelegate(nint cal);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ucal_getDelegate(
        nint cal,
        int field,
        out ErrorCode status);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void ucal_setMillisDelegate(
        nint cal,
        double dateTime,
        out ErrorCode status);
    
    // Константы для типов отображения
    private const int UCAL_STANDARD = 0;
    private const int UCAL_SHORT_STANDARD = 1;
    private const int UCAL_DST = 2;
    private const int UCAL_SHORT_DST = 3;
    private const int UCAL_GREGORIAN = 1;
    
    // Константы для полей календаря
    private const int UCAL_ZONE_OFFSET = 15;
    private const int UCAL_DST_OFFSET = 16;

    public LocalIcuTimezoneService()
    {
        _displayNameCache = new Dictionary<string, Dictionary<DisplayNameType, Dictionary<string, string>>>();
        
        ErrorCode error = Wrapper.Init();

        if (error != ErrorCode.NoErrors)
        {
            throw new TzInfoServiceException($"Не удалось инициализировать внутренний сервис Icu. ErrorCode: {error}. ErrorCodeType: {typeof(ErrorCode).FullName}");
        }
    }

    public IReadOnlyCollection<string> GetAvailableIanaIds()
    {
        return Icu.TimeZone
            .GetTimeZones(USystemTimeZoneType.CanonicalLocation, null)
            .Select(tz => tz.Id)
            .OrderBy(id => id)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> MapIanaIdsToWindowsIds(IEnumerable<string> ianaIds)
    {
        return ianaIds.Select(id => new KeyValuePair<string, string>(id, Icu.TimeZone.GetWindowsId(id)))
            .ToList()
            .AsReadOnly();
    }

    public string? GetTimeZoneDisplayName(string ianaId, DisplayNameType displayType = DisplayNameType.Standard, string locale = "en_US")
    {
        if (string.IsNullOrEmpty(ianaId))
            return null;

        // Проверяем кеш
        if (_displayNameCache.TryGetValue(ianaId, out Dictionary<DisplayNameType, Dictionary<string, string>>? typeCache) &&
            typeCache.TryGetValue(displayType, out Dictionary<string, string>? localeCache) &&
            localeCache.TryGetValue(locale, out string? cachedName))
        {
            return cachedName;
        }

        // Получаем display name из ICU
        string? displayName = GetTimeZoneDisplayNameFromICU(ianaId, displayType, locale);
        
        // Кешируем результат
        if (displayName != null)
        {
            CacheDisplayName(ianaId, displayType, locale, displayName);
        }

        return displayName;
    }

    public Dictionary<DisplayNameType, string?> GetAllTimeZoneDisplayNames(string ianaId, string locale = "en_US")
    {
        Dictionary<DisplayNameType, string?> result = new Dictionary<DisplayNameType, string?>();
        
        foreach (DisplayNameType displayType in Enum.GetValues<DisplayNameType>())
        {
            result[displayType] = GetTimeZoneDisplayName(ianaId, displayType, locale);
        }
        
        return result;
    }

    public TimeSpan GetTimeZoneOffset(string ianaId, DateTime? dateTime = null)
    {
        if (string.IsNullOrEmpty(ianaId))
            return TimeSpan.Zero;

        nint zoneIdPtr = Marshal.StringToHGlobalUni(ianaId);
        
        try
        {
            nint cal = CreateCalendar(zoneIdPtr, ianaId.Length, "en_US", UCAL_GREGORIAN);
            if (cal == nint.Zero)
                return TimeSpan.Zero;

            try
            {
                // Устанавливаем время, для которого нужно получить смещение
                if (dateTime.HasValue)
                {
                    SetCalendarTime(cal, dateTime.Value);
                }

                // Получаем смещение зоны (без учета DST) и DST смещение
                int zoneOffset = GetCalendarField(cal, UCAL_ZONE_OFFSET);
                int dstOffset = GetCalendarField(cal, UCAL_DST_OFFSET);
                
                // Общее смещение = смещение зоны + DST смещение
                int totalOffsetMs = zoneOffset + dstOffset;
                
                return TimeSpan.FromMilliseconds(totalOffsetMs);
            }
            finally
            {
                CloseCalendar(cal);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(zoneIdPtr);
        }
    }

    public string GetTimeZoneOffsetString(string ianaId, DateTime? dateTime = null)
    {
        TimeSpan offset = GetTimeZoneOffset(ianaId, dateTime);
        
        if (offset == TimeSpan.Zero)
            return "UTC";
            
        string sign = offset < TimeSpan.Zero ? "-" : "+";
        TimeSpan absOffset = offset.Duration();
        
        return $"UTC{sign}{absOffset.Hours:D2}:{absOffset.Minutes:D2}";
    }

    private void CacheDisplayName(string ianaId, DisplayNameType displayType, string locale, string displayName)
    {
        if (!_displayNameCache.TryGetValue(ianaId, out Dictionary<DisplayNameType, Dictionary<string, string>>? typeCache))
        {
            typeCache = new Dictionary<DisplayNameType, Dictionary<string, string>>();
            _displayNameCache[ianaId] = typeCache;
        }

        if (!typeCache.TryGetValue(displayType, out Dictionary<string, string>? localeCache))
        {
            localeCache = new Dictionary<string, string>();
            typeCache[displayType] = localeCache;
        }

        localeCache[locale] = displayName;
    }

    private string? GetTimeZoneDisplayNameFromICU(string timeZoneId, DisplayNameType displayType, string locale)
    {
        nint zoneIdPtr = Marshal.StringToHGlobalUni(timeZoneId);
        
        try
        {
            nint cal = CreateCalendar(zoneIdPtr, timeZoneId.Length, locale, UCAL_GREGORIAN);
            if (cal == nint.Zero)
                return null;

            try
            {
                return GetUnicodeString((ptr, length) =>
                {
                    int icuDisplayType = ConvertDisplayTypeToICU(displayType);
                    int actualLength = GetTimeZoneDisplayNameNative(cal, icuDisplayType, locale, ptr, length, out ErrorCode errorCode);
                    return new Tuple<ErrorCode, int>(errorCode, actualLength);
                });
            }
            finally
            {
                CloseCalendar(cal);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(zoneIdPtr);
        }
    }

    private int ConvertDisplayTypeToICU(DisplayNameType displayType)
    {
        return displayType switch
        {
            DisplayNameType.Standard => UCAL_STANDARD,
            DisplayNameType.ShortStandard => UCAL_SHORT_STANDARD,
            DisplayNameType.DST => UCAL_DST,
            DisplayNameType.ShortDST => UCAL_SHORT_DST,
            _ => UCAL_STANDARD
        };
    }

    private nint CreateCalendar(nint zoneId, int zoneIdLength, string locale, int calendarType)
    {
        ucal_openDelegate method = GetMethod<ucal_openDelegate>(GetIcuI18NLibHandle(), "ucal_open");
        return method(zoneId, zoneIdLength, locale, calendarType, out ErrorCode status);
    }

    private void CloseCalendar(nint cal)
    {
        ucal_closeDelegate method = GetMethod<ucal_closeDelegate>(GetIcuI18NLibHandle(), "ucal_close");
        method(cal);
    }

    private int GetTimeZoneDisplayNameNative(nint cal, int displayType, string locale, nint result, int resultLength, out ErrorCode status)
    {
        ucal_getTimeZoneDisplayNameDelegate method = GetMethod<ucal_getTimeZoneDisplayNameDelegate>(GetIcuI18NLibHandle(), "ucal_getTimeZoneDisplayName");
        return method(cal, displayType, locale, result, resultLength, out status);
    }

    private void SetCalendarTime(nint cal, DateTime dateTime)
    {
        // Конвертируем DateTime в миллисекунды с эпохи Unix (1 января 1970)
        DateTimeOffset dto = new DateTimeOffset(dateTime, TimeSpan.Zero);
        double milliseconds = dto.ToUnixTimeMilliseconds();
        
        ucal_setMillisDelegate method = GetMethod<ucal_setMillisDelegate>(GetIcuI18NLibHandle(), "ucal_setMillis");
        method(cal, milliseconds, out ErrorCode status);
        ThrowIfError(status);
    }

    private int GetCalendarField(nint cal, int field)
    {
        ucal_getDelegate method = GetMethod<ucal_getDelegate>(GetIcuI18NLibHandle(), "ucal_get");
        int result = method(cal, field, out ErrorCode status);
        ThrowIfError(status);
        return result;
    }

    private nint GetIcuI18NLibHandle()
    {
        Assembly asm = Assembly.Load("icu.net");
        Type? nativeMethodsType = asm.GetType("Icu.NativeMethods");
        if (nativeMethodsType == null)
            throw new InvalidOperationException("Icu.NativeMethods не найден.");

        PropertyInfo? prop = nativeMethodsType.GetProperty(
            "IcuI18NLibHandle",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        if (prop == null)
            throw new InvalidOperationException("Свойство IcuI18NLibHandle не найдено.");

        return (nint)prop.GetValue(null)!;
    }

    private int GetIcuVersion()
    {
        Assembly asm = Assembly.Load("icu.net");
        Type? nativeMethodsType = asm.GetType("Icu.NativeMethods");
        if (nativeMethodsType == null)
            throw new InvalidOperationException("Тип Icu.NativeMethods не найден.");

        FieldInfo? field = nativeMethodsType.GetField(
            "IcuVersion",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        if (field == null)
            throw new InvalidOperationException("Поле IcuVersion не найдено.");

        return (int)field.GetValue(null)!;
    }

    private T GetMethod<T>(nint handle, string methodName, bool missingInMinimal = false) where T : class
    {
        nint methodPointer;

        string versionedMethodName = $"{methodName}_{GetIcuVersion()}";
#if NET6_0_OR_GREATER
        try
        {
            NativeLibrary.TryGetExport(handle, versionedMethodName, out methodPointer);
        }
        catch (DllNotFoundException)
        {
            methodPointer = nint.Zero;
        }
#else
        methodPointer = IntPtr.Zero; // Для простоты в старых версиях .NET
#endif

        if (methodPointer == nint.Zero)
        {
#if NET6_0_OR_GREATER
            try
            {
                NativeLibrary.TryGetExport(handle, methodName, out methodPointer);
            }
            catch (DllNotFoundException) { }
#endif
        }

        if (methodPointer != nint.Zero)
        {
            return Marshal.GetDelegateForFunctionPointer<T>(methodPointer);
        }
        
        if (missingInMinimal)
        {
            throw new MissingMemberException(
                "Do you have the full version of ICU installed? " +
                $"The method '{methodName}' is not included in the minimal version of ICU.");
        }
        
        return default!;
    }

    private string? GetUnicodeString(Func<nint, int, Tuple<ErrorCode, int>> lambda, int initialLength = 255)
    {
        return GetString(lambda, true, initialLength);
    }

    private string? GetString(Func<nint, int, Tuple<ErrorCode, int>> lambda, bool isUnicodeString = false, int initialLength = 255)
    {
        int length = initialLength;
        nint resPtr = Marshal.AllocCoTaskMem(length * 2);
        try
        {
            (ErrorCode err, int outLength) = lambda(resPtr, length);
            if (err != ErrorCode.BUFFER_OVERFLOW_ERROR && err != ErrorCode.STRING_NOT_TERMINATED_WARNING)
                ThrowIfError(err);
            if (outLength >= length)
            {
                err = ErrorCode.NoErrors;
                Marshal.FreeCoTaskMem(resPtr);
                length = outLength + 1;
                resPtr = Marshal.AllocCoTaskMem(length * 2);
                (err, outLength) = lambda(resPtr, length);
            }

            ThrowIfError(err);

            if (outLength < 0)
                return null;

            string? result = isUnicodeString
                ? Marshal.PtrToStringUni(resPtr)
                : Marshal.PtrToStringAnsi(resPtr);

            if (err == ErrorCode.STRING_NOT_TERMINATED_WARNING && result != null)
                return result.Substring(0, outLength);
            return result;
        }
        finally
        {
            Marshal.FreeCoTaskMem(resPtr);
        }
    }

    private static void ThrowIfError(ErrorCode err)
    {
        if (err != ErrorCode.NoErrors)
            throw new TzInfoServiceException($"ICU ErrorCode: {err}");
    }

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: освободить управляемое состояние (управляемые объекты)
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            Wrapper.Cleanup();

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
