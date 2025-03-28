extern alias Exercise5;

using Ads.Tests.Exercise_5.StackQueue;
using Exercise5.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_5.StackQueue
{
    public class StackQueue_Enqueue_Tests : StackQueue_BaseTests
    {
        [Fact]
        public void Should_Enqueue_In_Empty_Queue()
        {
            var queue = GetEmptyQueue<int>();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            queue.Size().ShouldBe(3);
        }
    }
}
