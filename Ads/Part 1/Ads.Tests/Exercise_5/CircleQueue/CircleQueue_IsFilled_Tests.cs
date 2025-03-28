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
    public class CircleQueue_IsFilled_Tests : CircleQueue_BaseTests
    {
        [Fact]
        public void Should_Return_True_When_Filled()
        {
            var queue = GetFilledCircleQueue<int>(3, new int[] {1, 2, 3});

            queue.IsFilled().ShouldBeTrue();
        }

        [Fact]
        public void Should_Return_False_When_Not_Filled()
        {
            var queue = GetFilledCircleQueue<int>(3, new int[] { 1, 2 });

            queue.IsFilled().ShouldBeFalse();
        }
    }
}
