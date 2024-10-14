extern alias Exercise11;

using Exercise11.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise11
{
    public class BloomFilter_BaseTests
    {
        protected static BloomFilter GetEmptyBloomFilter(int length)
            => new BloomFilter(length);

        protected static BloomFilter GetFulledBloomFilter(int length, IEnumerable<string> bloomFilter)
        {
            BloomFilter filter = GetEmptyBloomFilter(length);

            foreach (var item in bloomFilter)
                filter.Add(item);

            return filter;
        }
    }
}
