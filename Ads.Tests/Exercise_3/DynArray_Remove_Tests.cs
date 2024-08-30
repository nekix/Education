extern alias Exercise3;

using Exercise3.AlgorithmsDataStructures;
using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Ads.Tests.Exercise_3
{
    public class DynArray_Remove_Tests : DynArray_BaseTests
    {
        [Theory]
        [MemberData(nameof(RemoveWithoutReallocationData))]
        public void Should_Remove_Without_Reallocation(int index, int[] sourceArray, DynArray<int> array)
        {
            var targetCount = array.count - 1;
            var targetCapacity = array.capacity;

            array.Remove(index);

            array.count.ShouldBe(targetCount);
            array.capacity.ShouldBe(targetCapacity);

            for (int i = 0; i < index; i++)
                array.array[i].ShouldBe(sourceArray[i]);

            for (int i = index; i < targetCount - 1; i++)
                array.array[i].ShouldBe(sourceArray[i + 1]);
        }

        [Theory]
        [MemberData(nameof(RemoveWithReallocationData))]
        public void Should_Remove_With_Reallocation(int index, int[] sourceArray, DynArray<int> array)
        {
            var targetCount = array.count - 1;
            var targetCapacity = (int)(array.capacity / DynArray<int>.CapacityReductionMultiplier);
            targetCapacity = targetCapacity > DynArray<int>.MinCapacity
                ? targetCapacity
                : DynArray<int>.MinCapacity;

            array.Remove(index);

            array.count.ShouldBe(targetCount);
            array.capacity.ShouldBe(targetCapacity);

            for (int i = 0; i < index; i++)
                array.array[i].ShouldBe(sourceArray[i]);

            for (int i = index + 1; i < targetCount; i++)
                array.array[i - 1].ShouldBe(sourceArray[i]);
        }

        [Theory]
        [MemberData(nameof(ThrowData))]
        public void Should_Throw_IndexOutOfRangeException(int index, DynArray<int> array)
        {
            Assert.Throws<IndexOutOfRangeException>(() => array.Remove(index));
        }

        public static IEnumerable<object[]> RemoveWithReallocationData =>
            new List<object[]>
            {
                // int index, int value, int[] sourceArray, DynArray<int> array
                new object[] { 0, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(20, Enumerable.Range(1, 10).ToArray()) },
                new object[] { 0, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(20, Enumerable.Range(1, 10).ToArray()) },

                new object[] { 0, Enumerable.Range(1, 20).ToArray(), GetTestDynArray(40, Enumerable.Range(1, 20).ToArray()) },
            };

        public static IEnumerable<object[]> RemoveWithoutReallocationData =>
            new List<object[]>
            {
                // int index, int value, int[] sourceArray, DynArray<int> array
                new object[] { 0, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 10).ToArray()) },
                new object[] { 5, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 10).ToArray()) },
                new object[] { 9, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 10).ToArray()) },

                new object[] { 15, Enumerable.Range(1, 16).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 16).ToArray()) },
            };

        public static IEnumerable<object[]> ThrowData =>
            new List<object[]>
            {
                // int index, int value, int[] sourceArray, DynArray<int> array
                new object[] { -1, GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },
                new object[] { 1, GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },

                new object[] { 5, GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 5).ToArray()) },
                new object[] { 16, GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 16).ToArray()) },
            };
    }
}
