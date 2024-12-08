extern alias Exercise12;

using Exercise12.AlgorithmsDataStructures;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Ads.Tests.Exercise12
{
    public class NativeCache_GetTests : NativeCache_BaseTests
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Should_Get(NativeCache<int> dictionary, string key, int value)
        {
            var currentValue = dictionary.Get(key);

            currentValue.ShouldBe(value);
        }

        [Fact]
        public void Should_Not_Get_From_Fulled()
        {
            var cache = GetEmptyNativeCache<int>(5);
            for (int i = 0; i < cache.slots.Length; i++)
            {
                cache.slots[i] = i.ToString();
                cache.values[i] = 2;
            }

            cache.Get("Abc").ShouldNotBe(2);
        }


        [Fact]
        public void Should_Not_Get_From_Empty()
        {
            var cache = GetEmptyNativeCache<int>(5);

            cache.Get("Abc").ShouldBe(default);
        }

        [Fact]
        public void Should_Count_Hits_When_Get()
        {
            var cache = GetEmptyNativeCache<int>(5);
            for (int i = 0; i < cache.slots.Length; i++)
            {
                cache.slots[i] = i.ToString();
                cache.values[i] = 2;
            }

            cache.Get("1");
            cache.Get("1");
            cache.Get("1");

            cache.hits[1].ShouldBe(3);
        }

        public static IEnumerable<object[]> GetData()
        {
            var cache = GetEmptyNativeCache<int>(17);
            cache.slots[12] = "a";
            cache.values[12] = 5;
            var data = new object[] { cache, "a", 5 };
            yield return data;

            cache = GetEmptyNativeCache<int>(17);
            cache.slots[11] = ">";
            cache.values[11] = 1;
            cache.slots[12] = "AbcfD1.w";
            cache.values[12] = 5;
            data = new object[] { cache, "AbcfD1.w", 5 };
            yield return data;

            cache = GetEmptyNativeCache<int>(17);
            cache.slots[11] = "\u001c";
            cache.values[11] = 2;
            cache.slots[12] = ">";
            cache.values[12] = 3;
            cache.slots[13] = "AbcfD1.w";
            cache.values[13] = 4;
            data = new object[] { cache, "AbcfD1.w", 4 };
            yield return data;
        }
    }
}
