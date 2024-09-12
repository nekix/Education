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
    public class ComparableDeque_RemoveTail_Tests : ComparableDeque_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeRemoveFrontTail))]
        public void Should_RemoveTail(ComparableDeque<int> deque, int removeCount, int[] targetItems, int min)
        {
            for (int i = 0; i < removeCount; i++)
                deque.RemoveTail();

            deque.Size().ShouldBe(targetItems.Length);
            deque.GetMin().ShouldBe(min);

            foreach (var item in targetItems)
                deque.RemoveTail().ShouldBe(item);
        }

        public static IEnumerable<object[]> MakeRemoveFrontTail =>
            new List<object[]>
            {
                new object[] { GetEmptyDeque<int>(), 1, Array.Empty<int>(), 0},
                new object[] { GetFilledDeque<int>(new int[] {3}), 1, Array.Empty<int>(), 0},
                new object[] { GetFilledDeque<int>(new int[] {1, 2, 3}), 2, new int[] {3}, 3},
            };
    }
}
