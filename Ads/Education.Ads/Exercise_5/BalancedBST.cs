using System;
using System.Collections.Generic;
using System.Linq;

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

        public static int[] RemoveNodeByKey(int[] a, int nodeIndex)
        {
            int[] sorted = new int[a.Length - 1];

            // Copy in sorted order without 'nodeIndex' element
            int i = 0;
            foreach (int index in GetInorderIndexes(a, 0))
                if (i != index)
                    sorted[i++] = a[index];

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

            int oldRoot = (right - left) / 2 + left;

            bbst[root] = sorted[oldRoot];

            GenerateBBSTArray(sorted, bbst, GetLeftChildIndex(root), left, oldRoot - 1);
            GenerateBBSTArray(sorted, bbst, GetRightChildIndex(root), oldRoot + 1, right);
        }

        private static int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private static int GetRightChildIndex(int index)
            => 2 * index + 2;
    }
}