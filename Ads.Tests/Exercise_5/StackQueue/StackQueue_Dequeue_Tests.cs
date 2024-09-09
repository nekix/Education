﻿extern alias Exercise5;

using Exercise5.Ads.Exercise5;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_5.StackQueue
{
    public class StackQueue_Dequeue_Tests : StackQueue_BaseTests
    {
        [Fact]
        public void Should_Return_Default_When_Dequeue_Empty_Queue()
        {
            var queue = GetEmptyQueue<int>();

            var item = queue.Dequeue();

            item.ShouldBe(default);
        }

        [Theory]
        [MemberData(nameof(MakeDequeData))]
        public void Should_Deque_Filled_Queue(int[] items, StackQueue<int> queue)
        {
            foreach (var item in items)
                queue.Dequeue().ShouldBe(item);

            queue.Size().ShouldBe(0);
            queue.Dequeue().ShouldBe(default);
        }

        public static IEnumerable<object[]> MakeDequeData =>
            new List<object[]>
            {
                new object[] { new int[] { 1, 2, 3 }, GetFilledQueue<int>(new int[] { 1, 2, 3 }) },
                new object[] { new int[] { 4 }, GetFilledQueue<int>(new int[] { 4 }) },
                new object[] { new int[] { 1, 1, 1, 2, 1, 1 }, GetFilledQueue<int>(new int[] { 1, 1, 1, 2, 1, 1 }) },
            };
    }
}
