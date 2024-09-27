extern alias Exercise8;
using Exercise8.Ads.Exercise8;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_8.DynamicHashTables
{
    public class DynamicHashTable_SeekSlotTests : DynamicHashTable_BaseTests
    {
        [Theory]
        [MemberData(nameof(SeekSlotData))]
        public void Should_SeekSlot(DynamicHashTable hashTable, string data)
        {
            var currentSlot = hashTable.SeekSlot(data);

            currentSlot.ShouldNotBe(-1);
        }

        [Fact]
        public void Should_Not_SeekSlot()
        {
            var hashTable = GetEmptyHashTable();
            for (int i = 0; i < hashTable.Size; i++)
                hashTable.Put("aaa");

            hashTable.SeekSlot("Abc").ShouldBe(-1);
        }

        public static IEnumerable<object[]> SeekSlotData()
        {
            var hashTable = GetEmptyHashTable();
            var data = new object[] { hashTable, "a" };
            yield return data;

            hashTable = GetEmptyHashTable();
            hashTable.Put("AbcD1.");
            data = new object[] { hashTable, "AbcfD1.w" };
            yield return data;

            hashTable = GetEmptyHashTable();
            hashTable.Put("AbcD1.");
            hashTable.Put("AbcfD1.");
            data = new object[] { hashTable, "AbcfD1.w" };
            yield return data;
        }
    }
}
