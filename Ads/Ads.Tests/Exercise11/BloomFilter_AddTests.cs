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
    public class BloomFilter_AddTests : BloomFilter_BaseTests
    {
        [Theory]
        [MemberData(nameof(Hash1FunData))]
        public void Should_Add(BloomFilter bloomFilter, string data)
        {
            bloomFilter.Add(data);

            bloomFilter.IsValue(data).ShouldBeTrue();
        }

        public static IEnumerable<object[]> Hash1FunData =>
            new List<object[]>
            {
                new object[] { GetEmptyBloomFilter(32), "0123456789" },
                new object[] { GetEmptyBloomFilter(32), "1234567890" },
                new object[] { GetEmptyBloomFilter(32), "2345678901" },
                new object[] { GetEmptyBloomFilter(32), "3456789012" },
                new object[] { GetEmptyBloomFilter(32), "4567890123" },
                new object[] { GetEmptyBloomFilter(32), "5678901234" },
                new object[] { GetEmptyBloomFilter(32), "6789012345" },
                new object[] { GetEmptyBloomFilter(32), "7890123456" },
                new object[] { GetEmptyBloomFilter(32), "8901234567" },
                new object[] { GetEmptyBloomFilter(32), "9012345678" },
            };
    }
}
