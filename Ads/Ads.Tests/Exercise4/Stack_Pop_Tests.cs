extern alias Exercise4;

using Exercise4.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Stack = Exercise4.AlgorithmsDataStructures.Stack<int>;

namespace Ads.Tests.Exercise4
{
    public class Stack_Pop_Tests : Stack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakePopData))]
        public void Should_Pop(int value, int size, Stack stack)
        {
            stack.Pop().ShouldBe(value);
            stack.Size().ShouldBe(size);
        }

        [Theory]
        [MemberData(nameof(MakePopEmptyData))]
        public void Should_Return_Null_When_Pop_From_Empty_Stack(Stack stack)
        {
            stack.Pop().ShouldBe(default);
            stack.Size().ShouldBe(0);
        }

        public static IEnumerable<object[]> MakePopData =>
            new List<object[]>
            {
                new object[] { 3, 0, GetTestStack(new[] {3}) },
                new object[] { 2, 1, GetTestStack(new[] {1, 2}) },
                new object[] { 1, 2, GetTestStack(new[] {3, 2, 1}) },
            };

        public static IEnumerable<object[]> MakePopEmptyData =>
            new List<object[]>
            {
                new object[] { GetTestStack(Array.Empty<int>()) },
            };
    }
}
