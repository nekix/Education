extern alias Exercise2;
using Exercise2.Ads.Exercise2;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_2.DummyLinkedList2Tests
{
    public class DummyLinkedList2_Clear_Tests : DummyLinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(ClearData))]
        public void Should_Clear(DummyLinkedList2 list)
        {
            list.Clear();

            list.head.GetType().ShouldBe(typeof(DummyNode));
            list.tail.GetType().ShouldBe(typeof(DummyNode));
            list.head.next.ShouldBe(list.tail);
            list.tail.prev.ShouldBe(list.head);

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
