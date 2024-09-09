extern alias Exercise5;

using Exercise5.Ads.Exercise5;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_5.CircleQueue
{
    public class CircleQueue_Enqueue_Tests : CircleQueue_BaseTests
    {
        [Fact]
        public void Should_Enqueue_In_Empty_Queue()
        {
            var queue = GetEmptyCircleQueue<int>(5);

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            queue.Size().ShouldBe(3);
        }

        [Fact]
        public void Should_Throw_InvalidOperationException_When_Enqueue_To_Filled_Queue()
        {
            var queue = GetFilledCircleQueue<int>(5, new int[] {1, 2, 3, 4, 5});

            Assert.Throws<InvalidOperationException>(() => queue.Enqueue(1));

            queue.Size().ShouldBe(5);
            queue.Capacity.ShouldBe(5);
        }
    }
}
