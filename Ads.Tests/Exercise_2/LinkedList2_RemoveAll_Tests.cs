﻿extern alias Exercise2;

using Exercise2.AlgorithmsDataStructures;

using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Ads.Tests.Exercise_2
{
    public class LinkedList2_RemoveAll_Tests : LinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(RemoveAllData))]
        public void Should_RemoveAll(int value, int newCount, LinkedList2 list, int[] finalValues)
        {
            var node = list.Find(value);
            list.RemoveAll(value);

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

        public static IEnumerable<object[]> RemoveAllData =>
            new List<object[]>
            {
                    new object[] { 5, 0, GetTestLinkedList(new int[0]), new int[0]},
                    new object[] { 8, 5, GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }), new[] { 5, 1, 9, 6, 5 } },
                    new object[] { 1, 4, GetTestLinkedList(new[] { 1, 2, 3, 4, 5 }), new[] { 2, 3, 4, 5 } },
                    new object[] { 5, 3, GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }), new[] { 1, 9, 6 } },
                    new object[] { 3, 3, GetTestLinkedList(new[] { 5, 1, 3, 3, 7 }), new[] { 5, 1, 7} },
                    new object[] { 2, 0, GetTestLinkedList(new[] { 2, 2, 2, 2, 2 }), new int[0] },
                    new object[] { 1, 2, GetTestLinkedList(new[] { 1, 2, 3 }), new[] { 2, 3 } },
                    new object[] { 2, 2, GetTestLinkedList(new[] { 1, 2, 3 }), new[] { 1, 3 } },
                    new object[] { 3, 2, GetTestLinkedList(new[] { 1, 2, 3 }), new[] { 1, 2 } },
                    new object[] { 1, 0, GetTestLinkedList(new[] { 1 }), new int[0] },
                    new object[] { 1, 1, GetTestLinkedList(new[] { 1, 2 }), new[] { 2 } },
                    new object[] { 2, 1, GetTestLinkedList(new[] { 1, 2 }), new[] { 1 } },
            };
    }
}
