using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace AlgorithmsDataStructures2
{
    public class aBST
    {
        public int?[] Tree;

        public aBST(int depth)
        {
            int tree_size = GetTreeSize(depth);
            Tree = new int?[tree_size];
            for (int i = 0; i < tree_size; i++) Tree[i] = null;
        }

        public int? FindKeyIndex(int key)
        {
            return FindKeyIndex(key, 0);
        }

        public int AddKey(int key)
        {
            int? index = FindKeyIndex(key);

            if (!index.HasValue)
                return -1;

            if (index > 0)
                return index.Value;

            if (index == 0 && Tree[index.Value] != null)
                return index.Value;

            Tree[-index.Value] = key;
            return -index.Value;
        }

        public int? GetLcaIndexByIndexes(int firstIndex, int secondIndex)
        {
            if (firstIndex >= Tree.Length || secondIndex >= Tree.Length)
                return null;

            while (firstIndex != secondIndex)
            {
                if (firstIndex < secondIndex)
                    secondIndex = GetParentIndex(secondIndex);
                else
                    firstIndex = GetParentIndex(firstIndex);
            }

            return firstIndex;
        }

        public int? GetLcaIndexByKeysRecursive(int firstKey, int secondKey)
        {
            if (Tree.Length == 0 || Tree[0] == null)
                return null;

            return GetLcaIndexByKeysRecursive(firstKey, secondKey, 0);
        }

        private int? GetLcaIndexByKeysRecursive(int firstKey, int secondKey, int index)
        {
            if (index >= Tree.Length)
                return null;

            if (Tree[index] == null)
                return null;

            if (Tree[index] == firstKey)
            {
                if (IsExistKey(secondKey, index))
                    return index;

                return null;
            }

            if (Tree[index] == secondKey)
            {
                if (IsExistKey(firstKey, index))
                    return index;

                return null;
            }

            if (Tree[index] > firstKey && Tree[index] < secondKey)
                return index;

            if (Tree[index] < firstKey && Tree[index] > secondKey)
                return index;

            index = Tree[index] > firstKey
                ? GetLeftChildIndex(index)
                : GetRightChildIndex(index);

            return GetLcaIndexByKeysRecursive(firstKey, secondKey, index);
        }

        public int? GetLcaIndexByKeysIterative(int firstKey, int secondKey)
        {
            if (Tree.Length == 0)
                return null;

            if (Tree[0] == firstKey || Tree[0] == secondKey)
                return 0;

            int currentIndex = 0;

            while (true)
            {
                if (currentIndex >= Tree.Length)
                    return null;

                if (Tree[currentIndex] == null)
                    return null;

                if (Tree[currentIndex] == firstKey)
                {
                    if (IsExistKey(secondKey, currentIndex))
                        return currentIndex;

                    return null;
                }
                
                if (Tree[currentIndex] == secondKey)
                {
                    if (IsExistKey(firstKey, currentIndex))
                        return currentIndex;

                    return null;
                }

                // Two left
                if (Tree[currentIndex] > firstKey && Tree[currentIndex] > secondKey)
                    currentIndex = GetLeftChildIndex(currentIndex);
                // Two right
                else if (Tree[currentIndex] < firstKey && Tree[currentIndex] < secondKey)
                    currentIndex = GetRightChildIndex(currentIndex);
                // One left, one right
                else
                    return currentIndex;
            }
        }

        private bool IsExistKey(int key, int startNode)
        {
            int? index = FindKeyIndex(key, startNode);

            return index >= 0 && Tree[index.Value] != null;
        }

        public List<int> WideAllNodes()
        {
            List<int> nodes = new List<int>();
            
            foreach (int? node in Tree)
                if (node.HasValue)
                    nodes.Add(node.Value);

            return nodes;
        }

        private int GetTreeSize(int depth)
        {
            int size = 0;

            for (int i = 0; i <= depth; i++)
                size += 1 << i;

            return size;
        }

        protected int? FindKeyIndex(int key, int startIndex)
        {
            if (startIndex >= Tree.Length)
                return null;

            if (Tree[startIndex] == key)
                return startIndex;

            if (Tree[startIndex] == null)
                return -startIndex;

            startIndex = Tree[startIndex] > key
                ? GetLeftChildIndex(startIndex)
                : GetRightChildIndex(startIndex);

            return FindKeyIndex(key, startIndex);
        }

        private int GetParentIndex(int index)
            => (index - 1) / 2;

        private int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private int GetRightChildIndex(int index)
            => 2 * index + 2;
    }
}