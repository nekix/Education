extern alias Exercise7;

using Exercise7.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_7
{
    public class OrderedList_RemoveDuplicatesTests : OrderedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(RemoveDuplicatesData))]
        public void Should_RemoveDuplicates(OrderedList<int> list, int newCount, int[] finalValues)
        {
            list.RemoveDuplicates();

            list.Count().ShouldBe(newCount);

            var node = list.head;
            foreach (var item in finalValues)
            {
                node.value.ShouldBe(item);

                node.next?.prev.ShouldBe(node);
                node = node.next;
            }

            if (newCount != 0)
            {
                list.head.ShouldNotBeNull();
                list.head.prev.ShouldBe(null);
                list.tail.ShouldNotBeNull();
                list.tail.value.ShouldBe(finalValues[finalValues.Length - 1]);
                list.tail.next.ShouldBe(null);
            }
            else
            {
                list.head.ShouldBe(null);
                list.tail.ShouldBe(null);
            }
        }

        public static IEnumerable<object[]> RemoveDuplicatesData =>
            new List<object[]>
            {
                new object[] { GetFulledOrderedList(true, new int[0]), 0, new int[0]},
                new object[] { GetFulledOrderedList(true, new[] { 1, 1, 1, 1, 1 }), 0, new int[0] },

                new object[] { GetFulledOrderedList(true, new[] { 1, 1, 1, 2, 3, 4 }), 3, new[] { 2, 3, 4 } },
                new object[] { GetFulledOrderedList(true, new[] { 1, 2, 3, 4, 4, 4 }), 3, new[] { 1, 2, 3} },

                new object[] { GetFulledOrderedList(true, new[] { 1, 2, 3, 3, 3, 4 }), 3, new[] { 1, 2, 4} },

                new object[] { GetFulledOrderedList(true, new[] { 1, 1, 2, 3, 4, 4, 4, 5, 6, 7, 7, 7, 7 }), 4, new[] { 2, 3, 5, 6 } },
            };
    }
}
