extern alias Exercise5;

using Exercise5.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Queue = Exercise5.AlgorithmsDataStructures.Queue<int>;

namespace Ads.Tests.Exercise_5
{
    public class Queue_Rotate_Tests : Queue_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeDequeData))]
        public void Should_Rotate_Queue(int rotateCount, int[] targetItems, Queue queue)
        {
            queue = queue.Rotate(rotateCount);

            foreach (var item in targetItems)
                queue.Dequeue().ShouldBe(item);

            queue.Size().ShouldBe(0);
            queue.Dequeue().ShouldBe(default);
        }

        public static IEnumerable<object[]> MakeDequeData =>
            new List<object[]>
            {
                new object[] { 1, new int[] { 1 }, GetFilledQueue<int>(new int[] { 1 }) },
                new object[] { 1, new int[] { 2, 3, 1 }, GetFilledQueue<int>(new int[] { 1, 2, 3 }) },
                new object[] { 3, new int[] { 3, 2, 1 }, GetFilledQueue<int>(new int[] { 3, 2, 1 }) },
                new object[] { 5, new int[] { 3, 1, 2 }, GetFilledQueue(new int[] { 1, 2, 3 }) },
            };
    }
}
