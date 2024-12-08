extern alias Exercise11;

using Exercise11.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise11
{
    public class BloomFilter_HashTests : BloomFilter_BaseTests
    {
        [Theory]
        [MemberData(nameof(Hash1FunData))]
        public void Should_Hash1(BloomFilter filter, string data, int resultHash)
        {
            filter.Hash1(data).ShouldBe(resultHash);
        }

        [Theory]
        [MemberData(nameof(Hash2FunData))]
        public void Should_Hash2(BloomFilter filter, string data, int resultHash)
        {
            filter.Hash2(data).ShouldBe(resultHash);
        }

        public static IEnumerable<object[]> Hash1FunData =>
            new List<object[]>
            {
                new object[] { GetEmptyBloomFilter(32), "0123456789", 13 },
                new object[] { GetEmptyBloomFilter(32), "1234567890", 29 },
                new object[] { GetEmptyBloomFilter(32), "2345678901", 13 },
                new object[] { GetEmptyBloomFilter(32), "3456789012", 29 },
                new object[] { GetEmptyBloomFilter(32), "4567890123", 13 },
                new object[] { GetEmptyBloomFilter(32), "5678901234", 29 },
                new object[] { GetEmptyBloomFilter(32), "6789012345", 13 },
                new object[] { GetEmptyBloomFilter(32), "7890123456", 29 },
                new object[] { GetEmptyBloomFilter(32), "8901234567", 13 },
                new object[] { GetEmptyBloomFilter(32), "9012345678", 29 },
            };

        public static IEnumerable<object[]> Hash2FunData =>
            new List<object[]>
            {
                new object[] { GetEmptyBloomFilter(32), "0123456789", 5 },
                new object[] { GetEmptyBloomFilter(32), "1234567890", 27 },
                new object[] { GetEmptyBloomFilter(32), "2345678901", 5 },
                new object[] { GetEmptyBloomFilter(32), "3456789012", 27 },
                new object[] { GetEmptyBloomFilter(32), "4567890123", 5 },
                new object[] { GetEmptyBloomFilter(32), "5678901234", 27 },
                new object[] { GetEmptyBloomFilter(32), "6789012345", 5 },
                new object[] { GetEmptyBloomFilter(32), "7890123456", 27 },
                new object[] { GetEmptyBloomFilter(32), "8901234567", 5 },
                new object[] { GetEmptyBloomFilter(32), "9012345678", 27 },
            };
    }
}
