using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise11
{
    public class RemovableBloomFilter
    {
        public int filter_len;
        private byte[] _bitsData;

        private const int Hash1Multiplier = 17;
        private const int Hash2Multiplier = 223;

        public RemovableBloomFilter(int f_len)
        {
            filter_len = f_len;
            _bitsData = new byte[f_len];
        }

        public int Hash1(string str1)
            => SimpleStringHash(str1, Hash1Multiplier);

        public int Hash2(string str1)
            => SimpleStringHash(str1, Hash2Multiplier);

        public void Add(string str1)
        {
            int hash1 = Hash1(str1);
            int hash2 = Hash2(str1);

            _bitsData[hash1]++;
            _bitsData[hash2]++;
        }

        public bool IsValue(string str1)
        {
            int hash1 = Hash1(str1);
            int hash2 = Hash2(str1);

            return _bitsData[hash1] != 0 && _bitsData[hash2] != 0;
        }

        public bool Remove(string str1)
        {
            int hash1 = Hash1(str1);
            int hash2 = Hash2(str1);

            if (_bitsData[hash1] != 0 && _bitsData[hash2] != 0)
            {
                _bitsData[hash1]--;
                _bitsData[hash2]--;

                return true;
            }

            return false;
        }

        private int SimpleStringHash(string str, int multiplier)
        {
            int res = 0;

            for (int i = 0; i < str.Length; i++)
                res = (res * multiplier + ((int)str[i])) % filter_len;

            return res;
        }
    }
}
