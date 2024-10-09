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
    public class PowerSet_IntersectionTests : PowerSet_BaseTests
    {
        [Theory]
        [MemberData(nameof(EqualsData))]
        public void Should_Intersection(PowerSet<string> set1, PowerSet<string> set2, string[] resultData)
        {
            Dictionary<>

            var resultSet = set1.Intersection(set2);

            resultSet.Size().ShouldBe(resultData.Length);
            foreach (var item in resultData)
                resultSet.Get(item).ShouldBe(true);
        }

        [Theory(Timeout = 1500)]
        [MemberData(nameof(EqualsData))]
        public async Task Should_IntersectionFast(PowerSet<string> set1, PowerSet<string> set2, string[] resultData)
        {
             await Task.Run(() => set1.Intersection(set2).Size().ShouldBe(resultData.Length));
        }

        public static IEnumerable<object[]> EqualsData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(), GetEmptyPowerSet<string>(), Array.Empty<string>() },
                new object[] { GetPowerSet(new string[] { "321", "123" }), GetEmptyPowerSet<string>(), Array.Empty<string>() },
                new object[] { GetEmptyPowerSet<string>(), GetPowerSet(new string[] { "321", "123" }), Array.Empty<string>() },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321" }), new string[] { "123", "321" } },
                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321", "456" }), new string[] { "123", "321" } },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    Enumerable.Range(10000, 10000).Select(i => i.GetHashCode().ToString() + i).ToArray()
                },
            };
    }
}
