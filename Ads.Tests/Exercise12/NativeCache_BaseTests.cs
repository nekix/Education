extern alias Exercise12;

using Exercise12.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise12
{
    public abstract class NativeCache_BaseTests
    {
        protected static NativeCache<T> GetEmptyNativeCache<T>(int sz, int stp = 1)
            => new NativeCache<T>(sz, stp);
    }
}
