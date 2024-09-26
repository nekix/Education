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
    public class HashTable_PutTests : HashTable_BaseTests
    {
        [Theory]
        [MemberData(nameof(PutData))]
        public void Should_Put(HashTable table, string value, int slot)
        {
            var currentSlot = table.Put(value);

            slot.ShouldBe(currentSlot);
            table.slots[currentSlot].ShouldBe(value);
        }

        [Fact]
        public void Should_Not_Put()
        {
            var hashTable = GetEmptyHashTable(17, 3);
            for (int i = 0; i < hashTable.slots.Length; i++)
                hashTable.slots[i] = "aaa";

            hashTable.Put("Abc").ShouldBe(-1);
            foreach (var slot in hashTable.slots)
                slot.ShouldBe("aaa");
        }

        public static IEnumerable<object[]> PutData()
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
