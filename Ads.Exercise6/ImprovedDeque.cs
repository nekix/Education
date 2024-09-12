using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise6
{
    public class ImprovedDeque<T>
    {
        private List<T> _frontItems;
        private List<T> _tailItems;

        public ImprovedDeque()
        {
            _frontItems = new List<T>();
            _tailItems = new List<T>();
        }

        public void AddFront(T item)
            => _frontItems.Add(item);

        public void AddTail(T item)
            => _tailItems.Add(item);

        public T RemoveFront()
            => Remove(_frontItems, _tailItems);

        public T RemoveTail()
            => Remove(_tailItems, _frontItems);

        public int Size()
            => _frontItems.Count() + _tailItems.Count();

        private T Remove(List<T> fromItems, List<T> oppositeItems)
        {
            if (fromItems.Count != 0)
            {
                var item = fromItems.Last();
                fromItems.Remove(item);
                return item;
            }

            if (oppositeItems.Count == 0)
                return default;

            // Балансирование элементов между списками
            var moveItemsCount = (oppositeItems.Count() - 1) / 2;

            // Копирование обязательно в обратном порядке
            for (int i = moveItemsCount; i >= 1; i--)
                fromItems.Add(oppositeItems[i]);

            var removedItem = oppositeItems[0];
            oppositeItems.RemoveRange(0, moveItemsCount + 1);
            return removedItem;
        }
    }
}
