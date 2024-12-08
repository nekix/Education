extern alias Exercise5;

using Exercise5.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_5
{
    public class Queue_Enqueue_Tests : Queue_BaseTests
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
