using System;

namespace Ads.Exercise9
{
    public enum DictionarySizes
    {
        _1 = 1,
        _2 = 2,
        _4 = 4,
        _8 = 8,
        _16 = 16,
        _32 = 32,
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
        _8192 = 8192,
        _16384 = 16384,
        _32768 = 32768,
        _65536 = 65536,
        _131072 = 131072,
        _262144 = 262144,
        _524288 = 524288,
        _1048576 = 1048576,
        _2097152 = 2097152,
        _4194304 = 4194304,
        _8388608 = 8388608,
        _16777216 = 16777216,
        _33554432 = 33554432,
        _67108864 = 67108864,
        _134217728 = 134217728,
        _268435456 = 268435456,
        _536870912 = 536870912,
        _1073741824 = 1073741824,
    }

    public class BitArrayDictionary<T>
    {
        private int _size;
        private byte[][] _slots;

        private int _bytesCount;

        private T[] _values;

        private int _step;

        public BitArrayDictionary(DictionarySizes size, int keyBitsCount)
        {
            _size = (int)size;
            _bytesCount = (int)Math.Ceiling((double)keyBitsCount / 8);

            _slots = new byte[_size][];

            _step = 1;
            _values = new T[_size];
        }

        public int HashFun(byte[] key)
            => BitConverter.ToInt32(key, 0) & (_size - 1);

        public bool IsKey(byte[] key)
        {
            var slot = FindSlot(key, false);

            return slot >= 0;
        }

        public void Put(byte[] key, T value)
        {
            var slot = FindSlot(key, true);

            if (slot < 0) return;

            _slots[slot] = key;
            _values[slot] = value;
        }

        public T Get(byte[] key)
        {
            var slot = FindSlot(key, false);

            return slot >= 0 ? _values[slot] : default;
        }

        public void Delete(byte[] key)
        {
            var slot = FindSlot(key, false);

            if (slot < 0) return;

            _slots[slot] = null;
            _values[slot] = default;
        }

        /// <summary>
        /// Return index of slots[] with current key.
        /// Return -1 if key not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="withEmpty">if set to <c>true</c> [with empty].</param>
        /// <returns></returns>
        private int FindSlot(byte[] key, bool withEmpty)
        {
            var hash = HashFun(key);

            if (_slots[hash] == null)
            {
                if (withEmpty) return hash;
                else return -1;
            }
                
            if (EqualsSlots(_slots[hash], key))
                return hash;

            for (int i = (hash + _step) & (_size - 1);
                i != hash && (withEmpty || _slots[i] != null);
                i = (i + _step) & (_size - 1))
            {
                if (EqualsSlots(_slots[i], key) || (withEmpty && _slots[i] == null))
                    return i;
            }

            return -1;
        }

        private bool EqualsSlots(byte[] bytes1, byte[] bytes2)
        {
            for (int i = 0; i < bytes1.Length; i++)
                if (bytes1[i] != bytes2[i]) return false;

            return true;
        }
    }
}
