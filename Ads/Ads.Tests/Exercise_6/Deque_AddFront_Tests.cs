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
    public class Deque_AddFront_Tests : Deque_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeAddFrontData))]
        public void Should_AddFront(Deque<int> deque, int[] addItems, int[] targetItems)
        {
            foreach (var item in addItems)
                deque.AddFront(item);

            deque.Size().ShouldBe(targetItems.Length);
            foreach (var item in targetItems)
                deque.RemoveTail().ShouldBe(item);
        }

        public static IEnumerable<object[]> MakeAddFrontData =>
            new List<object[]>
            {
                new object[] { GetEmptyDeque<int>(), new int[] {4, 5, 6}, new int[] {4, 5, 6}},
                new object[] { GetFilledDeque<int>(new int[] {3}), new int[] {4, 5, 6}, new int[] {3, 4, 5, 6}},
                new object[] { GetFilledDeque<int>(new int[] {1, 2, 3}), new int[] {4, 5, 6}, new int[] {1, 2, 3, 4, 5, 6}},
            };
    }
}
