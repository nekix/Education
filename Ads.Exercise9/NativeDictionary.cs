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
            step = size - 1;
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
            var hash = HashFun(key);

            if (slots[hash] == key) return true;

            for (int i = (hash + step) % size; i != hash; i = (i + step) % size)
                if (slots[i] == key) return true;

            return false;
        }

        public void Put(string key, T value)
        {
            var slot = SeekSlot(value);

            if (slot != -1)
                slots[slot] = value;

            return slot;


            // гарантированно записываем 
            // значение value по ключу key
        }

        public T Get(string key)
        {
            // возвращает value для key, 
            // или null если ключ не найден
            return default(T);
        }

        /// <summary>
        /// Return index of slots[] with current key.
        /// Return -1 if key not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int FindSlotByKey(string key)
        {
            var hash = HashFun(key);

            if (slots[hash] == key) return hash;

            for (int i = (hash + step) % size; i != hash; i = (i + step) % size)
                if (slots[i] == key) return i;

            return -1;
        }
    }
}