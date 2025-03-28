extern alias Exercise10;
using Exercise10.Ads.Exercise10;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_10.Bag_Tests
{
    public abstract class Bag_BaseTests
    {
        protected static Bag<T> GetEmptyBag<T>()
            => new Bag<T>();

        protected static Bag<T> GetEmptyBag<T>(int capacity)
            => new Bag<T>(capacity);

        protected static Bag<T> GetBag<T>(T[] data)
        {
            var ps = GetEmptyBag<T>();

            foreach (var item in data)
                ps.Put(item);

            return ps;
        }
    }
}
