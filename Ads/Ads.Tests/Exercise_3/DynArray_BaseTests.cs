extern alias Exercise3;

using Exercise3.AlgorithmsDataStructures;
using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_3
{
    public class DynArray_BaseTests
    {
        protected static DynArray<T> GetTestDynArray<T>(int capacity, T[] array)
        {
            var dynArray = new DynArray<T>();

            T[] newArray = new T[capacity];
            
            Array.Copy(array, newArray, array.Length);

            dynArray.array = newArray;
            dynArray.capacity = capacity;
            dynArray.count = array.Length;

            return dynArray;
        }
    }
}
