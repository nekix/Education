extern alias Exercise5;

using Exercise5.Ads.Exercise5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_5.CircleQueue
{
    public class CircleQueue_BaseTests
    {
        protected static CircleQueue<T> GetEmptyCircleQueue<T>(int capacity)
             => new CircleQueue<T>(capacity);

        protected static CircleQueue<T> GetFilledCircleQueue<T>(int capacity, T[] items)
        {
            var queue = GetEmptyCircleQueue<T>(capacity);

            foreach (var item in items)
                queue.Enqueue(item);

            return queue;
        }
    }
}
