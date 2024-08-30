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
    public class DynArray_Append_Tests : DynArray_BaseTests
    {
        [Theory]
        [MemberData(nameof(GetAppendData))]
        public void Should_Append(int appendValue, DynArray<int> array)
        {
            var newCapacity = array.capacity == array.count
                ? array.capacity * DynArray<int>.CapacityIncreaseMultiplier
                : array.capacity;
            var newCount = array.count + 1;

            array.Append(appendValue);

            array.array[newCount - 1].ShouldBe(appendValue);
            array.count.ShouldBe(newCount);
            array.capacity.ShouldBe(newCapacity);
        }

        public static IEnumerable<object[]> GetAppendData =>
            new List<object[]>
            {
                new object[] { 111, GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },

                new object[] { 111, GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 5).ToArray()) },
                new object[] { 111, GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 16).ToArray()) },
            };
    }
}
