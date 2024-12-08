extern alias Exercise8;
using Exercise8.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_8
{
    public class HashTable_SeekSlotTests : HashTable_BaseTests
    {
        [Theory]
        [MemberData(nameof(SeekSlotData))]
        public void Should_SeekSlot(HashTable hashTable, string data, int slot)
        {
            var currentSlot = hashTable.SeekSlot(data);

            currentSlot.ShouldBe(slot);
            hashTable.slots[currentSlot].ShouldBeNull();
        }

        [Fact]
        public void Should_Not_SeekSlot()
        {
            var hashTable = GetEmptyHashTable(17, 3);
            for (int i = 0; i < hashTable.slots.Length; i++)
                hashTable.slots[i] = "aaa";

            hashTable.SeekSlot("Abc").ShouldBe(-1);
        }

        public static IEnumerable<object[]> SeekSlotData()
        {
            var hashTable = GetEmptyHashTable(17, 3);
            var data = new object[] { hashTable, "a", 12 };
            yield return data;

            hashTable = GetEmptyHashTable(17, 3);
            hashTable.slots[0] = "AbcD1.";
            data = new object[] { hashTable, "AbcfD1.w", 3 };
            yield return data;

            hashTable = GetEmptyHashTable(17, 3);
            hashTable.slots[0] = "AbcD1.";
            hashTable.slots[3] = "AbcfD1.";
            data = new object[] { hashTable, "AbcfD1.w", 6 };
            yield return data;
        }
    }
}
