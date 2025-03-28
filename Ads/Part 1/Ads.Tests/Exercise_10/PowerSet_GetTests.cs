extern alias Exercise10;

using Exercise10.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_10
{
    public class PowerSet_GetTests : PowerSet_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(GetData))]
        public async Task Should_Get(PowerSet<string> powerSet, string data, bool result)
        {
            await Task.Run(() => powerSet.Get(data).ShouldBe(result));
        }

        public static IEnumerable<object[]> GetData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(),"123", false },
                new object[] { GetPowerSet(new string[] { "321", "123" }), "123", true },
                new object[] { GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "3556", true },
                new object[] { GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "22000", false },
            };
    }
}
