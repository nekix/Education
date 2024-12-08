using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise3
{
    public class BankedArray<T>
    {
        public T[] array;
        public int count;
        public int capacity;

        public const int MinCapacity = 1;
        public const int CapacityIncreaseMultiplier = 2;
        public const float CapacityReductionMultiplier = 1.5f;
        public const float MinFillMultiplier = 0.5f;

        private int _bank;

        private const int AppendCost = 2;
        private const int InsertCost = 2;
        private const int RemoveCost = 1;
        private const int ReallocationCostMultiplier = 1;

        public BankedArray()
        {
            count = 0;
            _bank = 0;
            MakeArray(MinCapacity);
        }

        private void MakeArray(int new_capacity)
        {
            new_capacity = new_capacity < MinCapacity ? MinCapacity : new_capacity;

            Array.Resize(ref array, new_capacity);

            capacity = new_capacity;

            count = count < new_capacity ? count : new_capacity;
        }

        public T GetItem(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            return array[index];
        }

        public void Append(T itm)
        {
            if (count == capacity)
                ExpandArray();

            array[count] = itm;
            count++;

            _bank += AppendCost;
        }

        public void Insert(T itm, int index)
        {
            if (index < 0 || index > count)
                throw new IndexOutOfRangeException();

            if (capacity == count)
                ExpandArray();

            for (int i = count - 1; i >= index; --i)
                array[i + 1] = array[i];

            array[index] = itm;
            count++;

            _bank += InsertCost;
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            for (int i = index + 1; i < count; i++)
                array[i - 1] = array[i];

            array[count - 1] = default;
            count--;

            _bank += RemoveCost;

            if ((float)count / capacity < MinFillMultiplier)
                ShrinkArray();
        }

        private void ExpandArray()
        {
            var newCapacity = capacity * CapacityIncreaseMultiplier;
            var cost = (newCapacity - capacity) * ReallocationCostMultiplier;

            if (cost > _bank)
            {
                newCapacity = cost / ReallocationCostMultiplier + capacity;
                cost = (newCapacity - capacity) * ReallocationCostMultiplier;
            }

            MakeArray(newCapacity);

            _bank -= cost;
        }

        private void ShrinkArray()
        {
            var newCapacity = (int)(capacity * CapacityReductionMultiplier);
            var cost = (capacity - newCapacity) * ReallocationCostMultiplier;

            if (cost > _bank)
            {
                newCapacity = capacity - cost / ReallocationCostMultiplier;
                cost = (newCapacity - capacity) * ReallocationCostMultiplier;
            }

            MakeArray(newCapacity);

            _bank -= cost;
        }
    }
}
