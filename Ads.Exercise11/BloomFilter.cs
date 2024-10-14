using System.Collections.Generic;
using System;
using System.IO;

namespace AlgorithmsDataStructures
{
    public class BloomFilter
    {
        public int filter_len;
        private int[] _bitsData;

        private const int BitsPerInt32 = 32;

        private const int Hash1Multiplier = 17;
        private const int Hash2Multiplier = 223;

        public BloomFilter(int f_len)
        {
            filter_len = f_len;
            _bitsData = new int[GetArrayLength(filter_len)];
        }

        public int Hash1(string str1)
            => SimpleStringHash(str1, Hash1Multiplier);

        public int Hash2(string str1)
            => SimpleStringHash(str1, Hash2Multiplier);

        public void Add(string str1)
        {
            int hash1 = Hash1(str1);
            int hash2 = Hash2(str1);

            Add(hash1);
            Add(hash2);
        }

        public bool IsValue(string str1)
        {
            int hash1 = Hash1(str1);
            int hash2 = Hash2(str1);

            return IsValue(hash1) && IsValue(hash2);
        }

        public BloomFilter Union(IEnumerable<BloomFilter> filters)
        {
            BloomFilter resFilter = new BloomFilter(filter_len);

            // Copy current filter
            for (int i = 0; i < _bitsData.Length; i++)
                resFilter._bitsData[i] |= _bitsData[i];

            // "And" with others filters
            foreach (BloomFilter filter in filters)
                for (int i = 0; i < resFilter._bitsData.Length; i++)
                    resFilter._bitsData[i] |= filter._bitsData[i];

            return resFilter;
        }

        private bool IsValue(int hash)
        {
            int index = GetIndexAndMask(hash, out int mask);

            return (_bitsData[index] & mask) != 0;
        }

        private void Add(int hash)
        {
            int index = GetIndexAndMask(hash, out int mask);

            _bitsData[index] |= mask;
        }

        private int SimpleStringHash(string str, int multiplier)
        {
            int res = 0;

            for (int i = 0; i < str.Length; i++)
                res = (res * multiplier + ((int)str[i])) % filter_len;

            return res;
        }

        private static int GetIndexAndMask(int hash, out int mask)
        {
            int div = Math.DivRem(hash, BitsPerInt32, out int rem);

            mask = 1 << rem;

            return div;
        }

        private static int GetArrayLength(int bitsCount)
        {
            if (bitsCount <= 0) return 0;

            return (bitsCount - 1) / BitsPerInt32 + 1;
        }
    }
}