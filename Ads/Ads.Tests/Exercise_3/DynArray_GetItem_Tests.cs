extern alias Exercise3;

using Exercise3.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_3
{
    public class DynArray_GetItem_Tests : DynArray_BaseTests
    {
        [Theory]
        [MemberData(nameof(GetItemData))]
        public void Should_GetItem(int itemIndex, DynArray<int> array)
        {
            var item = array.GetItem(itemIndex);

            item.ShouldBe(itemIndex + 1);
        }

        [Theory]
        [MemberData(nameof(ThrowItems))]
        public void Should_Throw_IndexOutOfRangeException(int itemIndex, DynArray<int> array)
        {
            Assert.Throws<IndexOutOfRangeException>(() => array.GetItem(itemIndex));
        }

        public static IEnumerable<object[]> ThrowItems =>
            new List<object[]>
            {
                new object[] { -1, GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },
                new object[] { 0, GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },

                new object[] { -1, GetTestDynArray(25, Enumerable.Range(1, 18).ToArray()) },
                new object[] { 18, GetTestDynArray(25, Enumerable.Range(1, 18).ToArray()) },
                new object[] { 20, GetTestDynArray(25, Enumerable.Range(1, 18).ToArray()) },
            };

        public static IEnumerable<object[]> GetItemData =>
            new List<object[]>
            {
                new object[] { 0, GetTestDynArray(25, Enumerable.Range(1, 18).ToArray()) },
                new object[] { 10, GetTestDynArray(25, Enumerable.Range(1, 18).ToArray()) },
                new object[] { 17, GetTestDynArray(25, Enumerable.Range(1, 18).ToArray()) },
            };
    }
}
