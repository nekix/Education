extern alias Exercise7;

using Exercise7.AlgorithmsDataStructures;
using Exercise7.Ads.Exercise7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_7.OrderedListExtensionsTests
{
    public class OrderedListExtensions_MergeTests : OrderedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(UnionData))]
        public void Should_Merge(OrderedList<int> first, OrderedList<int> second, int count, bool asc, params int[] finalValues)
        {
            var list = first.Merge(second,asc);

            list.Count().ShouldBe(count);

            var node = list.head;
            foreach (var value in finalValues)
            {
                node.ShouldNotBeNull();
                node.value.ShouldBe(value);
                node = node.next;
            }

            node.ShouldBeNull();
        }

        [Theory]
        [MemberData(nameof(ContainsData))]
        public void Should_Contains(OrderedList<int> first, OrderedList<int> second, bool isContains)
        {
            first.Contains(second).ShouldBe(isContains);
        }

        [Theory]
        [MemberData(nameof(GetMaxCommonValueData))]
        public void Should_GetMaxCommonValue(OrderedList<int> list, int maxCommonValue)
        {
            list.GetMaxCommonValue().ShouldBe(maxCommonValue);
        }

        public static IEnumerable<object[]> GetMaxCommonValueData() =>
            new List<object[]>
            {
                new object[] { GetFulledOrderedList(true, Array.Empty<int>()), default(int) },
                new object[] { GetFulledOrderedList(true, new int[] { 1, 5, 6, 2, 3 }), 1 },
                new object[] { GetFulledOrderedList(true, new int[] { 1, 5, 6, 2, 3, 6, 6, 6 }), 6 },
                new object[] { GetFulledOrderedList(true, new int[] { 1, 1, 2, 3, 4, 4, 5, 5, 5, 5, 6, 6, 7 }), 5 },
            };

        public static IEnumerable<object[]> ContainsData =>
            new List<object[]>
            {
                new object[]
                {
                    GetFulledOrderedList(true, new int[0]),
                    GetFulledOrderedList(false, new int[0]),
                    false
                },
                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 5, 6, 2, 3}),
                    GetFulledOrderedList(false, new int[0]),
                    false
                },
                new object[]
                {
                    GetFulledOrderedList(true, new int[0]),
                    GetFulledOrderedList(true, new int[] {1, 5, 6, 2, 3}),
                    false
                },

                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 1, 2, 3, 5, 6, 3}),
                    GetFulledOrderedList(true, new int[] {1, 2, 3}),
                    true
                },
                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 1, 2, 3, 5, 6, 2, 3}),
                    GetFulledOrderedList(true, new int[] {1, 2, 3, 4}),
                    false
                },
                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 1, 2, 3, 5, 6, 2, 3}),
                    GetFulledOrderedList(true, new int[] {3, 3, 5, 6}),
                    true
                },
            };

        public static IEnumerable<object[]> UnionData =>
            new List<object[]>
            {
                new object[]
                {
                    GetFulledOrderedList(true, new int[0]),
                    GetFulledOrderedList(false, new int[0]),
                    0,
                    true,
                    new int[0]
                },

                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 1, 2, 3}),
                    GetFulledOrderedList(false, new int[0]),
                    4,
                    true,
                    new [] { 1, 1, 2, 3 },
                },
                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 1, 2, 3}),
                    GetFulledOrderedList(false, new int[0]),
                    4,
                    false,
                    new [] { 3, 2, 1, 1 },
                },

                new object[]
                {
                    GetFulledOrderedList(true, new int[0]),
                    GetFulledOrderedList(false, new int[] {1 ,2, 2, 3}),
                    4,
                    true,
                    new [] { 1, 2, 2, 3 },
                },
                new object[]
                {
                    GetFulledOrderedList(true, new int[0]),
                    GetFulledOrderedList(false, new int[] {1, 2, 2, 3}),
                    4,
                    false,
                    new [] { 3, 2, 2, 1 },
                },

                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 5, 6, 2, 3}),
                    GetFulledOrderedList(false, new int[] {1, 2, 3}),
                    8,
                    true,
                    new [] { 1, 1, 2, 2, 3, 3, 5, 6 },
                },

                new object[]
                {
                    GetFulledOrderedList(true, new int[] {1, 5, 6, 2, 3}),
                    GetFulledOrderedList(false, new int[] {1, 2, 3}),
                    8,
                    false,
                    new [] { 6, 5, 3, 3, 2, 2, 1, 1 },
                }
            };
    }
}
