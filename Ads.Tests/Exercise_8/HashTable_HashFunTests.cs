extern alias Exercise8;
using Exercise8.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_8
{
    public class HashTable_HashFunTests : HashTable_BaseTests
    {
        [Theory]
        [MemberData(nameof(HashFunData))]
        public void Should_GetHash(string data, int hash)
        {
            var hashTable = GetEmptyHashTable(17, 3);

            hashTable.HashFun(data).ShouldBe(hash);
        }

        public static IEnumerable<object[]> HashFunData =>
            new List<object[]>
            {
                new object[] { "\u0011", 0 },
                new object[] { "AbcD1.", 0 },
                new object[] { "Lorem ipsum odor amet, consectetuer adipiscing elit.", 10 }
            };
    }
}
