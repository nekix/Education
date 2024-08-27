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
    public class LinkedList2_Clear_Tests : LinkedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(ClearData))]
        public void Should_Clear(LinkedList2 list)
        {
            list.Clear();

            list.head.ShouldBeNull();
            list.tail.ShouldBeNull();

            list.Count().ShouldBe(0);
        }

        public static IEnumerable<object[]> ClearData =>
            new List<object[]>
            {
                new object[] { GetTestLinkedList(new int[0]) },
                new object[] { GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }) },
                new object[] { GetTestLinkedList(new[] { 5 }) },
                new object[] { GetTestLinkedList(new[] { 5, 1 }) },
                new object[] { GetTestLinkedList(new[] { 5, 1, 3, 3, 7, 3, 34, 5 }) },
            };
    }
}
