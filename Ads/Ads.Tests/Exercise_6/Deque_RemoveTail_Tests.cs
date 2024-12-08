﻿extern alias Exercise6;

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
    public class Deque_RemoveTail_Tests : Deque_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeRemoveFrontTail))]
        public void Should_RemoveTail(Deque<int> deque, int removeCount, int[] targetItems)
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