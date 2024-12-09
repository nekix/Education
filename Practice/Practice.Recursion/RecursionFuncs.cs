using System;

namespace Practice.Recursion;

public static class RecursionFuncs
{
    public static double Pow(double num, int power)
    {
        if (num == 0 && power < 0)
            return double.PositiveInfinity;

        if (power == 0)
            return 1d;

        if (power == 1)
            return num;

        return Pow(num, power - 1, num);
    }

    private static double Pow(double num, int power, double acc)
    {
        if (power == 0)
            return acc;

        if (power > 0)
        {
            acc *= num;
            power -= 1;
        }
        else
        {
            acc /= num;
            power += 1;
        }

        return Pow(num, power, acc);
    }

    public static int SumOfDigits(int num)
    {
        if (num < 0) num = -num;

        return SumOfDigits(num / 10, num % 10);
    }

    private static int SumOfDigits(int num, int sum)
    {
        return num == 0 ? sum : SumOfDigits(num / 10, sum + num % 10);
    }

    public static int GetStackLength(Stack<int> stack)
    {
        return GetStackLength(stack, 0);
    }

    private static int GetStackLength(Stack<int> stack, int length)
    {
        if (!stack.TryPop(out var _))
            return length;

        return GetStackLength(stack, length + 1);
    }

    public static bool IsPalindrom(string str)
    {
        int length = str.Length;

        if (string.IsNullOrEmpty(str) || str.Length == 1) return true;

        if (str[0] != str[length - 1])
            return false;

        return IsPalindrom(str.Substring(1, length - 2));
    }
}
