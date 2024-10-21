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
    public class PowerSet_UnionTests : PowerSet_BaseTests
    {
        [Theory]
        [MemberData(nameof(UnionData))]
        public void Should_Union(PowerSet<string> set1, PowerSet<string> set2, string[] resultData)
        {
            var resultSet = set1.Union(set2);

            resultSet.Size().ShouldBe(resultData.Length);
            foreach (var item in resultData)
                resultSet.Get(item).ShouldBe(true);
        }

        [Theory(Timeout = 1000)]
        [MemberData(nameof(UnionData))]
        public void Should_UnionFast(PowerSet<string> set1, PowerSet<string> set2, string[] resultData)
        {
            set1.Union(set2).Size().ShouldBe(resultData.Length);
        }

        public static IEnumerable<object[]> UnionData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(), GetEmptyPowerSet<string>(), Array.Empty<string>() },
                new object[] { GetPowerSet(new string[] { "321", "123" }), GetEmptyPowerSet<string>(), new string[] { "321", "123" } },
                new object[] { GetEmptyPowerSet<string>(), GetPowerSet(new string[] { "321", "123" }), new string[] { "321", "123" } },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321" }), new string[] { "123", "321" } },
                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321", "456" }), new string[] { "123", "321", "456" } },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "456", "789" }), new string[] { "123", "321", "456", "789" } },

                new object[]
                {
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(20000, 20000).Select(i => i.ToString()).ToArray()),
                    Enumerable.Range(0, 40000).Select(i => i.ToString()).ToArray()
                },
            };
    }
}
