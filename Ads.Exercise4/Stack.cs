using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace AlgorithmsDataStructures
{

    public class Stack<T>
    {
        public List<T> InnerList;

        public Stack()
        {
            InnerList = new List<T>();
        }

        public int Size()
            => InnerList.Count;

        public T Pop()
        {
            var size = Size();

            if (size == 0)
                return default;

            var item = InnerList[size - 1];
            InnerList.RemoveAt(size - 1);

            return item;
        }

        public void Push(T val)
        {
            InnerList.Add(val);
        }

        public T Peek()
        {
            var size = Size();

            if(size == 0)
                return default;

            return InnerList[size - 1];
        }
    }

}