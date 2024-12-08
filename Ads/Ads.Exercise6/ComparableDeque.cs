using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmsDataStructures;

namespace Ads.Exercise6
{
    public class ComparableDeque<T> : Deque<T> where T : IComparable<T>
    {
        private List<T> _minItems;

        public ComparableDeque()
        {
            _minItems = new List<T>();
        }

        public override void AddFront(T item)
        {
            InsertInMin(item);

            base.AddFront(item);
        }

        public override void AddTail(T item)
        {
            InsertInMin(item);

            base.AddTail(item);
        }

        public override T RemoveFront()
        {
            var item = base.RemoveFront();

            if (Size() != 0) _minItems.Remove(item);

            return item;
        }

        public override T RemoveTail()
        {
            var item = base.RemoveTail();

            if (Size() != 0) _minItems.Remove(item);

            return item;
        }

        public T GetMin()
        {
            if (Size() == 0)
                return default;

            return _minItems.Last();
        }

        private void InsertInMin(T item)
        {
            for(int i = Size() - 1; i >= 0; i--)
            {
                if (_minItems[i].CompareTo(item) >= 0)
                {
                    _minItems.Insert(i + 1, item);
                    return;
                }
            }

            _minItems.Insert(0, item);
        }
    }
}