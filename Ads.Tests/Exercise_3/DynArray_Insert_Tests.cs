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
    public class DynArray_Insert_Tests : DynArray_BaseTests
    {
        [Theory]
        [MemberData(nameof(InsertWithoutReallocationData))]
        public void Should_Insert_Without_Reallocation(int index, int value, int[] sourceArray, DynArray<int> array)
        {
            var targetCount = array.count + 1;
            var targetCapacity = array.capacity;
            
            array.Insert(value, index);

            array.count.ShouldBe(targetCount);
            array.capacity.ShouldBe(targetCapacity);

            for (int i = 0; i < index; i++)
                array.array[i].ShouldBe(sourceArray[i]);

            array.array[index].ShouldBe(value);

            for (int i = index + 1; i < targetCount; i++)
                array.array[i].ShouldBe(sourceArray[i - 1]);
        }

        [Theory]
        [MemberData(nameof(InsertWithReallocationData))]
        public void Should_Insert_With_Reallocation(int index, int value, int[] sourceArray, DynArray<int> array)
        {
            var targetCount = array.count + 1;
            var targetCapacity = array.capacity * DynArray<int>.CapacityIncreaseMultiplier;

            array.Insert(value, index);

            array.count.ShouldBe(targetCount);
            array.capacity.ShouldBe(targetCapacity);

            for (int i = 0; i < index; i++)
                array.array[i].ShouldBe(sourceArray[i]);

            array.array[index].ShouldBe(value);

            for (int i = index + 1; i < targetCount; i++)
                array.array[i].ShouldBe(sourceArray[i - 1]);
        }

        [Theory]
        [MemberData(nameof(ThrowData))]
        public void Should_Throw_IndexOutOfRangeException(int index, int value, DynArray<int> array)
        {
            Assert.Throws<IndexOutOfRangeException>(() => array.Insert(index, value));
        }

        public static IEnumerable<object[]> InsertWithoutReallocationData =>
            new List<object[]>
            {
                // int index, int value, int[] sourceArray, DynArray<int> array
                new object[] { 0, 100, Array.Empty<int>(), GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },
                new object[] { 0, 100, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 10).ToArray()) },
                new object[] { 5, 100, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 10).ToArray()) },
                new object[] { 10, 100, Enumerable.Range(1, 10).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 10).ToArray()) },

                new object[] { 15, 100, Enumerable.Range(1, 15).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 15).ToArray()) },
            };

        public static IEnumerable<object[]> InsertWithReallocationData =>
            new List<object[]>
            {
                // int index, int value, int[] sourceArray, DynArray<int> array
                new object[] { 0, 100, Enumerable.Range(1, DynArray<int>.MinCapacity).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, DynArray<int>.MinCapacity).ToArray()) },
                new object[] { 10, 100, Enumerable.Range(1, DynArray<int>.MinCapacity).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, DynArray<int>.MinCapacity).ToArray()) },
                
                // Also check do not throwing exception when index is equal count;
                new object[] { DynArray<int>.MinCapacity, 100, Enumerable.Range(1, DynArray<int>.MinCapacity).ToArray(), GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, DynArray<int>.MinCapacity).ToArray()) },
            };

        public static IEnumerable<object[]> ThrowData =>
            new List<object[]>
            {
                // int index, int value, int[] sourceArray, DynArray<int> array
                new object[] { -1, 100, GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },
                new object[] { 1, 100, GetTestDynArray(DynArray<int>.MinCapacity, Array.Empty<int>()) },

                new object[] { 6, 100, GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 5).ToArray()) },
                new object[] { 17, 100, GetTestDynArray(DynArray<int>.MinCapacity, Enumerable.Range(1, 16).ToArray()) },
            };
    }
}
