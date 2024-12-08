extern alias Exercise10;
using Exercise10.Ads.Exercise10;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_10.Bag_Tests
{
    public class Bag_GetTests : Bag_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(GetData))]
        public async Task Should_Get(Bag<string> powerSet, string data, bool result)
        {
            await Task.Run(() => powerSet.Get(data).ShouldBe(result));
        }

        public static IEnumerable<object[]> GetData =>
            new List<object[]>
            {
                new object[] { GetEmptyBag<string>(),"123", false },
                new object[] { GetBag(new string[] { "321", "123" }), "123", true },
                new object[] { GetBag(new string[] { "321", "123", "123" }), "123", true },
                new object[] { GetBag(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "3556", true },
                new object[] { GetBag(Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray()), "22000", false },
            };
    }
}
