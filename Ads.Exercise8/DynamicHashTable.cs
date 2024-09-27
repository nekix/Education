using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise8
{
    public class DynamicHashTable
    {
        private string[] _slots;

        public int Size { get; private set; }
        
        public int Count { get; private set; }

        // The size must be a power of two.
        // Special for the double hashing method.
        private const int MinSize = 8;
        private const int SizeIncreaseMultiplier = 2;
        private const float MaxFillMultiplier = 0.75f;

        // Universal HashFunc parameters
        private int _u_p = 81293;
        private int _u_a;
        private int _u_b;
        // Seed random for _u_a and _u_b generation.
        private Random _random = new Random((int)(DateTime.Now.Millisecond % DateTime.Now.Ticks));

        // Polynomial HashFunc parameters
        private int _p_p = 35515;

        public DynamicHashTable()
        {
            Size = MinSize;
            _slots = new string[Size];

            for (int i = 0; i < Size; i++) _slots[i] = null;

            // Random universal HashFunc paramteters
            _u_a = _random.Next(1, Size - 1);
            _u_p = _random.Next(1, Size - 1);
        }

        public int HashFun(string value)
            => FirstDoubleHashingFunc(value);

        public int SeekSlot(string value)
            => FindWithDoubleHashingFunc(value, null);

        public int Put(string value)
        {
            // The size must be a power of two.
            // Special for the double hashing method.
            if (Count / Size > MaxFillMultiplier)
                Resize(Size * SizeIncreaseMultiplier);

            var slot = SeekSlot(value);

            if (slot != -1)
            {
                Count++;
                _slots[slot] = value;
            }

            return slot;
        }

        public int Find(string value)
            => FindWithDoubleHashingFunc(value, value);

        /// <summary>
        /// Resizes the specified new hash table size.
        /// </summary>
        /// <param name="newSize">The new size.</param>
        private void Resize(int newSize)
        {
            var oldSlots = _slots;

            _slots = new string[newSize];
            Size = newSize;
            Count = 0;

            // New random universal HashFunc paramteters
            _u_a = _random.Next(1, Size - 1);
            _u_b = _random.Next(1, Size - 1);

            foreach (var value in oldSlots.Where(s => s != null))
                Put(value);
        }

        /// <summary>
        /// Finds the first slot by specified key with specified value inside.
        /// </summary>
        /// <param name="key">The key of slot.</param>
        /// <param name="value">The value inside slot.</param>
        /// <returns></returns>
        private int FindWithDoubleHashingFunc(string key, string value)
        {
            if (key == null) return -1;

            var hash = HashFun(key);

            if (_slots[hash] == value) return hash;

            var hash2 = SecondDoubleHashingFunc(key);

            for (int i = (hash + hash2) % Size; i != hash; i = (i + hash2) % Size)
                if (_slots[i] == value) return i;

            return -1;
        }

        /// <summary>
        /// Implements the first double hashing function.
        /// </summary>
        /// <returns></returns>
        private int FirstDoubleHashingFunc(string value)
            => (PolynomialHashFunc(value) ^ UniversalHashFunc(value)) % Size;

        private int GetUnicodeBytesSum(string value)
        {
            int sum = 0;

            foreach (var bt in Encoding.Unicode.GetBytes(value))
                sum += bt;

            return sum;
        }

        /// <summary>
        /// Use for iterate through the slots.
        /// Works with a HashTable size that is a power of two.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int SecondDoubleHashingFunc(string value)
        {
            int sum = 0;

            foreach (var bt in Encoding.Unicode.GetBytes(value))
                sum += bt;

            return 1 + sum % (Size / 2) * 2;
        }

        /// <summary>
        /// A universals hash function.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int UniversalHashFunc(string value)
        {
            int sum = 0;

            foreach (var bt in Encoding.Unicode.GetBytes(value))
                sum += bt;

            return (_u_a * sum + _u_b) % _u_p % Size;
        }

        /// <summary>
        /// The polynomial hash function (special for string data).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int PolynomialHashFunc(string value)
        {
            int hash = 0;

            for(int i = 0; i < value.Length; i++)
                hash = ((hash * _p_p) + value[i]) % Size;

            return hash;
        }
    }
}
