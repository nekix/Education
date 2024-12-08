extern alias Exercise6;

using Exercise6.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_6
{
    public class Deque_BaseTests
    {
        protected static Deque<T> GetEmptyDeque<T>()
            => new Deque<T>();

        protected static Deque<T> GetFilledDeque<T>(T[] items)
        {
            var queue = GetEmptyDeque<T>();

            foreach (var item in items)
                queue.AddFront(item);

            return queue;
        }
    }
}
