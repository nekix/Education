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
    public class Stack_Push_Tests : HeadStack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakePushData))]
        public void Should_Push(int value, int size, HeadStack stack)
        {
            stack.Push(value);

            stack.Peek().ShouldBe(value);
            stack.Size().ShouldBe(size);
        }

        public static IEnumerable<object[]> MakePushData =>
            new List<object[]>
            {
                new object[] { 0, 1, GetTestHeadStack(Array.Empty<int>()) },
                new object[] { 2, 2, GetTestHeadStack(new[] {1}) },
                new object[] { 3, 3, GetTestHeadStack(new[] {1, 2}) },
                new object[] { 4, 4, GetTestHeadStack(new[] {1, 2, 3}) },
            };
    }
}
