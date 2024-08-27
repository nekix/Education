using AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exersise_1
{
    public class LinkedList_Sum_Tests
    {
        [Theory]
        [InlineData(new[] { -83, 55, 25, 24, 99, 15, 6, 7, 13, 0 }, new[] { 12, 13, 25, 12, 46, 36, 12, 46, 23, 12 })]
        [InlineData(new int[0], new int[0])]
        [InlineData(new[] { 12, 12, 12, 12 }, new[] { 12, 12, 12, 12 })]
        [InlineData(new[] { -6 }, new[] { 1 })]
        [InlineData(new[] { 4, 5 }, new[] { 1, 2 })]
        public void Should_Sum_Two_Linked_Lists(int[] firstListNodeValues, int[] secondListNodeValues)
        {
            var firstList = GetTestLinkedList(firstListNodeValues);
            var secondList = GetTestLinkedList(secondListNodeValues);

            var resultList = LinkedList.Sum(firstList, secondList);

            resultList.ShouldNotBeNull();
            resultList.Count().ShouldBe(firstListNodeValues.Length);

            var node = resultList.head;
            int i = 0;
            while(node != null)
            {
                node.value.ShouldBe(firstListNodeValues[i] + secondListNodeValues[i]);
                node = node.next;
                i += 1;
            }

            i.ShouldBe(secondListNodeValues.Length);
        }

        [Theory]
        [InlineData(new int[0], new[] { 4, 5 })]
        [InlineData(new[] { 12, 12, 12 }, new[] { 12, 12, 12, 12 })]
        [InlineData(new[] { -6 }, new[] { 1, 2 })]
        [InlineData(new[] { 4, 5 }, new int[0])]
        public void Should_Sum_Return_Null_When_Different_Lists_Nodes_Count(int[] firstListNodeValues, int[] secondListNodeValues)
        {
            var firstList = GetTestLinkedList(firstListNodeValues);
            var secondList = GetTestLinkedList(secondListNodeValues);

            var resultList = LinkedList.Sum(firstList, secondList);

            resultList.ShouldBeNull();
        }

        private LinkedList GetTestLinkedList(int[] nodValues)
        {
            var list = new LinkedList();

            foreach (var nodValue in nodValues)
            {
                list.AddInTail(new Node(nodValue));
            }

            return list;
        }
    }
}
