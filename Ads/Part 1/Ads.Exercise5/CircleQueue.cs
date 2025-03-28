using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise5
{
    public class CircleQueue<T>
    {
        private readonly T[] _items;
        private int _startIndex;
        private int _count;

        public int Capacity { get; }

        public CircleQueue(int capacity)
        {
            _items = new T[capacity];
            Capacity = capacity;
        }

        public void Enqueue(T item)
        {
            if (IsFilled())
                throw new InvalidOperationException();

            var index = (_startIndex + _count) % Capacity;

            _items[index] = item;
            _count++;
        }

        public T Dequeue()
        {
            if(_count <= 0)
                return default(T);

            var item = _items[_startIndex];
            _items[_startIndex] = default(T);

            _startIndex = (_startIndex + 1) % Capacity;
            _count--;

            return item;
        }

        public int Size()
            => _count;

        public bool IsFilled()
            => _count == Capacity;
    }
}
