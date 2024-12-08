extern alias Exercise5;

using Ads.Tests.Exercise_5.StackQueue;
using Exercise5.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_5.StackQueue
{
    public class StackQueue_Init_Tests : StackQueue_BaseTests
    {
        [Fact]
        public void Should_Init()
        {
            var queue = GetEmptyQueue<int>();

            queue.Size().ShouldBe(0);
            queue.Dequeue().ShouldBe(default);
        }
    }
}
