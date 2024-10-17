using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsDataStructures
{
    public class NativeCache<T>
    {
        public int size;
        public String[] slots;
        public T[] values;
        public int[] hits;

        private const int HashMultiplier = 35515;

        public NativeCache(int sz)
        {
            size = sz;
            slots = new String[sz];
            values = new T[sz];
            hits = new int[sz];
        }

        public int HashFun(string key)
        {
            int hash = 0;

            foreach (var ch in key)
                hash = ((hash * HashMultiplier) + ch) % size;

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
