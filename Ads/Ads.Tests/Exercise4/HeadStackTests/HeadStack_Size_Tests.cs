extern alias Exercise4;
using AlgorithmsDataStructures;
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
    public class HeadStack_Size_Tests : HeadStack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeSizeData))]
        public void Should_Size(int size, HeadStack stack)
        {
            stack.Size().ShouldBe(size);
        }

        public static IEnumerable<object[]> MakeSizeData =>
            new List<object[]>
            {
                new object[] { 0, GetTestHeadStack(Array.Empty<int>()) },
                new object[] { 1, GetTestHeadStack(new[] {1}) },
                new object[] { 2, GetTestHeadStack(new[] {1, 2}) },
                new object[] { 3, GetTestHeadStack(new[] {1, 2, 3}) },
            };
    }
}
