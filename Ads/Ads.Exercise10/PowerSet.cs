﻿using System;
using System.Collections.Generic;

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

        public PowerSet(IEqualityComparer<T> comparer) : this(_defaultCapacity, comparer)
        {

        }

        public PowerSet(int capacity, IEqualityComparer<T> comparer = null)
        {
            _slots = new List<T>[capacity];
            _comparer = comparer ?? EqualityComparer<T>.Default;
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
            else if (FindValueEntryIndex(value, slot) != -1)
            {
                return;
            }

            _slots[slot].Add(value);
            _count++;
        }

        public bool Get(T value)
        {
            int slot = FindSlot(value);
            int entryIndex = FindValueEntryIndex(value, slot);

            return entryIndex != -1;
        }

        public bool Remove(T value)
        {
            if(_count == 0) return false;

            int slot = FindSlot(value);
            int entryIndex = FindValueEntryIndex(value, slot);

            if (entryIndex != -1 && _slots[slot].Remove(value))
            {
                _count--;
                return true;
            }
            
            return false;
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

            PowerSet<T> resultSet = new PowerSet<T>(minSet._count);

            foreach (var slot in minSet._slots)
            {
                if (slot == null) continue;

                foreach (T item in slot)
                    if (maxSet.Get(item))
                        resultSet.Put(item);
            }

            return resultSet;
        }

        public PowerSet<T> Union(PowerSet<T> set2)
        {
            PowerSet<T> resultSet = new PowerSet<T>(_count + set2._count);

            foreach (List<T> slot in _slots)
            {
                if (slot == null) continue;

                foreach (T item in slot)
                    resultSet.Put(item);
            }

            foreach (List<T> slot in set2._slots)
            {
                if (slot == null) continue;

                foreach (T item in slot)
                    resultSet.Put(item);
            }

            return resultSet;
        }

        public PowerSet<T> Difference(PowerSet<T> set2)
        {
            PowerSet<T> resultSet = new PowerSet<T>(_count);

            foreach (var slot in _slots)
            {
                if (slot == null) continue;

                foreach (T item in slot)
                    if (!set2.Get(item))
                        resultSet.Put(item);
            }

           return resultSet;
        }

        public bool IsSubset(PowerSet<T> set2)
        {
            if (set2._count > _count) return false;

            foreach (var slot in set2._slots)
            {
                if(slot == null) continue;

                foreach (T item in slot)
                    if (!Get(item))
                        return false;
            }

            return true;
        }

        public bool Equals(PowerSet<T> set2)
        {
            if(_count != set2._count) return false;

            for(int slot = 0; slot < _slots.Length; slot++)
            {
                if (_slots[slot] == null) continue;

                foreach (T item in _slots[slot])
                {
                    if (set2.FindValueEntryIndex(item, slot) == -1)
                        return false;
                }
            }

            return true;
        }

        public PowerSet<(T, T)> CartesianProduct(PowerSet<T> set2)
        {
            PowerSet<(T, T)> resultSet = new PowerSet<(T, T)>(_count * set2._count, GetOrderedPairComparer());

            foreach (List<T> firstSlot in _slots)
            {
                if(firstSlot == null) continue;

                foreach (List<T> secondSlot in set2._slots)
                {
                    if (secondSlot == null) continue;

                    foreach (T firstItem in firstSlot)
                        foreach (T secondItem in secondSlot)
                            resultSet.Put((firstItem, secondItem));
                }
            }

            return resultSet;
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

            for (int i = 0; i < _slots[slotIndex].Count; i++)
            {
                if (_comparer.Equals(_slots[slotIndex][i], value))
                    return i;
            }

            return -1;
        }

        public static IEqualityComparer<(T, T)> GetOrderedPairComparer(IEqualityComparer<T> comparer = null)
            => new OrderedPairComparer(comparer);

        private class OrderedPairComparer : IEqualityComparer<(T, T)>
        {
            private readonly IEqualityComparer<T> _comparer;

            internal OrderedPairComparer(IEqualityComparer<T> comparer = null)
            {
                _comparer = comparer ?? EqualityComparer<T>.Default;
            }

            public bool Equals((T, T) x, (T, T) y)
            {
                return _comparer.Equals(x.Item1, y.Item1) && _comparer.Equals(x.Item2, y.Item2);
            }

            public int GetHashCode((T, T) obj)
            {
                int hash = 17;

                hash = hash * 31 + _comparer.GetHashCode(obj.Item1);
                hash = hash * 31 + _comparer.GetHashCode(obj.Item2);

                return hash;
            }
        }
    }
}