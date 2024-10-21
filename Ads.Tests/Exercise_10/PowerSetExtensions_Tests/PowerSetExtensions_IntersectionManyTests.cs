extern alias Exercise10;

using Exercise10.Ads.Exercise10;
using Exercise10.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_10.PowerSetExtensions_Tests
{
    public class PowerSetExtensions_IntersectionManyTests : PowerSet_BaseTests
    {
        [Theory]
        [MemberData(nameof(IntersectionData))]
        public void Should_IntersectionMany(string[] resData, PowerSet<string> set1, params PowerSet<string>[] sets)
        {
            var resSet = set1.IntersectionMany(sets);

            resSet.Size().ShouldBe(resData.Length);
            foreach (var item in resData)
                resSet.Get(item).ShouldBeTrue();
        }

        public static IEnumerable<object[]> IntersectionData =>
            new List<object[]>
            {
                new object[]
                {
                    Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray(),
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                    GetPowerSet(Enumerable.Range(200000, 20000).Select(i => i.GetHashCode().ToString() + i).ToArray()),
                },
                new object[]
                {
                    Enumerable.Range(10000, 10000).Select(i => i.ToString()).ToArray(),
                    GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.ToString()).ToArray()),
                },
                new object[]
                {
                    Enumerable.Empty<string>().ToArray(),
                    GetPowerSet(Enumerable.Range(0, 10000).Select(i => i.ToString()).ToArray()),
                    GetPowerSet(Enumerable.Range(10000, 20000).Select(i => i.ToString()).ToArray()),
                },
                new object[]
                {
                    Enumerable.Range(4500, 2500).Select(i => i.ToString()).ToArray(),
                    GetPowerSet(Enumerable.Range(0, 10000).Select(i => i.ToString()).ToArray()),
                    new PowerSet<string>[]
                    {
                        GetPowerSet(Enumerable.Range(3000, 20000).Select(i => i.ToString()).ToArray()),
                        GetPowerSet(Enumerable.Range(1000, 6000).Select(i => i.ToString()).ToArray()),
                        GetPowerSet(Enumerable.Range(4000, 7000).Select(i => i.ToString()).ToArray()),
                        GetPowerSet(Enumerable.Range(4500, 3000).Select(i => i.ToString()).ToArray()),
                        GetPowerSet(Enumerable.Range(1500, 9000).Select(i => i.ToString()).ToArray()),
                    },
                },
            };
    }
}
