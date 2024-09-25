extern alias Exercise8;
using Exercise8.AlgorithmsDataStructures;
using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_8
{
    public class HashTable_BaseTests
    {
        protected static HashTable GetEmptyHashTable(int sz = 17, int stp = 3)
            => new HashTable(sz, stp);
    }
}
