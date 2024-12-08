extern alias Exercise5;
using Exercise5.Ads.Exercise5;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_5.StackQueue
{
    public class StackQueue_BaseTests
    {
        protected static StackQueue<T> GetEmptyQueue<T>()
            => new StackQueue<T>();

        protected static StackQueue<T> GetFilledQueue<T>(T[] items)
        {
            var queue = GetEmptyQueue<T>();

            foreach (var item in items)
                queue.Enqueue(item);

            return queue;
        }
    }
}
