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
    public class IntStack_GetMiddle_Tests : IntStack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeGetMiddleData))]
        public void Should_GetMiddle(double value, IntStack stack)
        {
            stack.GetMiddle().ShouldBe(value);
        }

        [Theory]
        [MemberData(nameof(MakeGetMiddleEmptyData))]
        public void Should_Return_Null_When_GetMiddle_From_Empty_Stack(IntStack stack)
        {
            stack.GetMiddle().ShouldBe(default);
        }

        public static IEnumerable<object[]> MakeGetMiddleData =>
            new List<object[]>
            {
                new object[] { 3d, GetTestIntStack(new[] {3}) },
                new object[] { 1.5d, GetTestIntStack(new[] {1, 2}) },
                new object[] { 2d, GetTestIntStack(new[] {3, 2, 1}) },
            };

        public static IEnumerable<object[]> MakeGetMiddleEmptyData =>
            new List<object[]>
            {
                new object[] { GetTestIntStack(Array.Empty<int>()) },
            };
    }
}
