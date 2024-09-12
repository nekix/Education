extern alias Exercise6;

using Exercise6.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_6
{
    public class Deque_AddTail_Tests : Deque_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeAddTailData))]
        public void Should_AddTail(Deque<int> deque, int[] addItems, int[] targetItems)
        {
            foreach (var item in addItems)
                deque.AddTail(item);

            deque.Size().ShouldBe(targetItems.Length);
            foreach (var item in targetItems)
                deque.RemoveTail().ShouldBe(item);
        }

        public static IEnumerable<object[]> MakeAddTailData =>
            new List<object[]>
            {
                new object[] { GetEmptyDeque<int>(), new int[] {4, 5, 6}, new int[] {6, 5, 4}},
                new object[] { GetFilledDeque<int>(new int[] {3}), new int[] {4, 5, 6}, new int[] {6, 5, 4, 3}},
                new object[] { GetFilledDeque<int>(new int[] {1, 2, 3}), new int[] {4, 5, 6}, new int[] {6, 5, 4, 1, 2, 3}},
            };
    }
}
