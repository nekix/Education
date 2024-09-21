extern alias Exercise7;
using Exercise7.AlgorithmsDataStructures;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Ads.Tests.Exercise_7
{
    public class OrderedList_FindIndexTests : OrderedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(FindIndexData))]
        public void Should_FindIndex(OrderedList<int> list, int index)
        {
            var node = list.head;
            for(int i = 0; i < index; i++)
                node = node.next;

            list.FindIndex(node).ShouldBe(index);
        }

        [Fact]
        public void Should_Return_When_Not_Exist()
        {
            var list = GetFulledOrderedList(true, new[] { 0, 1, 2, 3, 4, 5 });

            var node = new Node<int>(10);

            list.FindIndex(node).ShouldBe(-1);
        }

        public static IEnumerable<object[]> FindIndexData =>
            new List<object[]>
            {
                new object[] { GetFulledOrderedList(true, new[] { 1 }), 0 },

                new object[] { GetFulledOrderedList(false, new[] { 0, 1, 2, 3, 4, 5 }), 5 },

                new object[] { GetFulledOrderedList(true, new[] { 1, 1, 1, 1, 1 }), 1 },

                new object[] { GetFulledOrderedList(true, new[] { 1, 1, 1, 2, 3, 4 }), 3 },
                new object[] { GetFulledOrderedList(false, new[] { 1, 2, 3, 4, 4, 4 }), 5 },

                new object[] { GetFulledOrderedList(true, new[] { 1, 2, 3, 3, 3, 4 }), 2 },

                new object[] { GetFulledOrderedList(true, new[] { 1, 1, 2, 3, 4, 4, 4, 5, 6, 7, 7, 7, 7 }), 7 },
            };
    }
}
