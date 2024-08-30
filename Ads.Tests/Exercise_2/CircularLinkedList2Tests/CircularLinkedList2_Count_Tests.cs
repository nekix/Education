extern alias Exercise2;
using Exercise2.Ads.Exercise2.CircleDummyLinkedLists;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Ads.Tests.Exercise_2.CircularLinkedList2Tests
{
    public class LinkedList_Count_Tests : CircularLinkedList2_BaseTests
    {
        [Theory]
        [MemberData(nameof(CountData))]
        public void Should_Count(int number, CircularLinkedList2 list)
        {
            var currentNumber = list.Count();

            currentNumber.ShouldBe(number);
        }

        public static IEnumerable<object[]> CountData =>
            new List<object[]>
            {
                new object[] { 0, GetTestLinkedList(new int[0]) },
                new object[] { 5, GetTestLinkedList(new[] { 5, 1, 9, 6, 5 }) },
                new object[] { 1, GetTestLinkedList(new[] { 5 }) },
                new object[] { 2, GetTestLinkedList(new[] { 5, 1 }) },
                new object[] { 8, GetTestLinkedList(new[] { 5, 1, 3, 3, 7, 3, 34, 5 }) },
            };
    }
}
