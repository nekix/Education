using Shouldly;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

public class IsPalindrom_Tests
{
    [Theory]
    [InlineData("", true)]
    [InlineData("a", true)]
    [InlineData("aa", true)]
    [InlineData("aA", false)]
    [InlineData("aAa", true)]
    [InlineData("aAA", false)]
    [InlineData("aAAa", true)]
    [InlineData("AaaA", true)]
    [InlineData("AaAA", false)]
    [InlineData("Куска мыла", false)]
    [InlineData("Молот толоМ", true)]
    public void Should_IsPalindrom(string str, bool isPalindrom)
    {
        RecursionFuncs.IsPalindrom(str).ShouldBe(isPalindrom);
    }
}