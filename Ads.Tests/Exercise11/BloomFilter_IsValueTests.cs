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
    public class BloomFilter_IsValueTests : BloomFilter_BaseTests
    {
        [Theory]
        [MemberData(nameof(AddData))]
        public void Should_Add(BloomFilter bloomFilter, IEnumerable<string> data)
        {
            foreach (var item in data)
                bloomFilter.IsValue(item).ShouldBeTrue();
        }

        public static IEnumerable<object[]> AddData =>
            new List<object[]>
            {
                new object[] { GetFulledBloomFilter(
                    32,
                    Enumerable.Range(0, 10)
                    .Select(i => Enumerable.Range(i, 10).Select(j => (char)((j + 48) % 58)))
                    .Select(i => new string(i.ToArray()))
                    ),
                    Enumerable.Range(0, 10)
                    .Select(i => Enumerable.Range(i, 10).Select(j => (char)((j + 48) % 58)))
                    .Select(i => new string(i.ToArray()))
                },
            };
    }
}
