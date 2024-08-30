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
    public class DynArray_MakeArray_Tests : DynArray_BaseTests
    {
        [Theory]
        [MemberData(nameof(MakeArrayData))]
        public void Should_MakeArray(int newCapacity, int[] sourceArray, DynArray<int> array)
        {
            array.MakeArray(newCapacity);

            var targetCapacity = newCapacity < DynArray<int>.MinCapacity
                ? DynArray<int>.MinCapacity
                : newCapacity;

            var targetCount = sourceArray.Length < targetCapacity
                ? sourceArray.Length
                : targetCapacity;

            array.capacity.ShouldBe(targetCapacity);
            array.count.ShouldBe(targetCount);

            for (int i = 0; i < newCapacity; i++)
            {
                array.array[i].ShouldBe(i < targetCount ? sourceArray[i] : default);
            }
        }

        public static IEnumerable<object[]> MakeArrayData =>
            new List<object[]>
            {
                new object[] { 0, Array.Empty<int>(), GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },
                new object[] { DynArray<int>.MinCapacity, Array.Empty<int>(), GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },
                new object[] { DynArray<int>.MinCapacity + 1, Array.Empty<int>(), GetTestDynArray(DynArray<int>.MinCapacity + 1, Array.Empty<int>()) },
                new object[] { DynArray<int>.MinCapacity + 2, Array.Empty<int>(), GetTestDynArray(DynArray<int>.MinCapacity + 2, Array.Empty<int>()) },

                new object[] { 0, Enumerable.Range(1, 13).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 13).ToArray()) },
                new object[] { 6, Enumerable.Range(1, 16).ToArray(), GetTestDynArray(16, Enumerable.Range(1, 16).ToArray()) },
                new object[] { 18, Enumerable.Range(1, 25).ToArray(), GetTestDynArray(30, Enumerable.Range(1, 25).ToArray()) },
                new object[] { 30, Enumerable.Range(1, 25).ToArray(), GetTestDynArray(25, Enumerable.Range(1, 25).ToArray()) },
            };
    }
}
