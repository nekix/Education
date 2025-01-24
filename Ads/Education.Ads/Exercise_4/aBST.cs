using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class aBST
    {
        public int?[] Tree; // массив ключей

        public aBST(int depth)
        {
            int tree_size = GetTreeSize(depth);
            Tree = new int?[tree_size];
            for (int i = 0; i<tree_size; i++) Tree[i] = null;
        }

        public int? FindKeyIndex(int key)
        {
            int index = 0;

            while (index < Tree.Length)
            {
                if (Tree[index] == key)
                {
                    return index;
                }
                if (Tree[index] == null)
                {
                    return -index;
                }
                else if (Tree[index] > key)
                {
                    index = GetLeftChildIndex(index);
                }
                else
                {
                    index = GetRightChildIndex(index);
                }
            }
            
            return null;
        }

        public int AddKey(int key)
        {
            int? index = FindKeyIndex(key);

            if (index == null)
                return -1;

            if (index > 0)
                return index.Value;

            if (index == 0 && Tree[index.Value] != null)
                return index.Value;

            Tree[-index.Value] = key;
            return -index.Value;
        }

        private int GetTreeSize(int depth)
        {
            int size = 0;

            for (int i = 0; i <= depth; i++)
                size += 1 << i;

            return size;
        }

        private int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private int GetRightChildIndex(int index)
            => 2 * index + 2;
    }
}