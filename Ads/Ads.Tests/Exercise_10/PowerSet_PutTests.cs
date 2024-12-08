extern alias Exercise10;

using Exercise10.AlgorithmsDataStructures;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_10
{
    public class PowerSet_PutTests : PowerSet_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(PutData))]
        public async Task Should_Put(PowerSet<string> powerSet, string[] data, int size)
        {
            await Task.Run(() =>
            {
                foreach (var str in data)
                    powerSet.Put(str);

                powerSet.Size().ShouldBe(size);
                foreach (var str in data)
                    powerSet.Get(str).ShouldBeTrue();
            });
        }

        public static IEnumerable<object[]> PutData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(), new string[] { "abc123", "321cba", "abba", "bbaa" }, 4 },
                new object[] { GetEmptyPowerSet<string>(), new string[] { "abc123", "321cba", "abc123", "321cba" }, 2 },
                new object[] { GetEmptyPowerSet<string>(), Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray(), 20000 },
            };
    }
}
