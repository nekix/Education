using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Practice.Recursion;

public static class RecursionFuncs
{
    // Задание 1.
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

    // Задание 2.
    public static int SumOfDigits(int num, int startSum = 0)
    {
        if (num < 0) num = -num;

        return num == 0 ? startSum : SumOfDigits(num / 10, startSum + num % 10);
    }

    // Задание 2. Рефлексия. Сделал рекурсивный вызов последним в функции.
    // Вообще, лучше стараться, чтобы рекурсивный вызов был последним
    // единственным в функции и без дополнительных вычислений, тогда
    // компилятор хорошо её оптимизирует (не во всех языках, к сожалению),
    // можно вообще без стека вызовов обойтись (т.н. элиминация хвостовой рекурсии). 

    // Задание 3.
    public static int GetStackLength(Stack<int> stack, int startLength = 0)
    {
        if (!stack.TryPop(out var _))
            return startLength;

        return GetStackLength(stack, startLength + 1);
    }

    // Задание 3. Рефлексия.
    // Сделал одной функцией, но использовал
    // дефолтные параметры, которые лучше вообще не использовать
    // (можно сделать одной функцией, просто без
    // хвостовой рекурсии).


    // Задание 4.
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

    // Задание 4. Рефлексия.
    // Изначально сделал с вырезанием подстроки,
    // что приводило к постоянному созданию её копий.

    // Задание 5.
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

    // Задание 6.
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

    // Задание 4,5,6. Рефлексия.
    // Изначально сделал через одну функцию с лишними параметреми,
    // переделал на несколько. Также в ней были дефолтные параметры,
    // которые лучше вообще не использовать.

    // Задание 7.
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

    // Задание 7. Рефлексия.
    // Лучше исходно проверять сколько элементов в списке,
    // и сразу прекращать если меньше двух.А дальше
    // использовать первый и второй элементы начальными
    // параметрами с учётом их отношений.
    // Внес проверку на наличие двух элементов в списке и
    // передачу первого и второго в качестве параметров.

    // Задание 8.
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

    // Задание 8. Рефлексия.
    // Изначально сделал двумя функциями, позже переделал
    // через одну рекурсивную и лишился элиминации.

    // Доп. задание повышенной сложности
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
