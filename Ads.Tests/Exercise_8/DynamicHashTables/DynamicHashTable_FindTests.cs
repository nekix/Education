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
    public class DynamicHashTable_FindTests : DynamicHashTable_BaseTests
    {
        [Theory]
        [MemberData(nameof(FindData))]
        public void Should_Find(DynamicHashTable hashTable, string data)
        {
            var currentSlot = hashTable.Find(data);

            currentSlot.ShouldNotBe(-1);
            hashTable.Find(data).ShouldNotBe(-1);
        }

        [Fact]
        public void Should_Not_Find()
        {
            var hashTable = GetEmptyHashTable(17, 3);
            for (int i = 0; i < hashTable.Size; i++)
                hashTable.Put("aaa");

            hashTable.Find("Abc").ShouldBe(-1);
        }

        public static IEnumerable<object[]> FindData()
        {
            var hashTable = GetEmptyHashTable(17, 3);
            hashTable.Put("a");
            var data = new object[] { hashTable, "a" };
            yield return data;

            hashTable = GetEmptyHashTable(17, 3);
            hashTable.Put("AbcD1.");
            hashTable.Put("AbcfD1.w");
            data = new object[] { hashTable, "AbcfD1.w" };
            yield return data;

            hashTable = GetEmptyHashTable(17, 3);
            hashTable.Put("AbcD1.");
            hashTable.Put("AbcfD1.");
            hashTable.Put("AbcfD1.w");
            data = new object[] { hashTable, "AbcfD1.w" };
            yield return data;
        }
    }
}
