extern alias Exercise6;

using Ads.Tests.Exercise_6.ComparableDequeTest;
using Exercise6.Ads.Exercise6;
using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_6.ImprovedDeque_Tests
{
    public class ImprovedDeque_RemoveTail_Tests : ImprovedDeque_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeRemoveFrontTail))]
        public void Should_RemoveTail(ImprovedDeque<int> deque, int removeCount, int[] targetItems)
        {
            for (int i = 0; i < removeCount; i++)
                deque.RemoveTail();

            deque.Size().ShouldBe(targetItems.Length);

            foreach (var item in targetItems)
                deque.RemoveTail().ShouldBe(item);
        }

        public static IEnumerable<object[]> MakeRemoveFrontTail =>
            new List<object[]>
            {
                new object[] { GetEmptyDeque<int>(), 1, Array.Empty<int>()},
                new object[] { GetFilledDeque<int>(new int[] {3}), 1, Array.Empty<int>()},
                new object[] { GetFilledDeque<int>(new int[] {1, 2, 3}), 2, new int[] {3}},
            };
    }
}
