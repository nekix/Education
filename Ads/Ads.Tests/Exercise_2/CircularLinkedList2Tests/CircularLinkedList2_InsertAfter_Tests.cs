extern alias Exercise2;
using Exercise2.Ads.Exercise2.CircleDummyLinkedLists;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_2.CircularLinkedList2Tests
{
    public class CircularLinkedList2_InsertAfter_Tests : CircularLinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(InsertAfterData))]
        public void Should_InsertAfter(int newNodeValue, int afterNodeNumber, CircularLinkedList2 list, int[] targetNodeValues)
        {
            var nodeAfter = GetNodeByNumber(afterNodeNumber, list);

            var newNode = new Node(newNodeValue);

            list.InsertAfter(nodeAfter, newNode);

            var node = list.head.next;
            foreach (var item in targetNodeValues)
            {
                node.value.ShouldBe(item);
                node.next.prev.ShouldBe(node);
                node = node.next;
            }

            if (targetNodeValues.Length != 1)
            {
                list.head.prev.value.ShouldBe(targetNodeValues[targetNodeValues.Length - 1]);
            }
            else
            {
                list.head.GetType().ShouldBe(typeof(DummyNode));
                list.head.next.next.ShouldBe(list.head);
                list.head.prev.prev.ShouldBe(list.head);
            }
        }

        public static IEnumerable<object[]> InsertAfterData =>
            new List<object[]>
            {
                    new object[] { 5, -1, GetTestLinkedList(new int[0]), new[] { 5 } },
                    new object[] { 5, -1, GetTestLinkedList(new[] { 1 }), new[] { 5, 1 } },
                    new object[] { 5, -1, GetTestLinkedList(new[] { 1, 2 }), new[] { 5, 1, 2 } },

                    new object[] { 5, 0, GetTestLinkedList(new[] { 1 }), new[] { 1, 5 } },
                    new object[] { 5, 0, GetTestLinkedList(new[] { 1, 2 }), new[] { 1, 5, 2 } },

                    new object[] { 5, 1, GetTestLinkedList(new[] { 1, 2 }), new[] { 1, 2, 5 } },

                    new object[] { 5, 0, GetTestLinkedList(new[] { 1, 2, 3 }), new[] { 1, 5, 2, 3 } },
                    new object[] { 5, 1, GetTestLinkedList(new[] { 1, 2, 3 }), new[] { 1, 2, 5, 3 } },
                    new object[] { 5, 2, GetTestLinkedList(new[] { 1, 2, 3 }), new[] { 1, 2, 3, 5 } }
            };


        private Node GetNodeByNumber(int number, CircularLinkedList2 list)
        {
            if(number == -1)
            {
                return null;
            }

            var node = list.head.next;

            for (int i = 0; i < number; i++)
            {
                node = node.next;
            }

            return node;
        }
    }
}
