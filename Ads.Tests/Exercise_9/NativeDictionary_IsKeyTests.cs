extern alias Exercise9;

using Exercise9.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_9
{
    public class NativeDictionary_IsKeyTests : NativeDictionary_BaseTests
    {
        [Theory]
        [MemberData(nameof(IsKeyData))]
        public void Should_Find(NativeDictionary<int> dictionary, string key, bool isKey)
        {
            dictionary.IsKey(key).ShouldBe(isKey);
        }

        [Fact]
        public void Should_Not_Find()
        {
            var dictionary = GetEmptyNativeDictionary<int>(5);
            for (int i = 0; i < dictionary.slots.Length; i++)
            {
                dictionary.slots[i] = i.ToString();
                dictionary.values[i] = 2;
            }

            dictionary.IsKey("Abc").ShouldBe(false);
        }

        public static IEnumerable<object[]> IsKeyData()
        {
            var dictionary = GetEmptyNativeDictionary<int>(17);
            yield return new object[] { dictionary, "a1", false };

            dictionary = GetEmptyNativeDictionary<int>(17);
            dictionary.slots[12] = "a";
            dictionary.values[12] = 5;
            yield return new object[] { dictionary, "a", true };
            yield return new object[] { dictionary, "b", false };

            dictionary = GetEmptyNativeDictionary<int>(17);
            dictionary.slots[11] = ">";
            dictionary.values[11] = 1;
            dictionary.slots[12] = "AbcfD1.w";
            dictionary.values[12] = 5;
            yield return new object[] { dictionary, "AbcfD1.w", true };
            yield return new object[] { dictionary, "AbcfD1.w1", false };

            dictionary = GetEmptyNativeDictionary<int>(17);
            dictionary.slots[11] = "\u001c";
            dictionary.values[11] = 2;
            dictionary.slots[12] = ">";
            dictionary.values[12] = 3;
            dictionary.slots[13] = "AbcfD1.w";
            dictionary.values[13] = 4;
            yield return new object[] { dictionary, "AbcfD1.w", true };
            yield return new object[] { dictionary, "AbcfD1.w1", false };
        }
    }
}
