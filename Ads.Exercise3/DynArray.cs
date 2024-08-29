using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures
{

    public class DynArray<T>
    {
        public T[] array;
        public int count;
        public int capacity;

        public DynArray()
        {
            count = 0;
            MakeArray(16);
        }

        public void MakeArray(int new_capacity)
        {
            // ваш код
        }

        public T GetItem(int index)
        {
            // ваш код
            return default(T);
        }

        public void Append(T itm)
        {
            // ваш код
        }

        public void Insert(T itm, int index)
        {
            // ваш код
        }

        public void Remove(int index)
        {
            // ваш код
        }

    }
}