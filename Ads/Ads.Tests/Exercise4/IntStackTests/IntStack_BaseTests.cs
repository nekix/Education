extern alias Exercise4;

using Exercise4.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise4.IntStackTests
{
    public class IntStack_BaseTests
    {
        protected static IntStack GetTestIntStack(int[] array)
        {
            var stack = new IntStack();

            foreach (var item in array)
                stack.Push(item);

            return stack;
        }
    }
}
