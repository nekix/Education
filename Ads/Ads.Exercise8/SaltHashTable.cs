using System;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmsDataStructures
{

    public class SaltHashTable
    {
        public int size;
        public int step;
        public string[] slots;

        public int MaxCollisionDeep = 0;

        // Polynomial HashFunc parameters
        private int _p_p = 35515;

        private int _saltInit;

        private Random _random = new Random((int)(DateTime.Now.Millisecond % DateTime.Now.Ticks));

        public SaltHashTable(int sz, int stp)
        {
            size = sz;
            step = stp;
            slots = new string[size];
            for (int i = 0; i < size; i++) slots[i] = null;

            _saltInit = _random.Next();
        }

        public int HashFun(string value)
        {
            int hash = 0;

            for (int i = 0; i < value.Length; i++)
                hash = ((hash * _p_p) + value[i]) % size;

            return hash;
        }

        public int SeekSlot(string value)
        {
            var hash = HashFun(GetSalt(value) + value);

            if (slots[hash] == null) return hash;

            for (int i = (hash + step) % size, count = 1; i != hash; i = (i + step) % size, count++)
            {
                if (slots[i] == null)
                {
                    MaxCollisionDeep = MaxCollisionDeep < count
                        ? count
                        : MaxCollisionDeep;

                    return i;
                }
            }

            return -1;
        }

        public int Put(string value)
        {
            var slot = SeekSlot(value);

            if (slot != -1)
                slots[slot] = value;

            return slot;
        }

        public int Find(string value)
        {
            var hash = HashFun(GetSalt(value) + value);

            if (slots[hash] == value) return hash;

            for (int i = (hash + step) % size; i != hash; i = (i + step) % size)
                if (slots[i] == value) return i;

            return -1;
        }

        private string GetSalt(string value)
        {
            StringBuilder sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(value);

            for(int i = 0; i < bytes.Length; i++)
                sb.Append((char)((bytes[i] ^ _saltInit) % 57) + 65);

            return sb.ToString();
        }
    }

}