extern alias Exercise3;

using Exercise3.Ads.Exercise3;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_3.MultidimDynArrayV3_Tests
{
    public class MultidimDynArrayV3_Remove_Tests : MultidimDynArrayV3_BaseTests
    {
        [Fact]
        public void Should_Remove_With_Reallocation()
        {
            var array = GetEmptyMultidimDynArrayV3<int>(4);

            int count = 35;

            Should.Throw<IndexOutOfRangeException>(() => array.Insert(-1, 0, 0, 0, 2));

            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                    for (int k = 0; k < count; k++)
                        for (int l = 0; l < count; l++)
                            array.Insert(i + j + k + l + 1, new int[] { i, j, k, l });

            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                    for (int k = 0; k < count; k++)
                        for (int l = 0; l < count; l++)
                        {
                            array.Remove(i, j, k, 0);
                        }

            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                    for (int k = 0; k < count; k++)
                        array.GetCount(i, j, k).ShouldBe(0);

        }
    }
}
