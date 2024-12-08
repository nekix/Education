extern alias Exercise8;
using Exercise8.AlgorithmsDataStructures;
using System.Drawing;

namespace Ads.Tests.Exercise_8
{
    public class HashTable_BaseTests
    {
        protected static HashTable GetEmptyHashTable(int sz, int stp)
            => new HashTable(sz, stp);
    }
}
