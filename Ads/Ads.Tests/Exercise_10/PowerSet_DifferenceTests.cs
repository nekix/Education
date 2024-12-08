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
    public class PowerSet_DifferenceTests : PowerSet_BaseTests
    {
        [Theory]
        [MemberData(nameof(DifferenceData))]
        public void Should_Difference(PowerSet<string> set1, PowerSet<string> set2, string[] resultData)
        {
            var set = set1.Difference(set2);

            set.Size().ShouldBe(resultData.Length);
            foreach (var item in resultData)
                set.Get(item).ShouldBeTrue();
        }

        [Theory(Timeout = 1000)]
        [MemberData(nameof(DifferenceData))]
        public async Task Should_DifferenceFast(PowerSet<string> set1, PowerSet<string> set2, string[] resultData)
        {
            await Task.Run(() => set1.Difference(set2).Size().ShouldBe(resultData.Length));
        }

        public static IEnumerable<object[]> DifferenceData =>
           new List<object[]>
           {
                new object[] { GetEmptyPowerSet<string>(), GetEmptyPowerSet<string>(), Array.Empty<string>() },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetEmptyPowerSet<string>(), new string[] { "321", "123" } },
                new object[] { GetEmptyPowerSet<string>(), GetPowerSet(new string[] { "321", "123" }), Array.Empty<string>() },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321" }), Array.Empty<string>() },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321", "456" }), Array.Empty<string>() },
                new object[] { GetPowerSet(new string[] { "123", "321", "456" }), GetPowerSet(new string[] { "321", "123" }), new string[] { "456" } },

                new object[]
                {
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    Array.Empty<string>()
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.ToString()).ToArray()),
                    Enumerable.Range(0, 10000).Select(i => i.ToString()).ToArray()
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 10000).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.ToString()).ToArray()),
                    Enumerable.Range(0, 10000).Select(i => i.ToString()).ToArray()
                },
           };
    }
}
