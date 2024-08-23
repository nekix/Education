using AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly int _newNodeValue = 10;

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

        private LinkedList GetTestLinkedList()
        {
            var list = new LinkedList();

            list.AddInTail(new Node(_headNodeValue));
            list.AddInTail(new Node(_middleNodeValue));
            list.AddInTail(new Node(_tailNodeValue));

            return list;
        }
    }
}
