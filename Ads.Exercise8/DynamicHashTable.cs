using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise8
{
    public class DynamicHashTable
    {
        private string[] _slots;

        public int Size { get; private set; }
        public int Step { get; private set; }
        
        public int Count { get; private set; }

        private const int SizeIncreaseMultiplier = 2;
        private const float MaxFillMultiplier = 0.75f;

        // Universal HashFunc parameters
        private int _u_p = 81293;
        private int _u_a = 0;
        private int _u_b = 0;
        private Random _random = new Random((int)(DateTime.Now.Millisecond % DateTime.Now.Ticks));

        // Polynomial HashFunc parameters
        private int _p_p = 35515;

        public DynamicHashTable(int sz, int stp)
        {
            Size = sz;
            Step = stp;
            _slots = new string[Size];
            for (int i = 0; i < Size; i++) _slots[i] = null;

            _u_a = _random.Next(1, Size - 1);
            _u_p = _random.Next(1, Size - 1);
        }

        public int HashFun(string value)
        {
            int sum = 0;

            foreach (var bt in Encoding.Unicode.GetBytes(value))
                sum += bt;

            return (_u_a * sum + _u_b) % _u_p % Size;
        }

        public int SeekSlot(string value)
        {
            if(string.IsNullOrEmpty(value)) return -1;

            var hash = HashFun(value);

            if (_slots[hash] == null) return hash;

            for (int i = (hash + Step) % Size; i != hash; i = (i + Step) % Size)
                if (_slots[i] == null) return i;

            return -1;
        }

        public int Put(string value)
        {
            if(Count / Size > MaxFillMultiplier)
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
        {
            var hash = HashFun(value);

            if (_slots[hash] == value) return hash;

            for (int i = (hash + Step) % Size; i != hash; i = (i + Step) % Size)
                if (_slots[i] == value) return i;

            return -1;
        }

        private void Resize(int newSize)
        {
            var oldSlots = _slots;

            _slots = new string[newSize];
            Size = newSize;
            Count = 0;

            _u_a = _random.Next(1, Size - 1);
            _u_p = _random.Next(1, Size - 1);

            foreach (var value in oldSlots.Where(s => s != null))
                Put(value);
        }

        private int PolynomialHashFunc(string value)
        {
            int hash = 0;

            for(int i = 0; i < Size; i++)
                hash = (hash * _p_p + value[i]) % Size;

            return hash;
        }
    }
}
