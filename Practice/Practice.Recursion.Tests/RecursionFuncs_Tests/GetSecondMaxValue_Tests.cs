using Shouldly;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

public class GetSecondMaxValue_Tests
{
    [Theory]
    [InlineData(new int[] { 5, 4, 3, 2, 5 }, 5)]
    [InlineData(new int[] { 2, 3, 5, 4 }, 4)]
    [InlineData(new int[] { 7, 7, 4, 1, 3, 6, 7, 8, 8, 3, 9},  8)]
    public void Should_Return(int[] numbers, int result)
    {
        RecursionFuncs.GetSecondMaxValue(numbers.ToList()).ShouldBe(result);
    }
}