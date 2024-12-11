using System;

namespace Practice.Recursion;

public static class RecursionFuncs
{
    public static double Pow(double num, int power, double startPowValue = 1d)
    {
        switch (power)
        {
            case < 0 when num == 0:
                return double.PositiveInfinity;
            case 0:
                return startPowValue;
            case > 0:
                startPowValue *= num;
                power -= 1;
                break;
            default:
                startPowValue /= num;
                power += 1;
                break;
        }

        return Pow(num, power, startPowValue);
    }

    public static int SumOfDigits(int num, int startSum = 0)
    {
        if (num < 0) num = -num;

        return num == 0 ? startSum : SumOfDigits(num / 10, startSum + num % 10);
    }

    public static int GetStackLength(Stack<int> stack, int startLength = 0)
    {
        if (!stack.TryPop(out var _))
            return startLength;

        return GetStackLength(stack, startLength + 1);
    }

    public static bool IsPalindrom(string str, int startCharOffset = 0)
    {
        int length = str.Length;

        if (startCharOffset == str.Length / 2)
            return true;

        if (str[startCharOffset] != str[length - startCharOffset - 1])
            return false;

        return IsPalindrom(str, startCharOffset + 1);
    }

    public static void PrintEvenNumberValues(List<int> numbers, int startIndex = 0)
    {
        if (startIndex >= numbers.Count)
            return;

        if (numbers[startIndex] % 2 == 0)
            Console.WriteLine(numbers[startIndex]);

        PrintEvenNumberValues(numbers, startIndex + 1);
    }

    public static void PrintValuesWithEvenNumberIndex(List<int> numbers, int startIndex = 0)
    {
        if (startIndex >= numbers.Count)
            return;

        if (startIndex % 2 == 0)
            Console.WriteLine(numbers[startIndex]);

        PrintValuesWithEvenNumberIndex(numbers, startIndex + 1);
    }
}
