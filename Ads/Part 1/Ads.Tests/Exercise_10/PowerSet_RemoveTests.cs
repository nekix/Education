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
    public class PowerSet_RemoveTests : PowerSet_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(RemoveData))]
        public async Task Should_Remove(PowerSet<string> powerSet, string item, bool result, int size)
        {
            await Task.Run(() =>
            {
                powerSet.Remove(item).ShouldBe(result);
                powerSet.Size().ShouldBe(size);
            });
        }

        public static IEnumerable<object[]> RemoveData =>
            new List<object[]>
            {
                new object[] { GetEmptyPowerSet<string>(),"123", false, 0 },
                new object[] { GetPowerSet(new string[] { "321", "123" }), "123", true, 1 },
                new object[] { GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "3556", true, 19999 },
                new object[] { GetPowerSet(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "22000", false, 20000 },
            };
    }
}
