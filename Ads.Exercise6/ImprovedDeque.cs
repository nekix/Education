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
        {
            if(_frontItems.Count == 0)
        }

        public T RemoveTail()
        {

        }
    }
}
