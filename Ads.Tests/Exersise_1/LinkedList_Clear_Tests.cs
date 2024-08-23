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
    public class LinkedList_Clear_Tests
    {
        [Theory]
        [InlineData(new[] { 12, 13, 25, 12, 46, 36, 12, 46, 23, 12 })]
        [InlineData(new int[0])]
        [InlineData(new[] { 12, 12, 12, 12 })]
        [InlineData(new[] { 12 })]
        [InlineData(new[] { 1 })]
        [InlineData(new[] { 1, 2 })]
        public void Should_Clear_List(int[] nodeValues)
        {
            var list = GetTestLinkedList(nodeValues);

            list.Clear();

            list.head.ShouldBeNull();
            list.tail.ShouldBeNull();
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
