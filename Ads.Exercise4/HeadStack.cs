using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace AlgorithmsDataStructures
{
    public class HeadStack<T>
    {
        public LinkedList<T> InnerList;

        public HeadStack()
        {
            InnerList = new LinkedList<T>();
        }

        public int Size()
            => InnerList.Count;

        public T Pop()
        {
            var size = Size();

            if (size == 0)
                return default;

            var item = InnerList.First.Value;
            InnerList.RemoveFirst();

            return item;
        }

        public void Push(T val)
        {
            InnerList.AddFirst(val);
        }

        public T Peek()
        {
            var size = Size();

            if (size == 0)
                return default;

            return InnerList.First.Value;
        }
    }
}