using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

public class GetStackLength_Tests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1, 1)]
    [InlineData(2, 2, 2)]
    [InlineData(3, 3, 4, 5)]
    public void Should_Get_Length(int length, params int[] list)
    {
        var stack = new Stack<int>(list);

        RecursionFuncs.GetStackLength(stack).ShouldBe(length);
    }
}