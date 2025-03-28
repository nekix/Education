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
    public class PowerSet_SizeTests : PowerSet_BaseTests
    {
        [Theory]
        [MemberData(nameof(SizeData))]
        public void Should_Size(PowerSet<string> powerSet, int size)
        {
            powerSet.Size().ShouldBe(size);
        }

        public static IEnumerable<object[]> SizeData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(), 0 },
                new object[] { GetPowerSet(new string[] { "321", "123" }), 2 },
                new object[] { GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "20000" },
            };
    }
}
