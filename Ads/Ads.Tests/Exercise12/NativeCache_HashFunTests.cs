extern alias Exercise12;

using Exercise12.AlgorithmsDataStructures;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Ads.Tests.Exercise12
{
    public class NativeCache_HashFunTests : NativeCache_BaseTests
    {
        [Theory]
        [MemberData(nameof(HashFunData))]
        public void Should_GetHash(string data, int hash)
        {
            var cache = GetEmptyNativeCache<int>(100);

            cache.HashFun(data).ShouldBe(hash);
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
