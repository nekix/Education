extern alias Exercise4;

using Exercise4.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise4.IntStackTests
{
    public class IntStack_PeekMin_Tests : IntStack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakePeekData))]
        public void Should_PeelMin(int value, IntStack stack)
        {
            stack.PeekMin().ShouldBe(value);
        }

        [Theory]
        [MemberData(nameof(MakePeekEmptyData))]
        public void Should_Return_Null_When_PeelMin_From_Empty_Stack(IntStack stack)
        {
            stack.Peek().ShouldBe(default);
        }

        public static IEnumerable<object[]> MakePeekData =>
            new List<object[]>
            {
                new object[] { 3, GetTestIntStack(new[] {3}) },
                new object[] { 1, GetTestIntStack(new[] {1, 2}) },
                new object[] { 1, GetTestIntStack(new[] {3, 2, 1}) },
            };

        public static IEnumerable<object[]> MakePeekEmptyData =>
            new List<object[]>
            {
                new object[] { GetTestIntStack(Array.Empty<int>()) },
            };
    }
}
