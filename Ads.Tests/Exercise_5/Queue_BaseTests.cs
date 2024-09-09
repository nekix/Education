extern alias Exercise5;

using Exercise5.AlgorithmsDataStructures;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_5
{
    public class Queue_BaseTests
    {
        protected static Queue<T> GetEmptyQueue<T>()
            => new Queue<T>();

        protected static Queue<T> GetFilledQueue<T>(T[] items)
        {
            var queue = GetEmptyQueue<T>();

            foreach (var item in items)
                queue.Enqueue(item);

            return queue;
        }
    }
}
