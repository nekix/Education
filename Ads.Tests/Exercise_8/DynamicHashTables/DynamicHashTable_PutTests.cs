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
    public class DynamicHashTable_PutTests : DynamicHashTable_BaseTests
    {
        [Theory]
        [MemberData(nameof(PutData))]
        public void Should_Put(DynamicHashTable table, string value)
        {
            var currentSlot = table.Put(value);

            currentSlot.ShouldNotBe(-1);
            table.Find(value).ShouldNotBe(-1);
        }

        [Fact]
        public void Should_Put_With_Resize()
        {
            var hashTable = GetEmptyHashTable(17, 3);
            for (int i = 0; i < 17; i++)
                hashTable.Put(((char)(34 + i)).ToString());

            var slot = hashTable.Put(((char)(60)).ToString());

            slot.ShouldNotBe(-1);
            hashTable.Find(((char)(60)).ToString()).ShouldNotBe(-1);
            hashTable.Size.ShouldBe(34);
        }

        public static IEnumerable<object[]> PutData()
        {
            var hashTable = GetEmptyHashTable(17, 3);
            var data = new object[] { hashTable, "a" };
            yield return data;

            hashTable = GetEmptyHashTable(17, 3);
            hashTable.Put("AbcD1.");
            data = new object[] { hashTable, "AbcfD1.w" };
            yield return data;

            hashTable = GetEmptyHashTable(17, 3);
            hashTable.Put("AbcD1.");
            hashTable.Put("AbcfD1.");
            data = new object[] { hashTable, "AbcfD1.w" };
            yield return data;
        }
    }
}
