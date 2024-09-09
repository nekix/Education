using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise3
{
    internal class BankedArray<T>
    {
        public T[] array;
        public int count;
        public int capacity;
        public const int MinCapacity = 16;

        private int _bank;

        private const int AppendCost = 3;
        private const int InsertCost = 3;
        private const int CopyCost = 3;
        private const int RemoveCost = -1;

        public const float MinFillMultiplier = 0.5f;

        public BankedArray()
        {
            count = 0;
            array = new T[MinCapacity];
            capacity = MinCapacity;
            count = 0;
        }

        public void ReallocateArray()
        {
            // уменьшать ли кол-во элементов если отрицательный банк
            // для реаллокации при удалении элементов
            // и как это балансировать

            // capacity from full bank
            var new_capacity = (int)Math.Pow(_bank, 2);

            Array.Resize(ref array, new_capacity);

            capacity = new_capacity;

            count = count < new_capacity ? count : new_capacity;

            // pay from bank
            _bank -= (int)Math.Log(new_capacity, 2);
        }

        public T GetItem(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            return array[index];
        }

        public void Append(T itm)
        {
            // pay for append
            _bank += AppendCost;

            if (count == capacity)
                ReallocateArray();

            array[count] = itm;

            count++;
        }

        public void Insert(T itm, int index)
        {
            if (index < 0 || index > count)
                throw new IndexOutOfRangeException();

            // pay for insert. (count - index - 1) pay for copy?
            _bank += InsertCost + (count - index - 1);

            if (capacity == count)
                ReallocateArray();

            for (int i = count - 1; i >= index; --i)
            {
                array[i + 1] = array[i];

                // Pay for copy?
                _bank -= CopyCost;
            }

            array[index] = itm;
            count++;
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            // Pay for remove?
            _bank -= RemoveCost;

            for (int i = index + 1; i < count; i++)
            {
                array[i - 1] = array[i];

                // Pay for copy?
                _bank -= CopyCost;
            }

            array[count - 1] = default;
            count--;

            if ((float)count / capacity < MinFillMultiplier)
                ReallocateArray();
        }

    }
}
