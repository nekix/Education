using KGySoft.Resources;
using Icu;
using System.Runtime.InteropServices;
using System.Reflection;

namespace CarPark.TimeZoneResxGenerator;

internal class Program
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal delegate int ucal_getTimeZoneDisplayNameDelegate(
        IntPtr cal,
        int type,
        [MarshalAs(UnmanagedType.LPStr)] string locale,
        IntPtr result,
        int resultLength,
        out ErrorCode status);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal delegate IntPtr ucal_openDelegate(
        IntPtr zoneID,
        int len,
        [MarshalAs(UnmanagedType.LPStr)] string locale,
        int type,
        out ErrorCode status);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ucal_closeDelegate(IntPtr cal);

    // UCalendarDisplayNameType constants
    private const int UCAL_STANDARD = 0;
    private const int UCAL_SHORT_STANDARD = 1;
    private const int UCAL_DST = 2;
    private const int UCAL_SHORT_DST = 3;

    // UCalendarType constants
    private const int UCAL_TRADITIONAL = 0;
    private const int UCAL_GREGORIAN = 1;

    static void Main(string[] args)
    {
        Icu.Wrapper.Init();

        

        IReadOnlyCollection<string> ianaIds = GetCanonicalLocationIanaIds();


        ianaIds.ToList().ForEach(tzid =>
        {
            var name1 = GetTimeZoneDisplayName(tzid, UCAL_STANDARD, "ru_RU");
            var name2 = GetTimeZoneDisplayName(tzid, UCAL_SHORT_STANDARD, "ru_RU");
            var name3 = GetTimeZoneDisplayName(tzid, UCAL_SHORT_DST, "ru_RU");
            var name4 = GetTimeZoneDisplayName(tzid, UCAL_DST, "ru_RU");

            Console.WriteLine($"{tzid} || {name1} || {name2} || {name3} || {name4}");
        });
        
        //// Тестируем различные типы отображения и локали
        //Console.WriteLine($"Testing TimeZone: {testTimeZone}");
        //Console.WriteLine($"UCAL_STANDARD (en_US): {GetTimeZoneDisplayName(testTimeZone, UCAL_STANDARD, "en_US")}");
        //Console.WriteLine($"UCAL_SHORT_STANDARD (en_US): {GetTimeZoneDisplayName(testTimeZone, UCAL_SHORT_STANDARD, "en_US")}");
        //Console.WriteLine($"UCAL_DST (en_US): {GetTimeZoneDisplayName(testTimeZone, UCAL_DST, "en_US")}");
        //Console.WriteLine($"UCAL_SHORT_DST (en_US): {GetTimeZoneDisplayName(testTimeZone, UCAL_SHORT_DST, "en_US")}");
        //Console.WriteLine($"UCAL_STANDARD (ru_RU): {GetTimeZoneDisplayName(testTimeZone, UCAL_STANDARD, "ru_RU")}");
        
        //// Тестируем с временной зоной, которая имеет DST
        //string dstTimeZone = "America/New_York";
        //Console.WriteLine($"\nTesting DST TimeZone: {dstTimeZone}");
        //Console.WriteLine($"UCAL_STANDARD: {GetTimeZoneDisplayName(dstTimeZone, UCAL_STANDARD, "en_US")}");
        //Console.WriteLine($"UCAL_DST: {GetTimeZoneDisplayName(dstTimeZone, UCAL_DST, "en_US")}");

        //IReadOnlyCollection<KeyValuePair<string, string>> ianaToWindowsIdsMap = MapIanaIdsToWindowsIds(ianaIds);

        //using FileStream fs = File.Create("iana_windows_timezones_info.txt");
        //using ResXResourceWriter writer = new ResXResourceWriter(fs);

        //IReadOnlyCollection<TimeZoneInfo> ianaTimeZoneInfo = LoadTimeZoneInfoFromTextFileV2("iana_timezoneinfo_snapshot.txt");

        //SaveTimeZoneInfoToResxFile("TimeZones.resx", ianaTimeZoneInfo);
    }

    public static string? GetTimeZoneDisplayName(string timeZoneId, int displayType, string locale)
    {
        // Создаем UChar массив из timeZoneId
        IntPtr zoneIdPtr = Marshal.StringToHGlobalUni(timeZoneId);
        
        try
        {
            // Создаем календарь с указанной временной зоной
            IntPtr cal = CreateCalendar(zoneIdPtr, timeZoneId.Length, locale, UCAL_GREGORIAN);
            if (cal == IntPtr.Zero)
                return null;

            try
            {
                return GetUnicodeString((ptr, length) =>
                {
                    int actualLength = GetTimeZoneDisplayNameNative(cal, displayType, locale, ptr, length, out ErrorCode errorCode);
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

    public static Dictionary<string, string?> GetAllTimeZoneDisplayNames(string timeZoneId, string locale)
    {
        return new Dictionary<string, string?>
        {
            ["Standard"] = GetTimeZoneDisplayName(timeZoneId, UCAL_STANDARD, locale),
            ["ShortStandard"] = GetTimeZoneDisplayName(timeZoneId, UCAL_SHORT_STANDARD, locale),
            ["DST"] = GetTimeZoneDisplayName(timeZoneId, UCAL_DST, locale),
            ["ShortDST"] = GetTimeZoneDisplayName(timeZoneId, UCAL_SHORT_DST, locale)
        };
    }

    public static string? GetDisplayString(string id)
    {
        return GetUnicodeString((ptr, length) =>
        {
            length = get_ucal_getWindowsTimeZoneId(id, ptr, length, out ErrorCode errorCode);
            return new Tuple<ErrorCode, int>(errorCode, length);
        });
    }

    public static IntPtr GetIcuI18NLibHandle()
    {
        // Загружаем сборку, где находится класс NativeMethods
        var asm = Assembly.Load("icu.net"); // имя dll без .dll

        // Получаем тип NativeMethods
        var nativeMethodsType = asm.GetType("Icu.NativeMethods");
        if (nativeMethodsType == null)
            throw new InvalidOperationException("Icu.NativeMethods не найден.");

        // Получаем PropertyInfo
        var prop = nativeMethodsType.GetProperty(
            "IcuI18NLibHandle",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        if (prop == null)
            throw new InvalidOperationException("Свойство IcuI18NLibHandle не найдено.");

        // Возвращаем значение IntPtr
        return (IntPtr)prop.GetValue(null);
    }

    public static int GetIcuVersion()
    {
        // Загружаем сборку, где находится класс NativeMethods
        var asm = Assembly.Load("icu.net"); // имя dll без .dll

        // Получаем тип NativeMethods
        var nativeMethodsType = asm.GetType("Icu.NativeMethods");
        if (nativeMethodsType == null)
            throw new InvalidOperationException("Тип Icu.NativeMethods не найден.");

        // Получаем FieldInfo для приватного статического поля
        var field = nativeMethodsType.GetField(
            "IcuVersion",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        if (field == null)
            throw new InvalidOperationException("Поле IcuVersion не найдено.");

        // Возвращаем значение
        return (int)field.GetValue(null);
    }

    private static IntPtr CreateCalendar(IntPtr zoneId, int zoneIdLength, string locale, int calendarType)
    {
        var method = GetMethod<ucal_openDelegate>(GetIcuI18NLibHandle(), "ucal_open");
        return method(zoneId, zoneIdLength, locale, calendarType, out ErrorCode status);
    }

    private static void CloseCalendar(IntPtr cal)
    {
        var method = GetMethod<ucal_closeDelegate>(GetIcuI18NLibHandle(), "ucal_close");
        method(cal);
    }

    private static int GetTimeZoneDisplayNameNative(IntPtr cal, int displayType, string locale, IntPtr result, int resultLength, out ErrorCode status)
    {
        var method = GetMethod<ucal_getTimeZoneDisplayNameDelegate>(GetIcuI18NLibHandle(), "ucal_getTimeZoneDisplayName");
        return method(cal, displayType, locale, result, resultLength, out status);
    }

    public static int get_ucal_getWindowsTimeZoneId(string id,
        IntPtr winId,
        int winIdCapacity,
        out ErrorCode status)
    {
        // Этот метод был для другой функции, возможно нужно его переименовать
        // Пока оставим как есть для совместимости
        status = ErrorCode.NoErrors;
        return 0;
    }

    private static T GetMethod<T>(IntPtr handle, string methodName, bool missingInMinimal = false) where T : class
    {
        IntPtr methodPointer;

        var versionedMethodName = $"{methodName}_{GetIcuVersion()}";
#if NET6_0_OR_GREATER
        try
        {
            NativeLibrary.TryGetExport(handle, versionedMethodName, out methodPointer);
        }
        catch (DllNotFoundException)
        {
            methodPointer = IntPtr.Zero;
        }
#else
			methodPointer = IsWindows
				? GetProcAddress(handle, versionedMethodName)
				: IsMac
				? IntPtr.Zero
				: dlsym(handle, versionedMethodName);
#endif

        // Some systems (eg. Tizen) don't use methods with IcuVersion suffix
        if (methodPointer == IntPtr.Zero)
        {
#if NET6_0_OR_GREATER
            try
            {
                NativeLibrary.TryGetExport(handle, methodName, out methodPointer);
            }
            catch (DllNotFoundException) { }
            ;
#else
				methodPointer = IsWindows
					? GetProcAddress(handle, methodName)
					: IsMac
					? IntPtr.Zero
					: dlsym(handle, methodName);
#endif
        }

        if (methodPointer != IntPtr.Zero)
        {
            // NOTE: Starting in .NET 4.5.1, Marshal.GetDelegateForFunctionPointer(IntPtr, Type) is obsolete.
#if NET40
				return Marshal.GetDelegateForFunctionPointer(
					methodPointer, typeof(T)) as T;
#else
            return Marshal.GetDelegateForFunctionPointer<T>(methodPointer);
#endif
        }
        if (missingInMinimal)
        {
            throw new MissingMemberException(
                "Do you have the full version of ICU installed? " +
                $"The method '{methodName}' is not included in the minimal version of ICU.");
        }
        return default(T);
    }

    public static string? GetUnicodeString(Func<IntPtr, int, Tuple<ErrorCode, int>> lambda,
        int initialLength = 255)
    {
        return GetString(lambda, true, initialLength);
    }

    public static string? GetString(Func<IntPtr, int, Tuple<ErrorCode, int>> lambda,
        bool isUnicodeString = false, int initialLength = 255)
    {
        var length = initialLength;
        var resPtr = Marshal.AllocCoTaskMem(length * 2);
        try
        {
            var (err, outLength) = lambda(resPtr, length);
            if (err != ErrorCode.BUFFER_OVERFLOW_ERROR && err != ErrorCode.STRING_NOT_TERMINATED_WARNING)
                ThrowIfError(err);
            if (outLength >= length)
            {
                err = ErrorCode.NoErrors; // ignore possible U_BUFFER_OVERFLOW_ERROR or STRING_NOT_TERMINATED_WARNING
                Marshal.FreeCoTaskMem(resPtr);
                length = outLength + 1; // allow room for the terminating NUL (FWR-505)
                resPtr = Marshal.AllocCoTaskMem(length * 2);
                (err, outLength) = lambda(resPtr, length);
            }

            ThrowIfError(err);

            if (outLength < 0)
                return null;

            var result = isUnicodeString
                ? Marshal.PtrToStringUni(resPtr)
                : Marshal.PtrToStringAnsi(resPtr);

            // Strip any garbage left over at the end of the string.
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
            throw new Exception($"ErrorCode: {err}");
    }

    private static IReadOnlyCollection<string> GetCanonicalLocationIanaIds()
    {
        var t = Icu.TimeZone
            .GetTimeZones(USystemTimeZoneType.CanonicalLocation, null)
            .Select(x => new { id = x.Id, off = x.GetDstSavings() })
            .ToList();

        return Icu.TimeZone
            .GetTimeZones(USystemTimeZoneType.CanonicalLocation, null)
            .Select(tz => tz.Id)
            .OrderBy(id => id)
            .ToList()
            .AsReadOnly();
    }

    private static IReadOnlyCollection<KeyValuePair<string, string>> MapIanaIdsToWindowsIds(IEnumerable<string> ianaIds)
    {
        return ianaIds.Select(id => new KeyValuePair<string, string>(id, Icu.TimeZone.GetWindowsId(id)))
            .ToList()
            .AsReadOnly();
    }

    private static void PrintTimeZonesInfo(IReadOnlyCollection<TimeZoneInfo> timeZones)
    {
        foreach (TimeZoneInfo timeZone in timeZones)
        {
            Console.WriteLine(timeZone.DisplayName);
            Console.WriteLine(timeZone.Id);
            Console.WriteLine(timeZone.DaylightName);
            Console.WriteLine(timeZone.SupportsDaylightSavingTime);
            Console.WriteLine(timeZone.BaseUtcOffset);
            Console.WriteLine(timeZone.StandardName);
            Console.WriteLine();
        }
    }



    private static void SaveTimeZoneInfoToResxFile(string filename, IReadOnlyCollection<TimeZoneInfo> timeZones)
    {
        using FileStream fs = File.Create(filename);
        using ResXResourceWriter writer = new ResXResourceWriter(fs);

        foreach (TimeZoneInfo timeZoneInfo in timeZones)
            writer.AddResource(timeZoneInfo.Id, timeZoneInfo.ToSerializedString());
    }

    private static IReadOnlyCollection<TimeZoneInfo> LoadTimeZoneInfoFromTextFileV2(string filename)
    {
        using FileStream fs = File.OpenRead(filename);
        using StreamReader sr = new StreamReader(fs);
        List<TimeZoneInfo> timeZoneInfos = new List<TimeZoneInfo>();

        while (!sr.EndOfStream)
        {
            string? serializedStr = sr.ReadLine();
            
            if (serializedStr != null)
            {
                string tmzId = serializedStr.Split(';')[0];
                TimeZoneInfo.TryConvertIanaIdToWindowsId(tmzId, out string? windowsId);

                if (windowsId != null)
                {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsId);
                    timeZoneInfos.Add(timeZone);
                }
            }
        }

        return timeZoneInfos;
    }

    private static IReadOnlyCollection<TimeZoneInfo> LoadTimeZoneInfoFromTextFile(string filename)
    {
        using FileStream fs = File.OpenRead(filename);
        using StreamReader sr = new StreamReader(fs);
        List<TimeZoneInfo> timeZoneInfo = new List<TimeZoneInfo>();

        while (!sr.EndOfStream)
        {
            string? serializedStr = sr.ReadLine();
            if (serializedStr != null)
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FromSerializedString(serializedStr);
                timeZoneInfo.Add(timeZone);
            }
        }

        return timeZoneInfo;
    }

    private static void SaveTimeZoneInfoToTextFile(string filename, IReadOnlyCollection<TimeZoneInfo> timeZones)
    {
        using FileStream fs = File.Create(filename);
        using StreamWriter sw = new StreamWriter(fs);

        foreach (TimeZoneInfo timeZoneInfo in timeZones)
            sw.WriteLine(timeZoneInfo.ToSerializedString());
    }
}