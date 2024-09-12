extern alias Exercise6;

using Ads.Tests.Exercise_6.ComparableDequeTest;
using Exercise6.Ads.Exercise6;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_6
{
    public class ComparableDeque_RemoveFront_Tests : ComparableDeque_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeRemoveFrontData))]
        public void Should_RemoveFront(ComparableDeque<int> deque, int removeCount, int[] targetItems, int min)
        {
            for (int i = 0; i < removeCount; i++)
                deque.RemoveFront();

            deque.Size().ShouldBe(targetItems.Length);
            deque.GetMin().ShouldBe(min);

            foreach (var item in targetItems)
                deque.RemoveTail().ShouldBe(item);
        }

        public static IEnumerable<object[]> MakeRemoveFrontData =>
            new List<object[]>
            {
                new object[] { GetEmptyDeque<int>(), 1, Array.Empty<int>(), 0},
                new object[] { GetFilledDeque<int>(new int[] {3}), 1, Array.Empty<int>(), 0},
                new object[] { GetFilledDeque<int>(new int[] {1, 2, 3}), 2, new int[] {1}, 1},
                new object[] { GetFilledDeque<int>(new int[] {3, 2, 1}), 2, new int[] {3}, 3},
                new object[] { GetFilledDeque<int>(new int[] {3, 2, 1, 5, 6}), 3, new int[] {3, 2}, 2},
            };
    }
}
