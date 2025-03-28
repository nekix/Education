extern alias Exercise2;

using Exercise2.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_2
{
    public class LinkedList2_SortMerge_Tests : LinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(MergeData))]
        public void Should_Merge(LinkedList2 first, LinkedList2 second, int count, int[] finalValues)
        {
            var list = first.SortMerge(second);

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

        public static IEnumerable<object[]> MergeData =>
            new List<object[]>
            {
                new object[]
                {
                    GetTestLinkedList(new int[0]),
                    GetTestLinkedList(new int[0]),
                    0,
                    new int[0]
                },

                new object[]
                {
                    GetTestLinkedList(new int[] {1, 1, 2, 3}),
                    GetTestLinkedList(new int[0]),
                    4,
                    new [] { 1, 1, 2, 3 },
                },

                new object[]
                {
                    GetTestLinkedList(new int[0]),
                    GetTestLinkedList(new int[] {1 ,2, 2, 3}),
                    4,
                    new [] { 1, 2, 2, 3 },
                },

                new object[]
                {
                    GetTestLinkedList(new int[] {1, 5, 6, 2, 3}),
                    GetTestLinkedList(new int[] {1, 2, 3}),
                    8,
                    new [] { 1, 1, 2, 2, 3, 3, 5, 6 },
                },
            };
    }
}
