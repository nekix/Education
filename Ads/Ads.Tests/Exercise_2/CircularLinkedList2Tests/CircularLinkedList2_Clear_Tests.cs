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
    public class CircularLinkedList2_Clear_Tests : CircularLinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(ClearData))]
        public void Should_Clear(CircularLinkedList2 list)
        {
            list.Clear();

            list.head.GetType().ShouldBe(typeof(DummyNode));
            list.head.next.ShouldBe(list.head);
            list.head.prev.ShouldBe(list.head);

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
