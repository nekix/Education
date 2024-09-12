extern alias Exercise6;

using Exercise6.Ads.Exercise6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_6.ImprovedDeque_Tests
{
    public class ImprovedDeque_BaseTests
    {
        protected static ImprovedDeque<T> GetEmptyDeque<T>()
            => new ImprovedDeque<T>();

        protected static ImprovedDeque<T> GetFilledDeque<T>(T[] items)
        {
            var queue = GetEmptyDeque<T>();

            foreach (var item in items)
                queue.AddFront(item);

            return queue;
        }
    }
}
