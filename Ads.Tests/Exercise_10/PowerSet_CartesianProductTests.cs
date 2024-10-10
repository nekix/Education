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
    public class PowerSet_CartesianProductTests : PowerSet_BaseTests
    {
        [Theory]
        [MemberData(nameof(DifferenceData))]
        public void Should_CartesianProduct(PowerSet<string> set1, PowerSet<string> set2, (string, string)[] resultData)
        {
            var resSet = set1.CartesianProduct(set2);

            resSet.Size().ShouldBe(resultData.Length);
            foreach (var item in resultData)
                resSet.Get(item).ShouldBeTrue();
        }

        [Theory(Timeout = 1000)]
        [MemberData(nameof(DifferenceData))]
        public async Task Should_CartesianProductFast(PowerSet<string> set1, PowerSet<string> set2, (string, string)[] resultData)
        {
            await Task.Run(() => set1.CartesianProduct(set2).Size().ShouldBe(resultData.Length));
        }

        public static IEnumerable<object[]> DifferenceData =>
           new List<object[]>
           {
                new object[] { GetEmptyPowerSet<string>(), GetEmptyPowerSet<string>(), Array.Empty<(string, string)>() },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetEmptyPowerSet<string>(), Array.Empty<(string, string)>() },
                new object[] { GetEmptyPowerSet<string>(), GetPowerSet(new string[] { "321", "123" }), Array.Empty<(string, string)>() },

                new object[] { GetPowerSet(new string[] { "321", "123" }), GetPowerSet(new string[] { "123", "321" }), GetLinqCartesian(new string[] { "123", "321" }, new string[] { "123", "321" }) },

                new object[]
                {
                    GetPowerSet(new string[] { "111", "123" }),
                    GetPowerSet(new string[] { "222", "321", "456" }),
                    GetLinqCartesian(new string[] { "111", "123" }, new string[] { "222", "321", "456" })
                },
                new object[]
                {
                    GetPowerSet(new string[] { "321", "123" }),
                    GetPowerSet(new string[] { "123", "321", "456" }),
                    GetLinqCartesian(new string[] { "321", "123" }, new string[] { "123", "321", "456" })
                },
                new object[]
                {
                    GetPowerSet(new string[] { "123", "321", "456" }),
                    GetPowerSet(new string[] { "321", "123" }),
                    GetLinqCartesian(new string[] { "123", "321", "456" }, new string[] { "321", "123" })
                },

                new object[]
                {
                    GetPowerSet(Enumerable.Range(500, 500).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(500, 500).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetLinqCartesian(Enumerable.Range(500, 500).Select(i => i.GetHashCode().ToString() + i), Enumerable.Range(500, 500).Select(i => i.GetHashCode().ToString() + i))
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 200).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 200).Select(i => i.ToString()).ToArray()),
                    GetLinqCartesian(Enumerable.Range(0, 200).Select(i => i.ToString()), Enumerable.Range(10000, 200).Select(i => i.ToString()))
                },
                new object[]
                {
                    GetPowerSet(Enumerable.Range(0, 100).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 200).Select(i => i.ToString()).ToArray()),
                    GetLinqCartesian(Enumerable.Range(0, 100).Select(i => i.ToString()), Enumerable.Range(10000, 200).Select(i => i.ToString()))
                },
           };

        private static (string, string)[] GetLinqCartesian(IEnumerable<string> first, IEnumerable<string> second)
        {
            return first.SelectMany(x => second, (x, y) => (x, y))
                .Distinct(PowerSet<string>.GetOrderedPairComparer())
                .ToArray();
        }
    }
}
