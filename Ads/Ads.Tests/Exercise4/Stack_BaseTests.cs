extern alias Exercise4;

using Exercise4.AlgorithmsDataStructures;
using AlgorithmsDataStructures;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise4
{
    public class Stack_BaseTests
    {
        protected static Stack<T> GetTestStack<T>(T[] array)
        {
            var stack = new Stack<T>();

            stack.InnerList.AddRange(array);

            return stack;
        }
    }
}
