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

        public static int[] RemoveNodeByIndex(int[] a, int nodeIndex)
        {
            if (a.Length == 0)
                return new int[0];

            int[] sorted = new int[a.Length - 1];

            // Copy in sorted order without 'nodeIndex' element
            int j = 0;
            foreach (int index in GetInorderIndexes(a, 0).Where(i => i != nodeIndex))
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

            int size = (right - left + 1);
            double deep = (int)Math.Ceiling(Math.Log(right - left + 2, 2) - 1);
            var fullSize = (((int)Math.Pow(2, deep + 1) - 1));
            int lastLevelFullSize = (int)Math.Pow(2, deep);

            int preLastBranchSize = (fullSize - 1 - lastLevelFullSize) / 2;

            int t = left + preLastBranchSize;
            t = t + lastLevelFullSize / 2;

            // Кол-во отсутсвующих узлов последнего уровня
            int t1 = fullSize - size;
            // Половина от максимального числа узлов последнего уровня
            int t2 = lastLevelFullSize / 2;
            
            bool t3 = t1 < t2;

            int oldRoot;
            // Если число отсутсвющих узлов меньше или равно половине последнего уровня
            if (t1 <= t2)
            {
                oldRoot = lastLevelFullSize / 2 + left + preLastBranchSize;
            }
            else
            {
                oldRoot = (lastLevelFullSize - t1) + left + preLastBranchSize;
            }

            //int preFullSize = fullSize - lastLevelFullSize;
            //int lastLevelSize = fullSize - lastLevelFullSize;
            //int oldRoot = preFullSize / 2;
            //int oldRoot = (fullSize - 1) / 2 - 1;
            //int oldRoot = left + size / 2;

            bbst[root] = sorted[oldRoot];

            int leftChild = GetLeftChildIndex(root);
            int rigthChild = GetRightChildIndex(root);


            GenerateBBSTArray(sorted, bbst, GetLeftChildIndex(root), left, oldRoot - 1);
            GenerateBBSTArray(sorted, bbst, GetRightChildIndex(root), oldRoot + 1, right);
        }

        private static int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private static int GetRightChildIndex(int index)
            => 2 * index + 2;
    }
}