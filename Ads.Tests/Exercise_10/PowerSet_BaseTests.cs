extern alias Exercise10;

using Exercise10.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_10
{
    public abstract class PowerSet_BaseTests
    {
        protected static PowerSet<T> GetEmptyPowerSet<T>()
            => new PowerSet<T>();

        protected static PowerSet<T> GetEmptyPowerSet<T>(int capacity)
            => new PowerSet<T>(capacity);

        protected static PowerSet<T> GetPowerSet<T>(T[] data)
        {
            var ps = GetEmptyPowerSet<T>();
            
            foreach (var item in data)
                ps.Put(item);

            return ps;
        }
    }
}
