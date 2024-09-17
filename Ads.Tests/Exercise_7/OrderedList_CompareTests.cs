extern alias Exercise7;

using Exercise7.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_7
{
    public class OrderedList_CompareTests : OrderedList_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeIComparableData))]
        public void Should_Compare_Integers(OrderedList<int> list, int v1, int v2, int result)
        {
            var res = list.Compare(v1, v2);

            res.ShouldBe(result);
        }

        public static IEnumerable<object[]> MakeIComparableData =>
            new List<object[]>
            {
                new object[] { GetEmptyOrderedList<int>(true), 5, 5, 0},
                new object[] { GetEmptyOrderedList<int>(true), 3, 7, -1},
                new object[] { GetEmptyOrderedList<int>(true), 7, 3, 1},
            };
    }
}
