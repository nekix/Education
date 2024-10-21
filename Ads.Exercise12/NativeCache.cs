using System;
using System.Collections.Generic;
using System.Globalization;
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

        private int _step;

        private const int HashMultiplier = 35515;

        public NativeCache(int sz) : this(sz, 1)
        {

        }

        public NativeCache(int sz, int step)
        {
            size = sz;
            slots = new String[sz];
            values = new T[sz];
            hits = new int[sz];
            _step = step;
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

            if (slot < 0)
                slot = FindMinHitsSlot();

            slots[slot] = key;
            values[slot] = value;
            hits[slot] = 0;
        }

        public T Get(string key)
        {
            var slot = FindSlot(key, false);

            if(slot < 0)
                return default;

            hits[slot]++;
            return values[slot];
        }

        /// <summary>
        /// Return index of slots[] with current key.
        /// Return -1 if key not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="withEmpty">if set to <c>true</c> find empty or .</param>
        /// <returns></returns>
        private int FindSlot(string key, bool withEmpty)
        {
            var hash = HashFun(key);

            if (slots[hash] == key || (withEmpty && slots[hash] == null))
                return hash;

            for (int i = (hash + _step) % size;
                i != hash && (withEmpty || slots[i] != null);
                i = (i + _step) % size)
            {
                if (slots[i] == key || (withEmpty && slots[i] == null))
                    return i;
            }

            return -1;
        }

        private int FindMinHitsSlot()
        {
            if (hits[0] == 0)
                return 0;

            int minIndex = 0;

            for (int i = 1; i < size; i++)
            {
                if (hits[i] >= hits[minIndex])
                    continue;

                minIndex = i;

                if (hits[minIndex] == 0)
                    break;
            }

            return minIndex;
        }
    }
}
