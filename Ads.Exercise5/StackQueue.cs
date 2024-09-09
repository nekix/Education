using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise5
{
    public class StackQueue<T>
    {
        private Stack<T> _items;
        private Stack<T> _tempStack;

        public StackQueue()
        {
            _items = new Stack<T>();
            _tempStack = new Stack<T>();
        }

        public void Enqueue(T item)
            => _items.Push(item);

        public T Dequeue()
        {
            if(_tempStack.Count > 0)
                return _tempStack.Pop();

            while(_items.Count > 0)
                _tempStack.Push(_items.Pop());

            return _tempStack.Count > 0 ? _tempStack.Pop() : default;
        }

        public int Size()
            => _items.Count + _tempStack.Count;
    }
}
