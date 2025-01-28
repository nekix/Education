using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public static class BalancedBST
    {
        public static int[] GenerateBBSTArray(int[] a)
        {
            Array.Sort(a);

            int size = GetArraySize();

            int[] bbst = new int[size];

            int rootIndex = (a.Length - 1) / 2;



            return null;
        }

        private static int[] GenerateBBSTArray(int[] a, )

        private static int GetArraySize(int depth)
        {
            return (int)Math.Pow(2, depth + 1) - 1;
        }
    }
}