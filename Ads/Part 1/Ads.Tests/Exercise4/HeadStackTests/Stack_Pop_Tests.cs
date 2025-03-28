extern alias Exercise4;
using Exercise4.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using HeadStack = Exercise4.AlgorithmsDataStructures.HeadStack<int>;

namespace Ads.Tests.Exercise4.HeadStackTests
{
    public class Stack_Pop_Tests : HeadStack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakePopData))]
        public void Should_Pop(int value, int size, HeadStack stack)
        {
            stack.Pop().ShouldBe(value);
            stack.Size().ShouldBe(size);
        }

        [Theory]
        [MemberData(nameof(MakePopEmptyData))]
        public void Should_Return_Null_When_Pop_From_Empty_Stack(HeadStack stack)
        {
            stack.Pop().ShouldBe(default);
            stack.Size().ShouldBe(0);
        }

        public static IEnumerable<object[]> MakePopData =>
            new List<object[]>
            {
                new object[] { 3, 0, GetTestHeadStack(new[] {3}) },
                new object[] { 2, 1, GetTestHeadStack(new[] {1, 2}) },
                new object[] { 1, 2, GetTestHeadStack(new[] {3, 2, 1}) },
            };

        public static IEnumerable<object[]> MakePopEmptyData =>
            new List<object[]>
            {
                new object[] { GetTestHeadStack(Array.Empty<int>()) },
            };
    }
}
