extern alias Exercise12;

using Exercise12.AlgorithmsDataStructures;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Ads.Tests.Exercise12
{
    public class NativeCache_PutTests : NativeCache_BaseTests
    {
        [Theory]
        [MemberData(nameof(PutData))]
        public void Should_Put(NativeCache<int> cache, string key, int value, int slotIndex)
        {
            cache.Put(key, value);

            cache.slots[slotIndex].ShouldBe(key);
            cache.values[slotIndex].ShouldBe(value);
        }

        [Fact]
        public void Should_Put_With_Replace()
        {
            var cache = GetEmptyNativeCache<int>(3);
            for (int i = 0; i < cache.slots.Length; i++)
            {
                cache.slots[i] = (3 + i).ToString();
                cache.values[i] = i;
            }

            cache.Put("4", 5);
            cache.values[1].ShouldBe(5);
            cache.slots[1].ShouldBe("4");
        }

        [Fact]
        public void Should_Put_With_Replace_Min_Hits()
        {
            var cache = GetEmptyNativeCache<int>(3);
            for (int i = 0; i < cache.slots.Length; i++)
            {
                cache.slots[i] = (3 + i).ToString();
                cache.values[i] = i;
            }
            cache.Get("3");
            cache.Get("5");
            cache.Get("5");

            cache.Put("8", 12);

            cache.slots[0].ShouldBe("3");
            cache.values[0].ShouldBe(0);
            cache.hits[0].ShouldBe(1);
            cache.slots[1].ShouldBe("8");
            cache.values[1].ShouldBe(12);
            cache.hits[1].ShouldBe(0);
            cache.slots[2].ShouldBe("5");
            cache.values[2].ShouldBe(2);
            cache.hits[2].ShouldBe(2);
        }

        public static IEnumerable<object[]> PutData()
        {
            var cache = GetEmptyNativeCache<int>(3);
            var data = new object[] { cache, "4", 12, 1 };
            yield return data;

            cache = GetEmptyNativeCache<int>(3);
            cache.slots[1] = "4";
            cache.values[1] = 5;
            data = new object[] { cache, "7", 3, 2 };
            yield return data;

            cache = GetEmptyNativeCache<int>(3);
            cache.slots[1] = "4";
            cache.values[1] = 5;
            cache.slots[2] = "7";
            cache.values[2] = 2;
            data = new object[] { cache, "10", 3, 0 };
            yield return data;
        }
    }
}
