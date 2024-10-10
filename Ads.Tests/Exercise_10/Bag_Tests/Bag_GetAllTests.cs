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
    public class Bag_GetAllTests : Bag_BaseTests
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Should_Get(Bag<string> powerSet, int count)
        {
            var res = powerSet.GetAll();
            res.Count.ShouldBe(count);
        }

        public static IEnumerable<object[]> GetData =>
            new List<object[]>
            {
                new object[] { GetEmptyBag<string>(), 0 },
                new object[] { GetBag(new string[] { "321", "123" }), 2 },
                new object[] { GetBag(new string[] { "321", "123", "123" }), 2 },
            };
    }
}
