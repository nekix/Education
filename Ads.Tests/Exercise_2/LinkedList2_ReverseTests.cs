extern alias Exercise2;

using Exercise2.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_2
{
    public class LinkedList2_ReverseTests : LinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(ReverseData))]
        public void Should_Reverse(LinkedList2 list, int[] result)
        {
            list.Reverse();

            var node = list.head;
            foreach (var item in result)
            {
                node.value.ShouldBe(item);

                node = node.next;
            }
        }

        public static IEnumerable<object[]> ReverseData =>
            new List<object[]>
            {
                new object[] { GetTestLinkedList(new int[0]), new int[0]},
                new object[] { GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }), new[] { 5, 6, 9, 1, 5 } },
                new object[] { GetTestLinkedList(new[] { 1, 2, 3 }), new[] { 3, 2, 1 } },
                new object[] { GetTestLinkedList(new[] { 1 }), new[] { 1 } },
                new object[] { GetTestLinkedList(new[] { 1, 2 }), new[] { 2, 1 } },
            };
    }
}
