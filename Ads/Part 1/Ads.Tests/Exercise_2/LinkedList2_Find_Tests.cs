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
    public class LinkedList2_Find_Tests : LinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(FindFirstData))]
        public void Should_Find_First(int value, bool isExist, int? prevNodeValue, int? nextNodeValue, LinkedList2 list)
        {
            var node = list.Find(value);

            if (isExist)
            {
                node.ShouldNotBeNull();
                node.value.ShouldBe(value);

                if(nextNodeValue != null)
                {
                    node.next.ShouldNotBeNull();
                    node.next.value.ShouldBe(nextNodeValue.Value);
                }

                if(prevNodeValue != null)
                {
                    node.prev.ShouldNotBeNull();
                    node.prev.value.ShouldBe(prevNodeValue.Value);
                }
            }
            else
            {
                node.ShouldBeNull();
            }
        }

        public static IEnumerable<object[]> FindFirstData =>
            new List<object[]>
            {
                new object[] { 5, false, null, null, GetTestLinkedList(new int[0]) },
                new object[] { 8, false, null, null, GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }) },
                new object[] { 5, true, null, 1, GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }) },
                new object[] { 7, true, 6, null, GetTestLinkedList(new[] { 5, 1, 9, 6, 7 }) },
                new object[] { 3, true, 1, 3, GetTestLinkedList(new[] { 5, 1, 3, 3, 7 }) },
                new object[] { 2, true, null, 2, GetTestLinkedList(new[] { 2, 2, 2, 2, 2 }) }
            };
    }
}
