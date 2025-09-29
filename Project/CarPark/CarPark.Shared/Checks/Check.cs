using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CarPark.Shared.Checks;

[DebuggerStepThrough]
public static class Check
{
    public static T NotNull<T>(
        [System.Diagnostics.CodeAnalysis.NotNull] T? value,
        [NotNull] string parameterName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    public static string NotNullOrWhiteSpace(
        [System.Diagnostics.CodeAnalysis.NotNull] string? value, 
        [NotNull] string parameterName,
        int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
        }

        if (value!.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value!.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    public static int NotNegative(
        int value,
        [NotNull] string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    public static decimal NotNegative(
        decimal value,
        [NotNull] string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentException($"{parameterName} is less than zero");
        }
        return value;
    }

    public static TEumerable WithoutDuplicates<TEumerable>(TEumerable value, [NotNull] string parameterName) 
        where TEumerable : IEnumerable<object>
    {
        if (value.Count() != value.Distinct().Count())
        {
            throw new ArgumentException($"{parameterName} should not retain duplicates");
        }

        return value;
    }
}