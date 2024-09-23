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
    public class LinkedList2_IsHacLoop_Tests : LinkedList2_BaseTests
    {
        [Fact]
        public void Should_Return_True_When_HasLoop_In_Next()
        {
            var list = GetTestLinkedList(new int[] { 1, 2, 3, 4, 5 });

            list.head.next.next.next = list.head.next;

            list.IsHacLoop().ShouldBeTrue();
        }

        [Fact]
        public void Should_Return_True_When_HasLoop_In_Prev()
        {
            var list = GetTestLinkedList(new int[] { 1, 2, 3, 4, 5 });

            list.head.next.next.prev = list.tail.prev;

            list.IsHacLoop().ShouldBeTrue();
        }

        [Fact]
        public void Should_Return_False_When_HasNotLoop()
        {
            var list = GetTestLinkedList(new int[] { 1, 2, 3, 4, 5 });

            list.IsHacLoop().ShouldBeFalse();
        }
    }
}
