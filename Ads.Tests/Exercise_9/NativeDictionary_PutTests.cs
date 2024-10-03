extern alias Exercise9;

using Exercise9.AlgorithmsDataStructures;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Ads.Tests.Exercise_9
{
    public class NativeDictionary_PutTests : NativeDictionary_BaseTests
    {
        [Theory]
        [MemberData(nameof(PutData))]
        public void Should_Put(NativeDictionary<int> dictionary, string key, int value, int slotIndex)
        {
            dictionary.Put(key, value);

            dictionary.slots[slotIndex].ShouldBe(key);
            dictionary.values[slotIndex].ShouldBe(value);
        }

        [Fact]
        public void Should_Put_With_Replace()
        {
            var dictionary = GetEmptyNativeDictionary<int>(3);
            for (int i = 0; i < dictionary.slots.Length; i++)
            {
                dictionary.slots[i] = (3 + i).ToString();
                dictionary.values[i] = i;
            }

            dictionary.Put("4", 5);
            dictionary.values[1].ShouldBe(5);
            dictionary.slots[1].ShouldBe("4");
        }

        public static IEnumerable<object[]> PutData()
        {
            var dictionary = GetEmptyNativeDictionary<int>(3);
            var data = new object[] { dictionary, "4", 12, 1 };
            yield return data;

            dictionary = GetEmptyNativeDictionary<int>(3);
            dictionary.slots[1] = "4";
            dictionary.values[1] = 5;
            data = new object[] { dictionary, "7", 3, 2 };
            yield return data;

            dictionary = GetEmptyNativeDictionary<int>(3);
            dictionary.slots[1] = "4";
            dictionary.values[1] = 5;
            dictionary.slots[2] = "7";
            dictionary.values[2] = 2;
            data = new object[] { dictionary, "10", 3, 0 };
            yield return data;
        }
    }
}
