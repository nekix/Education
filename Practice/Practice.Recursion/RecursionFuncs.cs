namespace Practice.Recursion;

public static class RecursionFuncs
{
    public static double Pow(double num, int power)
    {
        if (num == 0 && power < 0)
            return double.PositiveInfinity;

        return power switch
        {
            0 => 1d,
            1 => num,
            > 1 => num * Pow(num, power - 1),
            _ => 1 / num * Pow(num, power + 1)
        };
    }

    public static int SumOfDigits(int num)
    {
        if(num == 0) return 0;

        if (num < 0) num = -num;

        return num % 10 + SumOfDigits(num / 10);
    }
}
