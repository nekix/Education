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
using HeadStack = Exercise4.AlgorithmsDataStructures.HeadStack<int>;

namespace Ads.Tests.Exercise4.HeadStackTests
{
    public class HeadStack_Peek_Tests : HeadStack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakePeekData))]
        public void Should_Peek(int value, HeadStack stack)
        {
            stack.Peek().ShouldBe(value);
        }

        [Theory]
        [MemberData(nameof(MakePeekEmptyData))]
        public void Should_Return_Null_When_Peek_From_Empty_Stack(HeadStack stack)
        {
            stack.Peek().ShouldBe(default);
        }

        public static IEnumerable<object[]> MakePeekData =>
            new List<object[]>
            {
                new object[] { 3, GetTestHeadStack(new[] {3}) },
                new object[] { 2, GetTestHeadStack(new[] {1, 2}) },
                new object[] { 1, GetTestHeadStack(new[] {3, 2, 1}) },
            };

        public static IEnumerable<object[]> MakePeekEmptyData =>
            new List<object[]>
            {
                new object[] { GetTestHeadStack(Array.Empty<int>()) },
            };
    }
}
