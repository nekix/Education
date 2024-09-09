using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmsDataStructures
{

    public class Queue<T>
    {
        private readonly List<T> _items;

        public Queue()
        {
            _items = new List<T>();
        }

        public void Enqueue(T item)
        {
            _items.Insert(0, item);
        }

        public T Dequeue()
        {
            var lastIndex = Size() - 1;

            if(lastIndex > -1)
            {
                var item = _items[lastIndex];
                _items.RemoveAt(lastIndex);

                return item;
            }

            return default(T);
        }

        public int Size()
            => _items.Count;
    }
}