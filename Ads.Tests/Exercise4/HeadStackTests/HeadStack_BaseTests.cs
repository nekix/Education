extern alias Exercise4;

using Exercise4.AlgorithmsDataStructures;
using AlgorithmsDataStructures;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise4.HeadStackTests
{
    public class HeadStack_BaseTests
    {
        protected static HeadStack<T> GetTestHeadStack<T>(T[] array)
        {
            var stack = new HeadStack<T>();

            foreach (var item in array)
                stack.InnerList.AddFirst(item);

            return stack;
        }
    }
}
