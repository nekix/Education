using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmsDataStructures2;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise_5
{
    public class BalancedBST_Tests
    {
        [Theory]
        [MemberData(nameof(GetGenerateBBSTArrayData))]
        public void Should_GenerateBBSTArray(int[] a, int[] res)
        {
            BalancedBST.GenerateBBSTArray(a).ShouldBe(res);
        }

        public static IEnumerable<object[]> GetGenerateBBSTArrayData()
        {
            int[] a = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            int[] res = new int[] { 7, 3, 11, 1, 5, 9, 13, 0, 2, 4, 6, 8, 10, 12, 14 };
            yield return new object[] { a, res };
        }
    }
}
