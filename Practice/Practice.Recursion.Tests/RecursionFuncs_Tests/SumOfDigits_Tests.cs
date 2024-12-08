using Shouldly;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

public class SumOfDigits_Tests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(33, 6)]
    [InlineData(225, 9)]
    [InlineData(-1, 1)]
    [InlineData(-3, 3)]
    [InlineData(-33, 6)]
    public void Should_Return_Digit_Numbers(int num, int result)
    {
        RecursionFuncs.SumOfDigits(num).ShouldBe(result);
    }
}