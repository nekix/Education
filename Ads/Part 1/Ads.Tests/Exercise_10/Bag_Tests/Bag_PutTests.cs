extern alias Exercise10;
using Exercise10.Ads.Exercise10;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_10.Bag_Tests
{
    public class Bag_PutTests : Bag_BaseTests
    {
        [Theory(Timeout = 1000)]
        [MemberData(nameof(PutData))]
        public async Task Should_Put(Bag<string> powerSet, string[] data, int size)
        {
            await Task.Run(() =>
            {
                foreach (var str in data)
                    powerSet.Put(str);

                powerSet.Size().ShouldBe(size);
                foreach (var str in data)
                    powerSet.Get(str).ShouldBeTrue();
            });
        }

        public static IEnumerable<object[]> PutData =>
            new List<object[]>
            {
                new object[] { GetEmptyBag<string>(), new string[] { "abc123", "321cba", "abba", "bbaa" }, 4 },
                new object[] { GetEmptyBag<string>(), new string[] { "abc123", "321cba", "abc123", "321cba" }, 4 },
                new object[] { GetEmptyBag<string>(), Enumerable.Range(0, 20000).Select(i => i.ToString()).ToArray(), 20000 },
            };
    }
}
