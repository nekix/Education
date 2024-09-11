extern alias Exercise3;

using Exercise3.Ads.Exercise3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_3.MultidimDynArrayV3_Tests
{
    public class MultidimDynArrayV3_Insert_Tests : MultidimDynArrayV3_BaseTests
    {
        [Theory]
        [MemberData(nameof(InsertWithoutReallocationData))]
        public void Should_Insert_Without_Reallocation(int value, MultidimDynArrayV3<int> array, params int[] indexes)
        {
            array.Insert(value, indexes);

            array.GetItem(indexes).ShouldBe(value);
            array.GetCapacity(indexes.Length - 1).ShouldBe(16);
            array.GetCount(indexes.Take(indexes.Length - 1).ToArray()).ShouldBe(1);
        }

        [Fact]
        public void Should_Insert_With_Reallocation()
        {
            var array = GetEmptyMultidimDynArrayV3<int>(4);

            int count = 35;

            for (int i = 0; i < count; i++)
            for (int j = 0; j < count; j++)
            for (int k = 0; k < count; k++)
            for (int l = 0; l < count; l++)
                array.Insert(i + j + k + l  + 1, new int[] { i, j, k, l });

            for (int i = 0; i < count; i++)
            for (int j = 0; j < count; j++)
            for (int k = 0; k < count; k++)
            for (int l = 0; l < count; l++)
            {
                array.GetItem(new int[] { i, j, k, l })
                    .ShouldBe(i + j + k + l  + 1);

                array.GetCount(new int[] { i, j, k }).ShouldBe(count);
            }

            array.GetCapacity(0).ShouldBe(64);
            array.GetCapacity(1).ShouldBe(64);
            array.GetCapacity(2).ShouldBe(64);
            array.GetCapacity(3).ShouldBe(64);

            Assert.Throws<IndexOutOfRangeException>(() => array.GetItem(0, 0, 0, count + 200));
        }

        public static IEnumerable<object[]> InsertWithoutReallocationData =>
            new List<object[]>
            {
                // int index, int value, int[] sourceArray, DynArray<int> array
                new object[] { 0, GetEmptyMultidimDynArrayV3<int>(3), new int[] {0, 0, 0} },
                new object[] { 0, GetEmptyMultidimDynArrayV3<int>(5), new int[] {0, 0, 0, 0, 0} },
            };
    }
}
