using System.Collections.Generic;
using AlgorithmsDataStructures2;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise7
{
    public class Heap_Tests
    {
        [Theory]
        [MemberData(nameof(GetMakeHeapData))]
        public void Should_MakeHeap(int[] a, int depth, int[] heapArray)
        {
            Heap heap = new Heap();
            
            heap.MakeHeap(a, depth);

            heap.HeapArray.ShouldBe(heapArray);
        }

        [Theory]
        [MemberData(nameof(GetGetMaxData))]
        public void Should_GetMax(int[] a, int depth, int maxKey, int[] heapArray)
        {
            Heap heap = new Heap();
            heap.MakeHeap(a, depth);

            heap.GetMax().ShouldBe(maxKey);
            heap.HeapArray.ShouldBe(heapArray);
        }

        [Theory]
        [MemberData(nameof(GetAddData))]
        public void Should_Add(int[] a, int depth, int addKey, bool result, int[] heapArray)
        {
            Heap heap = new Heap();
            heap.MakeHeap(a, depth);

            heap.Add(addKey).ShouldBe(result);
            heap.HeapArray.ShouldBe(heapArray);
        }

        [Theory]
        [MemberData(nameof(GetIsHeapData))]
        public void Should_IsHeap(int[] a, bool result)
        {
            Heap.IsHeap(a).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(GetGetMaxInRangeData))]
        public void Should_GetMaxInRange(int[] a, int depth, int minValue, int maxValue, int res)
        {
            Heap heap = new Heap();
            heap.MakeHeap(a, depth);

            heap.GetMaxInRange(minValue, maxValue).ShouldBe(res);
        }

        [Theory]
        [MemberData(nameof(GetUnionData))]
        public void Should_Union(int[] first, int firstDepth, int[] second, int secondDepth, int[] unionHeap)
        {
            Heap firstHeap = new Heap();
            firstHeap.MakeHeap(first, firstDepth);
            Heap secondHeap = new Heap();
            secondHeap.MakeHeap(second, secondDepth);

            Heap newHeap = firstHeap.Union(secondHeap);

            newHeap.HeapArray.ShouldBe(unionHeap);
        }

        public static IEnumerable<object[]> GetUnionData()
        {
            // 1: Empty
            int[] first = new int[] { };
            int firstDepth = 2;
            int[] second = new int[] { };
            int secondDepth = 3;
            int[] unionHeap = new int[] { -1 };
            yield return new object[] { first, firstDepth, second, secondDepth, unionHeap };

            // 2: Two heaps with equal depth
            first = new int[] { 6, 1, 8, 3, 11, 4, 9, 2, 7, 5 };
            firstDepth = 3;
            second = new int[] { 17, 13, 16, 3, 10, 2, 12 };
            secondDepth = 3;
            unionHeap = new int[] 
            { 17, 16, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 3, 2, 2, 1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            yield return new object[] { first, firstDepth, second, secondDepth, unionHeap };

            // 3: Two heaps with diff depth
            first = new int[] { 6, 1, 8, 3, 11, 4, 9, 2, 7, 5 };
            firstDepth = 3;
            second = new int[] { 17, 13, 16 };
            secondDepth = 2;
            unionHeap = new int[]
            { 17, 16, 13, 11, 9, 8, 7, 6, 5, 4, 3, 2, 1, -1, -1 };
            yield return new object[] { first, firstDepth, second, secondDepth, unionHeap };

            // 4: Two heap, one empty
            first = new int[] { };
            firstDepth = 3;
            second = new int[] { 17, 13, 16 };
            secondDepth = 2;
            unionHeap = new int[]
            { 17, 16, 13 };
            yield return new object[] { first, firstDepth, second, secondDepth, unionHeap };
        }

        public static IEnumerable<object[]> GetGetMaxInRangeData()
        {
            // 1: Empty array
            int[] a = new int[] { };
            int depth = 0;
            int minValue = 5;
            int maxValue = 10;
            int res = -1;
            yield return new object[] { a, depth, minValue, maxValue, res };

            // 2: Empty array and depth 2
            a = new int[] { };
            depth = 2;
            minValue = 5;
            maxValue = 10;
            res = -1;
            yield return new object[] { a, depth, minValue, maxValue, res };

            // 3: One element and depth 2
            a = new int[] { 11 };
            depth = 2;
            minValue = 5;
            maxValue = 15;
            res = 11;
            yield return new object[] { a, depth, minValue, maxValue, res };

            // 4: One element and depth 2, not in range
            a = new int[] { 11 };
            depth = 2;
            minValue = 12;
            maxValue = 15;
            res = -1;
            yield return new object[] { a, depth, minValue, maxValue, res };

            // 5: From lesson, three levels, in range
            a = new int[] { 6, 1, 8, 3, 11, 4, 9, 2, 7, 5 };
            depth = 3;
            minValue = 4;
            maxValue = 6;
            res = 6;
            yield return new object[] { a, depth, minValue, maxValue, res };

            // 6: From lesson, three levels, not in range
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, 7, 2, 1, 0, 0 };
            depth = 3;
            minValue = 12;
            maxValue = 15;
            res = -1;
            yield return new object[] { a, depth, minValue, maxValue, res };
        }

        public static IEnumerable<object[]> GetIsHeapData()
        {
            // 1: Empty array
            int[] a = new int[] { };
            yield return new object[] { a, false };

            // 2: One level empty array
            a = new int[] { -1 };
            yield return new object[] { a, true };

            // 3: One level full array
            a = new int[] { 10 };
            yield return new object[] { a, true };

            // 4: Empty array deep 4
            a = new int[]
            {
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1
            };
            yield return new object[] { a, true };

            // 5: Not valid heap array deep 4
            a = new int[]
            {
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, 10, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1
            };
            yield return new object[] { a, false };

            // 6: Two levels, not valid
            a = new int[] { 9, 11, -1 };
            yield return new object[] { a, false };

            // 7: Two levels, valid
            a = new int[] { 11, 9, -1 };
            yield return new object[] { a, true };

            // 8: From lesson, 3 levels, not valid
            a = new int[] { 11, 9, 4, 7, 8, 5, 1, 2, 5, 6 };
            yield return new object[] { a, false };

            // 9: From lesson, 3 not full levels, not valid
            a = new int[] { 11, 9, 4, 7, 8, 3, 1 };
            yield return new object[] { a, true };

            // 10: From lesson, 3 levels, not full, not valid
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6 };
            yield return new object[] { a, false };

            // 11: From lesson, 4 levels, full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, -1, -1, -1, -1, -1 };
            yield return new object[] { a, true };

            // 12: From lesson, three levels, full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, 7, 2, 1, 0, 0 };
            yield return new object[] { a, true };

            // 13: From lesson, three levels, full, not valid
            a = new int[] { 11, 9, 4, 7, 8, 3, 14, 2, 5, 6, 7, 2, 1, 0, 0 };
            yield return new object[] { a, false };
        }

        public static IEnumerable<object[]> GetMakeHeapData()
        {
            // 1: Empty array
            int[] a = new int[] { };
            int depth = 0;
            int[] heapArray = new int[] { -1 };
            yield return new object[] { a, depth, heapArray };

            // 2: Empty array and deep 4
            a = new int[] { };
            depth = 4;
            heapArray = new int[]
            {
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1
            };
            yield return new object[] { a, depth, heapArray };

            // 3: One element
            a = new int[] { 11 };
            depth = 0;
            heapArray = new int[] { 11 };
            yield return new object[] { a, depth, heapArray };

            // 4: Two levels, not full
            a = new int[] { 9, 11 };
            depth = 1;
            heapArray = new int[] { 11, 9, -1 };
            yield return new object[] { a, depth, heapArray };

            // 5: Two levels, full
            a = new int[] { 9, 4, 11 };
            depth = 1;
            heapArray = new int[] { 11, 4, 9 };
            yield return new object[] { a, depth, heapArray };

            // 6: From lesson, two levels
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6 };
            depth = 2;
            heapArray = new int[] { 11, 9, 4, 7, 8, 3, 1 };
            yield return new object[] { a, depth, heapArray };

            // 7: From lesson, three levels, not full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6 };
            depth = 3;
            heapArray = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, -1, -1, -1, -1, -1 };
            yield return new object[] { a, depth, heapArray };

            // 8: From lesson, other order, three levels, not full
            a = new int[] { 6, 1, 8, 3, 11, 4, 9, 2, 7, 5 };
            depth = 3;
            heapArray = new int[] { 11, 8, 9, 7, 5, 4, 6, 1, 2, 3, -1, -1, -1, -1, -1 };
            yield return new object[] { a, depth, heapArray };

            // 9: From lesson, three levels, full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, 7, 2, 1, 0, 0 };
            depth = 3;
            heapArray = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, 7, 2, 1, 0, 0 };
            yield return new object[] { a, depth, heapArray };
        }

        public static IEnumerable<object[]> GetGetMaxData()
        {
            // 1: Empty array
            int[] a = new int[] { };
            int depth = 0;
            int max = -1;
            int[] heapArray = new int[] { -1 };
            yield return new object[] { a, depth, max, heapArray };

            // 2: Empty array and deep 4
            a = new int[] { };
            depth = 4;
            max = -1;
            heapArray = new int[]
            {
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1
            };
            yield return new object[] { a, depth, max, heapArray };

            // 3: One element
            a = new int[] { 11 };
            depth = 0;
            max = 11;
            heapArray = new int[] { -1 };
            yield return new object[] { a, depth, max, heapArray };

            // 4: Two levels, not full
            a = new int[] { 9, 11 };
            depth = 1;
            max = 11;
            heapArray = new int[] { 9, -1, -1 };
            yield return new object[] { a, depth, max, heapArray };

            // 5: Two levels, full
            a = new int[] { 9, 4, 11 };
            depth = 1;
            max = 11;
            heapArray = new int[] { 9, 4, -1 };
            yield return new object[] { a, depth, max, heapArray };

            // 6: From lesson, two levels
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6 };
            depth = 2;
            max = 11;
            heapArray = new int[] { 9, 8, 4, 7, 1, 3, -1 };
            yield return new object[] { a, depth, max, heapArray };

            // 7: From lesson, three levels, not full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6 };
            depth = 3;
            max = 11;
            heapArray = new int[] { 9, 8, 4, 7, 6, 3, 1, 2, 5, -1, -1, -1, -1, -1, -1 };
            yield return new object[] { a, depth, max, heapArray };

            // 8: From lesson, other order, three levels, not full
            a = new int[] { 6, 1, 8, 3, 11, 4, 9, 2, 7, 5 };
            depth = 3;
            max = 11;
            heapArray = new int[] { 9, 8, 6, 7, 5, 4, 3, 1, 2, -1, -1, -1, -1, -1, -1 };
            yield return new object[] { a, depth, max, heapArray };

            // 9: From lesson, three levels, full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, 7, 2, 1, 0, 0 };
            depth = 3;
            max = 11;
            heapArray = new int[] { 9, 8, 4, 7, 7, 3, 1, 2, 5, 6, 0, 2, 1, 0, -1 };
            yield return new object[] { a, depth, max, heapArray };
        }

        public static IEnumerable<object[]> GetAddData()
        {
            // 1: Empty array
            int[] a = new int[] { };
            int depth = 0;
            int key = 2;
            bool result = true;
            int[] heapArray = new int[] { 2 };
            yield return new object[] { a, depth, key, result, heapArray };

            // 2: Empty array and deep 4
            a = new int[] { };
            depth = 4;
            key = 10;
            result = true;
            heapArray = new int[]
            {
                10, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1, -1,
                -1, -1, -1
            };
            yield return new object[] { a, depth, key, result, heapArray };

            // 3: One element
            a = new int[] { 11 };
            depth = 0;
            key = 10;
            result = false;
            heapArray = new int[] { 11 };
            yield return new object[] { a, depth, key, result, heapArray };

            // 4: Two levels, not full
            a = new int[] { 9, 11 };
            depth = 1;
            key = 10;
            result = true;
            heapArray = new int[] { 11, 9, 10 };
            yield return new object[] { a, depth, key, result, heapArray };

            // 5: Two levels, full
            a = new int[] { 9, 4, 11 };
            depth = 1;
            key = 7;
            result = false;
            heapArray = new int[] { 11, 4, 9 };
            yield return new object[] { a, depth, key, result, heapArray };

            // 6: From lesson, two levels
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6 };
            depth = 2;
            key = 2;
            result = false;
            heapArray = new int[] { 11, 9, 4, 7, 8, 3, 1 };
            yield return new object[] { a, depth, key, result, heapArray };

            // 7: From lesson, three levels, not full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6 };
            depth = 3;
            key = 10;
            result = true;
            heapArray = new int[] { 11, 10, 4, 7, 9, 3, 1, 2, 5, 6, 8, -1, -1, -1, -1 };
            yield return new object[] { a, depth, key, result, heapArray };

            // 8: From lesson, other order, three levels, not full
            a = new int[] { 6, 1, 8, 3, 11, 4, 9, 2, 7, 5 };
            depth = 3;
            key = 10;
            result = true;
            heapArray = new int[] { 11, 10, 9, 7, 8, 4, 6, 1, 2, 3, 5, -1, -1, -1, -1 };
            yield return new object[] { a, depth, key, result, heapArray };

            // 9: From lesson, three levels, full
            a = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, 7, 2, 1, 0, 0 };
            depth = 3;
            key = 10;
            result = false;
            heapArray = new int[] { 11, 9, 4, 7, 8, 3, 1, 2, 5, 6, 7, 2, 1, 0, 0 };
            yield return new object[] { a, depth, key, result, heapArray };
        }
    }
}
