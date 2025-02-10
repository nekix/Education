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
            throw new NotImplementedException();
        }

        public static IEnumerable<object[]> GetGetMaxData()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<object[]> GetAddData()
        {
            throw new NotImplementedException();
        }
    }
}
