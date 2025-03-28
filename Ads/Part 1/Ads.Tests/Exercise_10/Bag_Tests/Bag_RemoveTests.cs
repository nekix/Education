extern alias Exercise10;

using Exercise10.Ads.Exercise10;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_10.Bag_Tests
{
    public class Bag_RemoveTests : Bag_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(RemoveData))]
        public async Task Should_Remove(Bag<string> powerSet, string item, bool result, int size)
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
                new object[] { GetEmptyBag<string>(),"123", false, 0 },
                new object[] { GetBag(new string[] { "321", "123", "123" }), "123", true, 2 },
                new object[] { GetBag(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "3556", true, 19999 },
                new object[] { GetBag(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "22000", false, 20000 },
            };
    }
}
