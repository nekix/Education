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
    public class BloomFilter_UnionTests : BloomFilter_BaseTests
    {
        [Theory]
        [MemberData(nameof(UnionData))]
        public void Should_Union(BloomFilter bloomFilter, IEnumerable<BloomFilter> filters, IEnumerable<string> data)
        {
            var resultFilter = bloomFilter.Union(filters);

            foreach (var item in data)
                resultFilter.IsValue(item).ShouldBeTrue();
        }

        public static IEnumerable<object[]> UnionData =>
            new List<object[]>
            {
                new object[] { GetFulledBloomFilter(
                    32,
                    GetStrings(48, 3, 10)),
                    Enumerable.Range(51, 3).Select(i => GetFulledBloomFilter(32, GetStrings(i, 3, 10))),
                    GetStrings(48, 6, 10),
                }
            };

        private static IEnumerable<string> GetStrings(int start, int count, int length)
        {
            return Enumerable.Range(start, count)
                    .Select(i => Enumerable.Range(i, length).Select(j => (char)(j + i)))
                    .Select(i => new string(i.ToArray()));
        }
    }
}
