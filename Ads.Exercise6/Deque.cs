using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmsDataStructures
{

    public class Deque<T>
    {
        private List<T> _items;

        public Deque()
        {
            _items = new List<T>();
        }

        public T PeekFront()
            => _items.FirstOrDefault();

        public T PeekTail()
            => _items.LastOrDefault();

        public virtual void AddFront(T item)
            => _items.Add(item);

        public virtual void AddTail(T item)
            => _items.Insert(0, item);

        public virtual T RemoveFront()
        {
            var lastIndex = Size() - 1;

            if (lastIndex < 0)
                return default(T);

            var item = _items[lastIndex];

            _items.RemoveAt(lastIndex);

            return item;
        }

        public virtual T RemoveTail()
        {
            if (Size() < 1)
                return default(T);

            var item = _items[0];

            _items.RemoveAt(0);

            return item;
        }

        public int Size()
            => _items.Count();
    }

}