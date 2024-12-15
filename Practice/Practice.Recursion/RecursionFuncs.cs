using System;
using System.Reflection;
using System.Runtime.InteropServices;

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

    public static bool IsPalindrom(string str)
    {
        return IsPalindrom(str, 0);
    }

    private static bool IsPalindrom(string str, int charOffset)
    {
        int length = str.Length;

        if (charOffset == str.Length / 2)
            return true;

        if (str[charOffset] != str[length - charOffset - 1])
            return false;

        return IsPalindrom(str, charOffset + 1);
    }

    public static void PrintEvenNumberValues(List<int> numbers)
    {
        PrintEvenNumberValues(numbers, 0);
    }

    private static void PrintEvenNumberValues(List<int> numbers, int index)
    {
        if (index >= numbers.Count)
            return;

        if (numbers[index] % 2 == 0)
            Console.WriteLine(numbers[index]);

        PrintEvenNumberValues(numbers, index + 1);
    }

    public static void PrintValuesWithEvenNumberIndex(List<int> numbers)
    {
        PrintValuesWithEvenNumberIndex(numbers, 0);
    }

    private static void PrintValuesWithEvenNumberIndex(List<int> numbers, int index)
    {
        if (index >= numbers.Count)
            return;

        Console.WriteLine(numbers[index]);

        PrintValuesWithEvenNumberIndex(numbers, index + 2);
    }

    public static int? GetSecondMaxValue(List<int> numbers)
    {
        if (numbers.Count < 2)
            return null;

        int max;
        int secondMax;

        if (numbers[0] < numbers[1])
        {
            max = numbers[1];
            secondMax = numbers[0];
        }
        else
        {
            max = numbers[0];
            secondMax = numbers[1];
        }

        return GetSecondMaxValue(numbers, 2, max, secondMax);
    }

    private static int? GetSecondMaxValue(List<int> numbers, int index, int max, int secondMax)
    {
        if (numbers.Count <= index)
            return secondMax;

        if (max < numbers[index])
        {
            secondMax = max;
            max = numbers[index];
        }
        else if (secondMax < numbers[index])
        {
            secondMax = numbers[index];
        }

        return GetSecondMaxValue(numbers, index + 1, max, secondMax);
    }

    public static List<string> GetFilesPathsFromDirectory(string directoryPath)
    {
        List<string> files = Directory.GetFiles(directoryPath).ToList();

        string[] innerDirs = Directory.GetDirectories(directoryPath);

        foreach (string innerDir in innerDirs)
        {
            List<string> innerFiles = GetFilesPathsFromDirectory(innerDir);

            files.AddRange(innerFiles);
        }

        return files;
    }

    public static List<string> GetAllBalancedParentheses(int openParenthesesCount)
    {
        List<string> parentheses = new List<string>();

        GetAllBalancedParentheses(openParenthesesCount, openParenthesesCount, string.Empty, parentheses);

        return parentheses;
    }
    
    private static void GetAllBalancedParentheses(int openParenthesesCount, int closeParentheses, string currentParenthese, List<string> parentheses)
    {
        if (openParenthesesCount > closeParentheses) return;

        if (openParenthesesCount < 0 || closeParentheses < 0)
            return;

        if (openParenthesesCount == 0 && closeParentheses == 0)
        {
            parentheses.Add(currentParenthese);
            return;
        }

        GetAllBalancedParentheses(openParenthesesCount - 1, closeParentheses, currentParenthese + "(", parentheses);

        GetAllBalancedParentheses(openParenthesesCount, closeParentheses - 1, currentParenthese + ")", parentheses);
    }
}
