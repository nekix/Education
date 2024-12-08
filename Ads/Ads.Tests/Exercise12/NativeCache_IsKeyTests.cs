extern alias Exercise12;

using Exercise12.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise12
{
    public class NativeCache_IsKeyTests : NativeCache_BaseTests
    {
        [Theory]
        [MemberData(nameof(IsKeyData))]
        public void Should_Find(NativeCache<int> cache, string key, bool isKey)
        {
            cache.IsKey(key).ShouldBe(isKey);
        }

        [Fact]
        public void Should_Not_Find()
        {
            var cache = GetEmptyNativeCache<int>(5);
            for (int i = 0; i < cache.slots.Length; i++)
            {
                cache.slots[i] = i.ToString();
                cache.values[i] = 2;
            }

            cache.IsKey("Abc").ShouldBe(false);
        }

        public static IEnumerable<object[]> IsKeyData()
        {
            var cache = GetEmptyNativeCache<int>(17);
            yield return new object[] { cache, "a1", false };

            cache = GetEmptyNativeCache<int>(17);
            cache.slots[12] = "a";
            cache.values[12] = 5;
            yield return new object[] { cache, "a", true };
            yield return new object[] { cache, "b", false };

            cache = GetEmptyNativeCache<int>(17);
            cache.slots[11] = ">";
            cache.values[11] = 1;
            cache.slots[12] = "AbcfD1.w";
            cache.values[12] = 5;
            yield return new object[] { cache, "AbcfD1.w", true };
            yield return new object[] { cache, "AbcfD1.w1", false };

            cache = GetEmptyNativeCache<int>(17);
            cache.slots[11] = "\u001c";
            cache.values[11] = 2;
            cache.slots[12] = ">";
            cache.values[12] = 3;
            cache.slots[13] = "AbcfD1.w";
            cache.values[13] = 4;
            yield return new object[] { cache, "AbcfD1.w", true };
            yield return new object[] { cache, "AbcfD1.w1", false };
        }
    }
}
