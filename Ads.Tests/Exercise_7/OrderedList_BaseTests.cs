extern alias Exercise7;

using Exercise7.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_7
{
    public class OrderedList_BaseTests
    {
        protected static OrderedList<T> GetEmptyOrderedList<T>(bool asc)
            => new OrderedList<T>(asc);
    }
}
