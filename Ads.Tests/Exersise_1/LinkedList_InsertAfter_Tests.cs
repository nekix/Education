using AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace Ads.Tests.Exersise_1
{
    public class LinkedList_InsertAfter_Tests
    {

        private readonly int _headNodeValue = 12;
        private readonly int _middleNodeValue = 55;
        private readonly int _tailNodeValue = 128;
        private const int _newNodeValue = 10;

        [Theory]
        [InlineData(0, new[] { 1, 2, 3, 4, 5 }, new[] { 1, _newNodeValue, 2, 3, 4, 5 })]
        [InlineData(1, new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, _newNodeValue, 3, 4, 5 })]
        [InlineData(2, new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, _newNodeValue, 4, 5 })]
        [InlineData(3, new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, _newNodeValue, 5 })]
        [InlineData(4, new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5, _newNodeValue })]
        public void Shoudl_Insert_In_List(int afterNodeNumber, int[] nodeValues, int[] targetNodeValues)
        {
            var list = GetTestLinkedList(nodeValues);
            var nodeAfter = GetNodeByNumber(afterNodeNumber, list);
            var newNode = new Node(_newNodeValue);

            list.InsertAfter(nodeAfter, newNode);

            var node = list.head;
            for(int i = 0; i < targetNodeValues.Length; i++)
            {
                node.ShouldNotBeNull();
                node.value.ShouldBe(targetNodeValues[i]);
                node = node.next;
            }

            list.tail.value.ShouldBe(targetNodeValues[targetNodeValues.Length - 1]);
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, new[] { _newNodeValue, 1, 2, 3, 4, 5 })]
        [InlineData(new[] { 1 }, new[] { _newNodeValue, 1 })]
        [InlineData(new int[0], new[] { _newNodeValue })]
        public void Should_Insert_First_When_AfterNode_Null(int[] nodeValues, int[] targetNodeValues)
        {
            var list = GetTestLinkedList(nodeValues);
            Node nodeAfter = null;
            var newNode = new Node(_newNodeValue);

            list.InsertAfter(nodeAfter, newNode);

            var node = list.head;
            for (int i = 0; i < targetNodeValues.Length; i++)
            {
                node.ShouldNotBeNull();
                node.value.ShouldBe(targetNodeValues[i]);
                node = node.next;
            }

            list.tail.value.ShouldBe(targetNodeValues[targetNodeValues.Length - 1]);
        }

        [Fact]
        public void Should_Insert_In_Empty_List_First_When_AfterNode_Null()
        {
            var list = new LinkedList();

            Node nodeAfter = null;
            var newNode = new Node(_newNodeValue);

            list.InsertAfter(nodeAfter, newNode);

            list.head.ShouldBe(newNode);
            list.tail.ShouldBe(newNode);
            list.head.value.ShouldBe(_newNodeValue);
        }


        [Fact]
        public void Should_Insert_After_Head_Node()
        {
            var list = GetTestLinkedList();
            var nodeAfter = list.head;
            var newNode = new Node(_newNodeValue);

            list.InsertAfter(nodeAfter, newNode);

            list.head.value.ShouldBe(_headNodeValue);
            list.head.next.value.ShouldBe(_newNodeValue);
            list.head.next.next.value.ShouldBe(_middleNodeValue);
            list.head.next.next.next.value.ShouldBe(_tailNodeValue);
            list.head.next.next.next.next.ShouldBeNull();
        }

        [Fact]
        public void Should_Insert_After_Middle_Node()
        {
            var list = GetTestLinkedList();
            var nodeAfter = list.head.next;
            var newNode = new Node(_newNodeValue);

            list.InsertAfter(nodeAfter, newNode);

            list.head.value.ShouldBe(_headNodeValue);
            list.head.next.value.ShouldBe(_middleNodeValue);
            list.head.next.next.value.ShouldBe(_newNodeValue);
            list.head.next.next.next.value.ShouldBe(_tailNodeValue);
            list.head.next.next.next.next.ShouldBeNull();
        }

        [Fact]
        public void Should_Insert_After_Tail_Node()
        {
            var list = GetTestLinkedList();
            var nodeAfter = list.tail;
            var newNode = new Node(_newNodeValue);

            list.InsertAfter(nodeAfter, newNode);

            list.head.value.ShouldBe(_headNodeValue);
            list.head.next.value.ShouldBe(_middleNodeValue);
            list.head.next.next.value.ShouldBe(_tailNodeValue);
            list.head.next.next.next.value.ShouldBe(_newNodeValue);
            list.head.next.next.next.next.ShouldBeNull();
        }

        private Node GetNodeByNumber(int number, LinkedList list)
        {
            var node = list.head;

            for(int i = 0; i < number; i++)
            {
                node = node.next;
            }

            return node;
        }

        private LinkedList GetTestLinkedList()
        {
            var list = new LinkedList();

            list.AddInTail(new Node(_headNodeValue));
            list.AddInTail(new Node(_middleNodeValue));
            list.AddInTail(new Node(_tailNodeValue));

            return list;
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
