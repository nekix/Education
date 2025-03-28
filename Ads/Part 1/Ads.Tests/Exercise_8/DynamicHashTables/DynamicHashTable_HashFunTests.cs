extern alias Exercise8;
using Exercise8.Ads.Exercise8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_8.DynamicHashTables
{
    public class DynamicHashTable_HashFunTests : DynamicHashTable_BaseTests
    {
        [Theory]
        [MemberData(nameof(HashFunData))]
        public void Should_GetHash(string data)
        {
            var hashTable = GetEmptyHashTable();

            var hash = hashTable.HashFun(data);
            hash.ShouldBeLessThan(17);
            hash.ShouldBeGreaterThanOrEqualTo(0);
        }

        public static IEnumerable<object[]> HashFunData =>
            new List<object[]>
            {
                new object[] { "\u0011" },
                new object[] { "AbcD1." },
                new object[] { "Lorem ipsum odor amet, consectetuer adipiscing elit." }
            };
    }
}
