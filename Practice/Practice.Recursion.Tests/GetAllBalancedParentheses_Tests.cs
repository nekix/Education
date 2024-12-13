using System.Runtime.InteropServices;
using Shouldly;

namespace Practice.Recursion.Tests;

public class GetAllBalancedParentheses_Tests
{
    [Theory]
    [InlineData(1, new string[] {"()"})]
    [InlineData(2, new string[] { "(())", "()()" })]
    [InlineData(3, new string[] { "((()))", "(()())", "(())()", "()(())", "()()()" })]
    [InlineData(4, new string[] { "(((())))", "((()()))", "((())())", "((()))()", "(()(()))", "(()()())", "(()())()", "(())(())", "(())()()", "()((()))", "()(()())", "()(())()", "()()(())", "()()()()" })]
    public void Should_Return(int numParentheses, string[] result)
    {
        var res = RecursionFuncs.GetAllBalancedParentheses(numParentheses);
            
        res.ShouldBe(result);
    }
}