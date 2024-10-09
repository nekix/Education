using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmsDataStructures
{
    // наследуйте этот класс от HashTable
    // или расширьте его методами из HashTable
    public class PowerSet<T>
    {
        private const int _defaultCapacity = 20000;

        private List<T>[] _slots;
        private int _count;

        private IEqualityComparer<T> _comparer;

        public PowerSet() : this(_defaultCapacity)
        {

        }

        public PowerSet(int capacity)
        {
            _slots = new List<T>[capacity];
            _comparer = EqualityComparer<T>.Default;
        }

        public int Size()
            => _count;

        public void Put(T value)
        {
            if (_count == _slots.Length) return;

            int slot = FindSlot(value);
            if (_slots[slot] == null)
            {
                _slots[slot] = new List<T>(1);
            }
            else
            {
                if (_slots[slot].Contains(value, _comparer))
                    return;
            }

            _slots[slot].Add(value);
            _count++;
        }

        public bool Get(T value)
        {
            int slot = FindSlot(value);
            int entryIndex = FindValueEntryIndex(value, slot);

            return slot != -1;
        }

        public bool Remove(T value)
        {
            if(_count == 0) return false;

            int slot = FindSlot(value);
            int entryIndex = FindValueEntryIndex(value, slot);

            if (entryIndex == -1)
                return false;

            _slots[slot].Remove(value);

            return true;
        }

        public PowerSet<T> Intersection(PowerSet<T> set2)
        {
            PowerSet<T> minSet;
            PowerSet<T> maxSet;

            if (_count >= set2._count)
            {
                minSet = set2;
                maxSet = this;
            }
            else
            {
                minSet = this;
                maxSet = set2;
            }

            PowerSet<T> resultSet = new PowerSet<T>(minSet._count, minSet._step);

            foreach (T item in minSet._slots)
            {
                int hash = HashFun(item);

                if (maxSet.FindValueSlot(hash, item) != -1)
                    resultSet.Put(item);
            }

            return resultSet;
        }

        public PowerSet<T> Union(PowerSet<T> set2)
        {
            // объединение текущего множества и set2
            return null;
        }

        public PowerSet<T> Difference(PowerSet<T> set2)
        {
            // разница текущего множества и set2
            return null;
        }

        public bool IsSubset(PowerSet<T> set2)
        {
            // возвращает true, если set2 есть
            // подмножество текущего множества,
            // иначе false
            return false;
        }

        public bool Equals(PowerSet<T> set2)
        {
            if(_count != set2._count) return false;

            foreach (T item in _slots)
            {
                if (_comparer.Equals(item, default))
                    continue;

                int hash = HashFun(item);
                if (set2.FindValueSlot(hash, item) == -1)
                    return false;
            }

            return true;
        }

        private int HashFun(T value)
        {
            if (value == null)
                return 0;

            return Math.Abs(_comparer.GetHashCode(value));
        }


        private int FindSlot(T value)
            => HashFun(value) % _slots.Length;

        /// <summary>
        /// Finds the index of the value entry.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="slotIndex">Index of the slot.</param>
        /// <returns>index in List in _slots[slotIndex]. If not exist return -1.</returns>
        private int FindValueEntryIndex(T value, int slotIndex)
        {
            if (_slots[slotIndex] == null) return -1;

            return _slots[slotIndex].IndexOf(value);
        }
    }
}