extern alias Exercise9;

using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_9
{
    public class NativeDictionary_HashFunTests : NativeDictionary_BaseTests
    {
        [Theory]
        [MemberData(nameof(HashFunData))]
        public void Should_GetHash(string data, int hash)
        {
            var hashTable = GetEmptyNativeDictionary<int>(100);

            hashTable.HashFun(data).ShouldBe(hash);
        }

        public static IEnumerable<object[]> HashFunData =>
            new List<object[]>
            {
                new object[] { "\u0011", 17 },
                new object[] { "AbcD1.", 31 },
                new object[] { ".1DcbA", 85 },
                new object[] { "Lorem ipsum odor amet, consectetuer adipiscing elit.", 11 }
            };
    }
}
