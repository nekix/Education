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
    public class PowerSet_EqualsTests : PowerSet_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(EqualsData))]
        public async Task Should_Equals(PowerSet<string> set1, PowerSet<string> set2, bool res)
        {
            await Task.Run(() => set1.Equals(set2).ShouldBe(res));
        }

        public static IEnumerable<object[]> EqualsData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(), GetEmptyPowerSet<string>(), true },
                new object[] { GetPowerSet(new string[] { "321", "123" }), GetEmptyPowerSet<string>(), false },
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
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(9999, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    false
                }
            };
    }
}
