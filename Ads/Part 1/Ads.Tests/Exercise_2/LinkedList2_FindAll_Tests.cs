extern alias Exercise2;

using Exercise2.AlgorithmsDataStructures;

using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Ads.Tests.Exercise_2
{
    public class LinkedList2_FindAll_Tests : LinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(FindAllData))]
        public void Should_Find_All(int value, int count, LinkedList2 list)
        {
            var nodes = list.FindAll(value);

            nodes.ShouldNotBeNull();
            nodes.Count().ShouldBe(count);
            nodes.ForEach(n => n.value.ShouldBe(value));
        }

        public static IEnumerable<object[]> FindAllData =>
            new List<object[]>
            {
                new object[] { 5, 0, GetTestLinkedList(new int[0]) },
                new object[] { 8, 0, GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }) },
                new object[] { 1, 1, GetTestLinkedList(new[] { 1, 2, 3, 4, 5 }) },
                new object[] { 5, 2, GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }) },
                new object[] { 7, 1, GetTestLinkedList(new[] { 5, 1, 9, 6, 7 }) },
                new object[] { 3, 2, GetTestLinkedList(new[] { 5, 1, 3, 3, 7 }) },
                new object[] { 2, 5, GetTestLinkedList(new[] { 2, 2, 2, 2, 2 }) }
            };
    }
}
