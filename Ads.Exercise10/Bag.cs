using System;
using System.Collections.Generic;
using System.Linq;

namespace Ads.Exercise10
{
    public class Bag<T>
    {
        private const int _defaultCapacity = 20000;

        private List<List<T>>[] _slots;
        private int _count;

        private IEqualityComparer<T> _comparer;

        public Bag() : this(_defaultCapacity)
        {

        }

        public Bag(IEqualityComparer<T> comparer) : this(_defaultCapacity, comparer)
        {

        }

        public Bag(int capacity, IEqualityComparer<T> comparer = null)
        {
            _slots = new List<List<T>>[capacity];
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public int Size()
            => _count;

        public virtual void Put(T value)
        {
            if (_count == _slots.Length) return;

            int slotIndex = FindSlot(value);
            if (_slots[slotIndex] == null)
            {
                _slots[slotIndex] = new List<List<T>>(1);
            }

            int colIndex = FindCollisionIndex(value, slotIndex);
            if(colIndex == -1)
            {
                _slots[slotIndex].Add(new List<T>(1));
                colIndex = _slots[slotIndex].Count() - 1;
            }

            _slots[slotIndex][colIndex].Add(value);
            _count++;
        }

        public bool Get(T value)
        {
            if (_count == 0) return false;

            int slot = FindSlot(value);
            if (_slots[slot] == null) return false;

            int entryIndex = FindCollisionIndex(value, slot);

            return entryIndex != -1;
        }

        public bool Remove(T value)
        {
            if (_count == 0) return false;

            int slot = FindSlot(value);
            if (_slots[slot] == null) return false;

            int entryIndex = FindCollisionIndex(value, slot);
            if (entryIndex == -1)
                return false;

            // Удаление value
            if (!_slots[slot][entryIndex].Remove(value))
                return false;

            // Удаление списка дубликатов value если он пуст
            if (_slots[slot][entryIndex].Count == 0)
                _slots[slot].RemoveAt(entryIndex);

            _count--;
            return true;
        }

        public List<(T item, int frequency)> GetAll()
        {
            List<(T item, int frequency)> items = new List<(T item, int frequency)>(_count);

            foreach (List<List<T>> slot in _slots)
            {
                if (slot == null) continue;

                foreach (List<T> collisionGroup in slot)
                    items.Add((collisionGroup[0], collisionGroup.Count));
            }

            return items;
        }


        private int FindSlot(T value)
            => HashFun(value) % _slots.Length;

        private int FindCollisionIndex(T value, int slotIndex)
        {
            List<List<T>> slot = _slots[slotIndex];

            if (slot == null) return -1;

            // Обход списка "коллизий"
            // В каждом элементе списка находятся списки
            // повторяющихся элементов (T).
            for (int i = 0; i < slot.Count; i++)
            {
                // Поиск списка который содержит дубликаты value.
                if (_comparer.Equals(slot[i][0], value))
                    return i;
            }

            return -1;
        }

        private int HashFun(T value)
        {
            if (value == null)
                return 0;

            return Math.Abs(_comparer.GetHashCode(value));
        }
    }
}
