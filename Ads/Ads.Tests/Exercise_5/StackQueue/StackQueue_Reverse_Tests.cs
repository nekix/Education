extern alias Exercise5;

using Exercise5.Ads.Exercise5;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ads.Tests.Exercise_5.StackQueue
{
    public class StackQueue_Reverse_Tests : StackQueue_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeReverseData))]
        public void Should_Reverse_Queue(int[] targetItems, StackQueue<int> queue)
        {
            queue.Reverse();

            foreach (var item in targetItems)
                queue.Dequeue().ShouldBe(item);

            queue.Size().ShouldBe(0);
            queue.Dequeue().ShouldBe(default);
        }

        public static IEnumerable<object[]> MakeReverseData =>
            new List<object[]>
            {
                new object[] { Array.Empty<int>(), GetEmptyQueue<int>() },
                new object[] { new int[] { 1 }, GetFilledQueue<int>(new int[] { 1 }) },
                new object[] { new int[] { 1, 2 }, GetFilledQueue<int>(new int[] { 2, 1 }) },
                new object[] { new int[] { 3, 2, 1 }, GetFilledQueue<int>(new int[] { 1, 2, 3 }) },
            };
    }
}
