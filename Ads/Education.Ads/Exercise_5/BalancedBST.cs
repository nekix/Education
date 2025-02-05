using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public static class BalancedBST
    {
        public static int[] GenerateBBSTArray(int[] a)
        {
            int[] sorted = new int[a.Length];
            a.CopyTo(sorted, 0);
            Array.Sort(sorted);

            int[] bbst = new int[a.Length];

            GenerateBBSTArray(sorted, bbst, 0, 0, sorted.Length - 1);

            return bbst;
        }

        public static int[] RemoveNodeByIndex(int[] a, int nodeIndex)
        {
            if (a.Length == 0)
                return new int[0];

            int[] sorted = new int[a.Length - 1];

            // Copy in sorted order without 'nodeIndex' element
            int j = 0;
            foreach (int index in GetInorderIndexes(a, 0))
                if (index != nodeIndex)
                    sorted[j++] = a[index];

            int[] bbst = new int[a.Length - 1];
            
            GenerateBBSTArray(sorted, bbst, 0, 0, bbst.Length - 1);

            return bbst;
        }

        private static IEnumerable<int> GetInorderIndexes(int[] a, int index)
        {
            int left = GetLeftChildIndex(index);
            if (left < a.Length)
                foreach (int nextIndex in GetInorderIndexes(a, left))
                    yield return nextIndex;

            yield return index;

            int right = GetRightChildIndex(index);
            if (right < a.Length)
                foreach (int nextIndex in GetInorderIndexes(a, right))
                    yield return nextIndex;
        }

        private static void GenerateBBSTArray(int[] sorted, int[] bbst, int root, int left, int right)
        {
            if (root >= bbst.Length)
                return;

            if (left > right)
                return;

            // Support fully filled trees,
            // but not support partially filled trees.
            // int oldRoot = (right - left) / 2 + left;

            // Support fully and partially filled trees.
            int oldRoot = ComputeMiddleIndex(left, right);

            bbst[root] = sorted[oldRoot];

            GenerateBBSTArray(sorted, bbst, GetLeftChildIndex(root), left, oldRoot - 1);
            GenerateBBSTArray(sorted, bbst, GetRightChildIndex(root), oldRoot + 1, right);
        }

        private static int ComputeMiddleIndex(int left, int right)
        {
            // Subtree.
            int size = right - left + 1;
            double deep = (int)Math.Ceiling(Math.Log(right - left + 2, 2) - 1);
            var maxSize = (((int)Math.Pow(2, deep + 1) - 1));

            // Simple for fully filled tree.
            if (size == maxSize)
                return (right - left) / 2 + left;

            // Bottom level.
            int bottomLevelNodesMaxCount = (int)Math.Pow(2, deep);
            int bottomLevelHolesCount = maxSize - size;
            int leftBranchBottomNodesCount = bottomLevelNodesMaxCount / 2;

            // Above the bottom level without subtree root node.
            int middleLevelsNodesCount = (maxSize - 1 - bottomLevelNodesMaxCount) / 2;

            int oldRoot = left + middleLevelsNodesCount;

            // Check if the bottom nodes are in the right subtree only
            // or in both the right and left subtree.
            // Shift by the number of elements of the bottom level of the left tree.
            if (bottomLevelHolesCount <= leftBranchBottomNodesCount)
                oldRoot += leftBranchBottomNodesCount;
            else oldRoot += (bottomLevelNodesMaxCount - bottomLevelHolesCount);
            return oldRoot;
        }

        private static int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private static int GetRightChildIndex(int index)
            => 2 * index + 2;
    }
}