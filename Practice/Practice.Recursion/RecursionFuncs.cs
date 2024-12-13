using System;
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

    public static bool IsPalindrom(string str, int startCharOffset)
    {
        int length = str.Length;

        if (startCharOffset == str.Length / 2)
            return true;

        if (str[startCharOffset] != str[length - startCharOffset - 1])
            return false;

        return IsPalindrom(str, startCharOffset + 1);
    }

    public static void PrintEvenNumberValues(List<int> numbers, int startIndex)
    {
        if (startIndex >= numbers.Count)
            return;

        if (numbers[startIndex] % 2 == 0)
            Console.WriteLine(numbers[startIndex]);

        PrintEvenNumberValues(numbers, startIndex + 1);
    }

    public static void PrintValuesWithEvenNumberIndex(List<int> numbers, int startIndex)
    {
        if (startIndex >= numbers.Count)
            return;

        if (startIndex % 2 == 0)
            Console.WriteLine(numbers[startIndex]);

        PrintValuesWithEvenNumberIndex(numbers, startIndex + 1);
    }

    public static int? GetSecondMaxValue(List<int> numbers)
    {
        if (numbers.Count == 0)
            return null;

        int max = numbers[0];
        int secondMax = numbers[0];

        return GetSecondMaxValue(numbers, 1, max, secondMax);
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
        List<string> filesPaths = Directory.GetFiles(directoryPath).ToList();

        string[] dirs = Directory.GetDirectories(directoryPath);

        Queue<string> processingDirs = new Queue<string>(dirs);

        return GetFilesPathsFromDirectory(processingDirs, filesPaths);
    }

    private static List<string> GetFilesPathsFromDirectory(Queue<string> processingDirs, List<string> filesPaths)
    {
        int processingDirsCount = processingDirs.Count;

        if (processingDirsCount == 0)
            return filesPaths;

        for (int i = 0; i < processingDirsCount; i++)
        {
            string directory = processingDirs.Dequeue();

            string[] files = Directory.GetFiles(directory);
            filesPaths.AddRange(files);

            foreach (string subDir in Directory.GetDirectories(directory))
                processingDirs.Enqueue(subDir);
        }

        return GetFilesPathsFromDirectory(processingDirs, filesPaths);
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
