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
    public class OrderedList_DeleteTests : OrderedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(RemoveData))]
        public void Should_Delete(OrderedList<int> list, int value, int newCount, int[] finalValues)
        {
            var node = list.Find(value);
           
            list.Delete(value);

            list.Count().ShouldBe(newCount);
            if (node != null)
            {
                list.Find(value).ShouldNotBe(node);
            }

            node = list.head;
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

        public static IEnumerable<object[]> RemoveData =>
            new List<object[]>
            {
                new object[] { GetFulledOrderedList(true, new int[0]), 5, 0, new int[0]},
                new object[] { GetFulledOrderedList(true, new[] { 5, 1, 9, 6, 5 }), 8, 5, new[] { 1, 5, 5, 6, 9 } },

                new object[] { GetFulledOrderedList(true, new[] { 5, 1, 9, 6, 7 }), 1, 4, new[] { 5, 6, 7, 9 } },
                new object[] { GetFulledOrderedList(false, new[] { 5, 1, 9, 6, 7 }), 1, 4, new[] { 9, 7, 6, 5 } },

                new object[] { GetFulledOrderedList(true, new[] { 5, 1, 9, 6, 7 }), 6, 4, new[] { 1, 5, 7, 9 } },
                new object[] { GetFulledOrderedList(false, new[] { 5, 1, 9, 6, 7 }), 6, 4, new[] { 9, 7, 5, 1 } },

                new object[] { GetFulledOrderedList(true, new[] { 5, 1, 9, 6, 7 }), 9, 4, new[] { 1, 5, 6, 7 } },
                new object[] { GetFulledOrderedList(false, new[] { 5, 1, 9, 6, 7 }), 9, 4, new[] { 7, 6, 5, 1 } },
            };
    }
}
