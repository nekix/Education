using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures
{

    public class NativeDictionary<T>
    {
        public int size;
        public string[] slots;
        public T[] values;


        private int step;
        private int _p_p = 35515;

        public NativeDictionary(int sz)
        {
            size = sz;
            step = 1;
            slots = new string[size];
            values = new T[size];
        }

        public int HashFun(string key)
        {
            int hash = 0;

            foreach (var ch in key)
                hash = ((hash * _p_p) + ch) % size;

            return hash;
        }

        public bool IsKey(string key)
        {
            var slot = FindSlot(key, false);

            return slot >= 0;
        }

        public void Put(string key, T value)
        {
            var slot = FindSlot(key, true);

            if (slot < 0) return;

            slots[slot] = key;
            values[slot] = value;
        }

        public T Get(string key)
        {
            var slot = FindSlot(key, false);

            return slot >= 0 ? values[slot] : default;
        }

        /// <summary>
        /// Return index of slots[] with current key.
        /// Return -1 if key not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="withEmpty">if set to <c>true</c> [with empty].</param>
        /// <returns></returns>
        private int FindSlot(string key, bool withEmpty)
        {
            var hash = HashFun(key);

            if (slots[hash] == key || (withEmpty && slots[hash] == null))
                return hash;

            for (int i = (hash + step) % size; i != hash && (withEmpty || slots[i] != null); i = (i + step) % size)
            {
                if (slots[i] == key || (withEmpty && slots[i] == null))
                    return i;
            }
                
            return -1;
        }
    }
}