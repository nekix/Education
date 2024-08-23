using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exersise_1
{
    public class LinkedList_RemoveAll_Tests
    {
        [Theory]
        [InlineData(12, new [] { 12, 13, 25, 12, 46, 36, 12, 46, 23, 12 })]
        [InlineData(12, new int[0])]
        [InlineData(12, new [] { 12, 12, 12, 12 } )]
        [InlineData(12, new[] { 12 })]
        [InlineData(12, new[] { 1 })]
        [InlineData(12, new[] { 1, 2 })]
        public void Should_Remove_All_By_Value(int nodeValue, int[] nodeValues)
        {
            var list = GetTestLinkedList(nodeValues);

            list.RemoveAll(nodeValue);

            list.FindAll(12).ShouldBeEmpty();

            foreach (var currentValue in nodeValues
                         .Where(v => v != nodeValue)
                         .Distinct())
            {
                list.Find(currentValue).ShouldNotBeNull();
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
