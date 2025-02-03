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
            int? firstIndex = FindKeyIndex(firstKey);
            if (firstIndex == null || firstIndex < 0)
                return null;

            int? secondIndex = FindKeyIndex(secondKey);
            if (secondIndex == null || secondIndex < 0)
                return null;

            return GetLcaIndexByIndexes(firstIndex.Value, secondIndex.Value);
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
            => FindKeyIndex(key, startNode) > 0;

        public List<int> WideAllNodes()
        {
            var nodes = new List<int>();
            
            foreach (var node in Tree)
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