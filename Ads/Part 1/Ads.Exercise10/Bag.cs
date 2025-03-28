using System;
using System.Collections.Generic;
using System.Linq;

namespace Ads.Exercise10
{
    public class Bag<T>
    {
        private const int _defaultCapacity = 20000;

        private List<LinkedList<T>>[] _slots;
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
            _slots = new List<LinkedList<T>>[capacity];
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public int Size()
            => _count;

        public virtual void Put(T value)
        {
            if (_count == _slots.Length) return;

            int slotIndex = FindSlotIndex(value);
            if (_slots[slotIndex] == null)
            {
                _slots[slotIndex] = new List<LinkedList<T>>(1);
            }

            int equalItemsIndex = FindEqualItemsIndex(value, slotIndex);
            if(equalItemsIndex == -1)
            {
                _slots[slotIndex].Add(new LinkedList<T>());
                equalItemsIndex = _slots[slotIndex].Count - 1;
            }

            _slots[slotIndex][equalItemsIndex].AddLast(value);
            _count++;
        }

        public bool Get(T value)
        {
            if (_count == 0) return false;

            int slot = FindSlotIndex(value);
            if (_slots[slot] == null) return false;

            int equalItemsIndex = FindEqualItemsIndex(value, slot);

            return equalItemsIndex != -1;
        }

        public bool Remove(T value)
        {
            if (_count == 0) return false;

            int slot = FindSlotIndex(value);
            if (_slots[slot] == null) return false;

            int equalItemsIndex = FindEqualItemsIndex(value, slot);
            if (equalItemsIndex == -1)
                return false;

            // Удаление value
            if (!_slots[slot][equalItemsIndex].Remove(value))
                return false;

            // Удаление списка дубликатов value если он пуст
            if (_slots[slot][equalItemsIndex].Count == 0)
                _slots[slot].RemoveAt(equalItemsIndex);

            _count--;
            return true;
        }

        public List<(T item, int frequency)> GetAll()
        {
            List<(T item, int frequency)> items = new List<(T item, int frequency)>(_count);

            foreach (List<LinkedList<T>> slot in _slots)
            {
                if (slot == null) continue;

                foreach (LinkedList<T> equalItems in slot)
                    items.Add((equalItems.First(), equalItems.Count));
            }

            return items;
        }


        private int FindSlotIndex(T value)
            => HashFun(value) % _slots.Length;

        private int FindEqualItemsIndex(T value, int slotIndex)
        {
            List<LinkedList<T>> slot = _slots[slotIndex];

            if (slot == null) return -1;

            // Обход списка "коллизий"
            // В каждом элементе списка находятся списки
            // повторяющихся элементов (T).
            for (int i = 0; i < slot.Count; i++)
            {
                // Поиск списка который содержит дубликаты value.
                if (_comparer.Equals(slot[i].First(), value))
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
