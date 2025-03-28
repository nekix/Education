extern alias Exercise10;

using Exercise10.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_10
{
    public class PowerSet_IsSubsetTests : PowerSet_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(SubsetData))]
        public async Task Should_IsSubset(PowerSet<string> set1, PowerSet<string> set2, bool result)
        {
            await Task.Run(() => set1.IsSubset(set2).ShouldBe(result));
        }

        public static IEnumerable<object[]> SubsetData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(), GetEmptyPowerSet<string>(), true },
                new object[] { GetPowerSet(new string[] { "321", "123" }), GetEmptyPowerSet<string>(), true },
                new object[] { GetEmptyPowerSet<string>(), GetPowerSet(new string[] { "321", "123" }), false },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321" }), true },
                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321", "456" }), false },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    true
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.ToString()).ToArray()),
                    false
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 10000).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.ToString()).ToArray()),
                    false
                },
            };
    }
}
