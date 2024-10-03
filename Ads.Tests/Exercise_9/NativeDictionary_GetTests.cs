extern alias Exercise9;

using Exercise9.AlgorithmsDataStructures;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Ads.Tests.Exercise_9
{
    public class NativeDictionary_GetTests : NativeDictionary_BaseTests
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Should_Get(NativeDictionary<int> dictionary, string key, int value)
        {
            var currentValue = dictionary.Get(key);

            currentValue.ShouldBe(value);
        }

        [Fact]
        public void Should_Not_Get_From_Fulled()
        {
            var dictionary = GetEmptyNativeDictionary<int>(5);
            for (int i = 0; i < dictionary.slots.Length; i++)
            {
                dictionary.slots[i] = i.ToString();
                dictionary.values[i] = 2;
            }
                
            dictionary.Get("Abc").ShouldNotBe(2);
        }


        [Fact]
        public void Should_Not_Get_From_Empty()
        {
            var dictionary = GetEmptyNativeDictionary<int>(5);

            dictionary.Get("Abc").ShouldBe(default);
        }

        public static IEnumerable<object[]> GetData()
        {
            var dictionary = GetEmptyNativeDictionary<int>(17);
            dictionary.slots[12] = "a";
            dictionary.values[12] = 5;
            var data = new object[] { dictionary, "a", 5 };
            yield return data;

            dictionary = GetEmptyNativeDictionary<int>(17);
            dictionary.slots[11] = ">";
            dictionary.values[11] = 1;
            dictionary.slots[12] = "AbcfD1.w";
            dictionary.values[12] = 5;
            data = new object[] { dictionary, "AbcfD1.w", 5 };
            yield return data;

            dictionary = GetEmptyNativeDictionary<int>(17);
            dictionary.slots[11] = "\u001c";
            dictionary.values[11] = 2;
            dictionary.slots[12] = ">";
            dictionary.values[12] = 3;
            dictionary.slots[13] = "AbcfD1.w";
            dictionary.values[13] = 4;
            data = new object[] { dictionary, "AbcfD1.w", 4 };
            yield return data;
        }
    }
}
