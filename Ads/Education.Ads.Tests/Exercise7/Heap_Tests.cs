using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // 11, 8, 9, 7, 5, 4, 6, 1, 2, 3, -1, -1, -1, -1, -1
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
