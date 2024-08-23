using AlgorithmsDataStructures;
using Shouldly;
using Xunit;
using LinkedList = AlgorithmsDataStructures.LinkedList;
using Node = AlgorithmsDataStructures.Node;

namespace Ads.Tests.Exersise_1
{
    public class LinkedList_Remove_Tests
    {
        private readonly int _headNodeValue = 12;
        private readonly int _middleNodeValue = 55;
        private readonly int _tailNodeValue = 128;
        private readonly int _notExistedNodeValue = 10;

        [Fact]
        public void Should_Return_False_When_Remove_By_Value_Not_Existed_Node()
        {
            var list = GetTestLinkedList();

            var isDeleted = list.Remove(_notExistedNodeValue);

            isDeleted.ShouldBeFalse();
            list.head.value.ShouldBe(_headNodeValue);
            list.tail.value.ShouldBe(_tailNodeValue);
            list.head.next.value.ShouldBe(_middleNodeValue);
            list.head.next.next.ShouldBe(list.tail);
            list.tail.next.ShouldBe(null);
        }

        [Fact]
        public void Should_Remove_Head_Node_By_Value()
        {
            var list = GetTestLinkedList();

            var isDeleted = list.Remove(_headNodeValue);

            isDeleted.ShouldBeTrue();
            list.head.value.ShouldBe(_middleNodeValue);
            list.tail.value.ShouldBe(_tailNodeValue);
            list.head.next.ShouldBe(list.tail);
            list.tail.next.ShouldBe(null);
        }

        [Fact]
        public void Should_Remove_Middle_Node_By_Value()
        {
            var list = GetTestLinkedList();

            var isDeleted = list.Remove(_middleNodeValue);

            isDeleted.ShouldBeTrue();
            list.head.value.ShouldBe(_headNodeValue);
            list.tail.value.ShouldBe(_tailNodeValue);
            list.head.next.ShouldBe(list.tail);
            list.tail.next.ShouldBe(null);
        }

        [Fact]
        public void Should_Remove_Tail_Node_By_Value()
        {
            var list = GetTestLinkedList();

            var isDeleted = list.Remove(_tailNodeValue);

            isDeleted.ShouldBeTrue();
            list.head.value.ShouldBe(_headNodeValue);
            list.tail.value.ShouldBe(_middleNodeValue);
            list.head.next.ShouldBe(list.tail);
            list.tail.next.ShouldBe(null);
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