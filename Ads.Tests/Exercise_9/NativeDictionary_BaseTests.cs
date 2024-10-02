extern alias Exercise9;

using AlgorithmsDataStructures;
using Exercise9.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_9
{
    public class NativeDictionary_BaseTests
    {
        protected static NativeDictionary<T> GetEmptyNativeDictionary<T>(int sz)
            => new NativeDictionary<T>(sz);
    }
}
