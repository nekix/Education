extern alias Exercise4;

using Exercise4.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Stack = Exercise4.AlgorithmsDataStructures.Stack<int>;

namespace Ads.Tests.Exercise4
{
    public class Stack_Peek_Tests : Stack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakePeekData))]
        public void Should_Peek(int value, Stack stack)
        {
            stack.Peek().ShouldBe(value);
        }

        [Theory]
        [MemberData(nameof(MakePeekEmptyData))]
        public void Should_Throw_InvalidOperationException_When_Peek_From_Empty_Stack(Stack stack)
        {
            stack.Peek().ShouldBe(default);
        }

        public static IEnumerable<object[]> MakePeekData =>
            new List<object[]>
            {
                new object[] { 3, GetTestStack(new[] {3}) },
                new object[] { 2, GetTestStack(new[] {1, 2}) },
                new object[] { 1, GetTestStack(new[] {3, 2, 1}) },
            };

        public static IEnumerable<object[]> MakePeekEmptyData =>
            new List<object[]>
            {
                new object[] { GetTestStack(Array.Empty<int>()) },
            };
    }
}
