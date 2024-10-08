﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmsDataStructures
{

    public class HashTable
    {
        public int size;
        public int step;
        public string[] slots;
        public int MaxCollisionDeep = 0;

        public HashTable(int sz, int stp)
        {
            size = sz;
            step = stp;
            slots = new string[size];
            for (int i = 0; i < size; i++) slots[i] = null;
        }

        public int HashFun(string value)
        {
            int sum = 0;

            foreach (var bt in Encoding.Unicode.GetBytes(value))
                sum += bt;

            return sum % size;
        }

        public int SeekSlot(string value)
        {
            var hash = HashFun(value);

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
            var hash = HashFun(value);

            if (slots[hash] == value) return hash;

            for (int i = (hash + step) % size; i != hash; i = (i + step) % size)
                if (slots[i] == value) return i;

            return -1;
        }
    }

}