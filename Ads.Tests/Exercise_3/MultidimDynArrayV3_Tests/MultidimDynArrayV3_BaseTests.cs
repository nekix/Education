extern alias Exercise3;

using Exercise3.Ads.Exercise3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_3.MultidimDynArrayV3_Tests
{
    public class MultidimDynArrayV3_BaseTests
    {
        protected static MultidimDynArrayV3<T> GetEmptyMultidimDynArrayV3<T>(int dimensionsCount)
        {
            var dynArray = new MultidimDynArrayV3<T>(dimensionsCount);

            return dynArray;
        }
    }
}
