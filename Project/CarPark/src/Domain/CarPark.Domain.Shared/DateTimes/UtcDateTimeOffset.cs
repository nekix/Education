using System.Numerics;

namespace CarPark.DateTimes;

public readonly record struct UtcDateTimeOffset : IComparable<DateTimeOffset>, IComparable<UtcDateTimeOffset>
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
            // Проверка на UTC.
            _value = value.TotalOffsetMinutes != 0 
                ? value.ToUniversalTime() 
                : value;
        }
    }

    public static implicit operator DateTimeOffset(UtcDateTimeOffset d) => d.Value;

    /// <inheritdoc cref="IComparisonOperators{TSelf,TOther,TResult}.op_LessThan(TSelf, TOther)" />
    public static bool operator <(UtcDateTimeOffset left, UtcDateTimeOffset right) =>
        left.Value.UtcDateTime < right.Value.UtcDateTime;

    /// <inheritdoc cref="IComparisonOperators{TSelf,TOther,TResult}.op_LessThanOrEqual(TSelf, TOther)" />
    public static bool operator <=(UtcDateTimeOffset left, UtcDateTimeOffset right) =>
        left.Value.UtcDateTime <= right.Value.UtcDateTime;

    /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
    public static bool operator >(UtcDateTimeOffset left, UtcDateTimeOffset right) =>
        left.Value.UtcDateTime > right.Value.UtcDateTime;

    /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
    public static bool operator >=(UtcDateTimeOffset left, UtcDateTimeOffset right) =>
        left.Value.UtcDateTime >= right.Value.UtcDateTime;

    public int CompareTo(DateTimeOffset other)
    {
        return Value.CompareTo(other);
    }

    public int CompareTo(UtcDateTimeOffset other)
    {
        return Value.CompareTo(other.Value);
    }
}