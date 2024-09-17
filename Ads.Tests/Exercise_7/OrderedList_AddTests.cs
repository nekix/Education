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
    public class OrderedList_AddTests : OrderedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeIComparableData))]
        public void Should_Add_Integers(OrderedList<int> list, int[] input, int[] result)
        {
            foreach (var item in input)
                list.Add(item);

            var node = list.head;
            foreach (var value in result)
            {
                node.ShouldNotBeNull();
                node.value.ShouldBe(value);
                node = node.next;
            }

            node.ShouldBeNull();
        }

        public static IEnumerable<object[]> MakeIComparableData =>
            new List<object[]>
            {
                new object[]
                {
                    GetEmptyOrderedList<int>(true), 
                    new int[] { 1, 2, 3, 4, 5 }, 
                    new int[] { 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    GetEmptyOrderedList<int>(false),
                    new int[] { 1, 2, 3, 4, 5 },
                    new int[] { 5, 4, 3, 2, 1 }
                },
                new object[]
                {
                    GetEmptyOrderedList<int>(true),
                    new int[] { 3, 5, 4, 1, 2 },
                    new int[] { 1, 2, 3, 4, 5 }
                },
                new object[]
                {
                    GetEmptyOrderedList<int>(false),
                    new int[] { 3, 5, 4, 1, 2 },
                    new int[] { 5, 4, 3, 2, 1 }
                },
            };
    }
}
