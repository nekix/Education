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
using Stack = Exercise4.AlgorithmsDataStructures.Stack<int>;

namespace Ads.Tests.Exercise4
{
    public class Stack_Size_Tests : Stack_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeSizeData))]
        public void Should_Size(int size, Stack stack)
        {
            stack.Size().ShouldBe(size);
        }

        public static IEnumerable<object[]> MakeSizeData =>
            new List<object[]>
            {
                new object[] { 0, GetTestStack(Array.Empty<int>()) },
                new object[] { 1, GetTestStack(new[] {1}) },
                new object[] { 2, GetTestStack(new[] {1, 2}) },
                new object[] { 3, GetTestStack(new[] {1, 2, 3}) },
            };
    }
}
