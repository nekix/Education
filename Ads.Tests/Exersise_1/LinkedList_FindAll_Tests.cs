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
    public class LinkedList_FindAll_Tests
    {
        [Theory]
        [InlineData(12, 4, new[] { 12, 13, 25, 12, 46, 36, 12, 46, 23, 12 })]
        [InlineData(12, 0, new int[0])]
        [InlineData(12, 4, new[] { 12, 12, 12, 12 })]
        [InlineData(12, 1, new[] { 12 })]
        [InlineData(12, 0, new[] { 1 })]
        [InlineData(12, 0, new[] { 1, 2 })]
        public void Should_FindAll_List(int value, int count, int[] nodeValues)
        {
            var list = GetTestLinkedList(nodeValues);

            var nodes = list.FindAll(value);

            nodes.Count.ShouldBe(count);
            foreach (var node in nodes)
            {
                node.value.ShouldBe(value);
            }
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
