extern alias Exercise5;

using Exercise5.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_5
{
    public class Queue_Init_Tests : Queue_BaseTests
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
