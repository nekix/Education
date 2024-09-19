extern alias Exercise7;

using Exercise7.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_7
{
    public class OrderedList_FindTests : OrderedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(FindFirstData))]
        public void Should_Find(OrderedList<int> list, int value, bool isExist, int? prevNodeValue, int? nextNodeValue)
        {
            var node = list.Find(value);

            if (isExist)
            {
                node.ShouldNotBeNull();
                node.value.ShouldBe(value);

                if (nextNodeValue != null)
                {
                    node.next.ShouldNotBeNull();
                    node.next.value.ShouldBe(nextNodeValue.Value);
                }

                if (prevNodeValue != null)
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
                new object[] { GetFulledOrderedList(true, 5, 1, 9, 6, 5), 1, true, null, 5},
                new object[] { GetFulledOrderedList(true, 5, 1, 9, 6, 5), 9, true, 6, null},
                new object[] { GetFulledOrderedList(true, 5, 1, 9, 6, 5), 5, true, 1, 5},

                new object[] { GetFulledOrderedList(false, 5, 1, 9, 6, 5), 1, true, 5, null},
                new object[] { GetFulledOrderedList(false, 5, 1, 9, 6, 5), 9, true, null, 6},
                new object[] { GetFulledOrderedList(false, 5, 1, 9, 6, 5), 5, true, 6, 5},

                new object[] { GetFulledOrderedList(true, 5, 1, 9, 6, 5), 0, false, null, null},
                new object[] { GetFulledOrderedList(true, 5, 1, 9, 6, 5), 10, false, null, null},
                new object[] { GetFulledOrderedList(false, 5, 1, 9, 6, 5), 0, false, null, null},
                new object[] { GetFulledOrderedList(false, 5, 1, 9, 6, 5), 10, false, null, null},
            };
    }
}
