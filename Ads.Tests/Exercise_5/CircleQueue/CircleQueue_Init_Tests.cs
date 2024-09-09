extern alias Exercise5;

using Exercise5.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_5.CircleQueue
{
    public class CircleQueue_Init_Tests : CircleQueue_BaseTests
    {
        [Fact]
        public void Should_Init()
        {
            var queue = GetEmptyCircleQueue<int>(5);

            queue.Size().ShouldBe(0);
            queue.Capacity.ShouldBe(5);
            queue.Dequeue().ShouldBe(default);
        }
    }
}
