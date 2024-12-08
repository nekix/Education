using Shouldly;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

public class Pow_Tests
{
    [Theory]
    [InlineData(-12d, 1, -12d)]
    [InlineData(-5d, 3, -125d)]
    [InlineData(-1d, 3, -1d)]
    [InlineData(-3d, 4, 81d)]
    [InlineData(3d, 4, 81d)]
    [InlineData(0d, 0, 1d)]
    [InlineData(0d, -1, double.PositiveInfinity)]
    [InlineData(0d, -2, double.PositiveInfinity)]
    [InlineData(12d, 0, 1)]
    [InlineData(-12d, 0, 1)]
    [InlineData(3.5d, 2, 12.25d)]
    [InlineData(3.5d, -2, 0.08163265306122448d)]
    public void Should_Return_Num_In_Power(double num, int power, double result)
    {
        RecursionFuncs.Pow(num, power).ShouldBe(result);
    }

}