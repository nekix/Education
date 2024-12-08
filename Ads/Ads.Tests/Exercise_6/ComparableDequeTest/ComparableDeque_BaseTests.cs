extern alias Exercise6;

using Exercise6.Ads.Exercise6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_6.ComparableDequeTest
{
    public class ComparableDeque_BaseTests
    {
        protected static ComparableDeque<T> GetEmptyDeque<T>() where T: IComparable<T>
            => new ComparableDeque<T>();

        protected static ComparableDeque<T> GetFilledDeque<T>(T[] items) where T : IComparable<T>
        {
            var queue = GetEmptyDeque<T>();

            foreach (var item in items)
                queue.AddFront(item);

            return queue;
        }
    }
}
