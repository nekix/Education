﻿using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public partial class Heap
    {
        private const int EmptyKey = -1;

        private int _count = 0;

        public int[] HeapArray;

        public Heap() { HeapArray = null; }

        public void MakeHeap(int[] a, int depth)
        {
            int size = GetSizeByDepth(depth);

            HeapArray = new int[size];

            foreach (int key in a)
                if (_count != size)
                    Add(key);

            for (int i = _count; i < size; i++)
                HeapArray[i] = EmptyKey;
        }

        public int GetMax()
        {
            if (_count == 0)
                return EmptyKey;

            int maxKey = HeapArray[0];
            int lastKey = HeapArray[_count - 1];

            HeapArray[_count - 1] = EmptyKey;

            _count--;

            if (_count == 0)
                return maxKey;

            GetMaxRebalance(lastKey, 0);

            return maxKey;
        }

        private void GetMaxRebalance(int key, int index)
        {
            int leftChild = GetLeftChildIndex(index);
            int rightChild = GetRightChildIndex(index);
            
            if (leftChild >= HeapArray.Length)
            {
                HeapArray[index] = key;
                return;
            }

            int maxChild = HeapArray[leftChild] < HeapArray[rightChild]
                ? rightChild
                : leftChild;

            if (key > HeapArray[maxChild])
            {
                HeapArray[index] = key;
                return;
            }

            HeapArray[index] = HeapArray[maxChild];

            GetMaxRebalance(key, maxChild);
        }

        public bool Add(int key)
        {
            if (HeapArray.Length == _count)
                return false;

            AddRebalance(key, _count);

            _count++;

            return true;
        }

        private void AddRebalance(int key, int index)
        {
            if (index == 0)
            {
                HeapArray[index] = key;
                return;
            }

            int parent = GetParentIndex(index);

            if (key < HeapArray[parent])
            {
                HeapArray[index] = key;
                return;
            }

            HeapArray[index] = HeapArray[parent];

            AddRebalance(key, parent);
        }

        private static int GetSizeByDepth(int depth)
            => (int)Math.Pow(2, depth + 1) - 1;

        private static int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private static int GetRightChildIndex(int index)
            => 2 * index + 2;

        private static int GetParentIndex(int index)
            => (index - 1) / 2;
    }
}