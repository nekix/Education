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

        // HashFunc parameters
        private int _p = 81293;
        private int _a = 0;
        private int _b = 0;
        private Random _random = new Random((int)(DateTime.Now.Millisecond % DateTime.Now.Ticks));

        public DynamicHashTable(int sz, int stp)
        {
            Size = sz;
            Step = stp;
            _slots = new string[Size];
            for (int i = 0; i < Size; i++) _slots[i] = null;

            _a = _random.Next(1, Size - 1);
            _p = _random.Next(1, Size - 1);
        }

        public int HashFun(string value)
        {
            int sum = 0;

            foreach (var bt in Encoding.Unicode.GetBytes(value))
                sum += bt;

            return (_a * sum + _b) % _p % Size;
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

            _a = _random.Next(1, Size - 1);
            _p = _random.Next(1, Size - 1);

            foreach (var value in oldSlots.Where(s => s != null))
                Put(value);
        }
    }
}
