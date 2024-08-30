using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures
{

    public class DynArray<T>
    {
        public T[] array;
        public int count;
        public int capacity;

        public const int MinCapacity = 16;
        public const int CapacityIncreaseMultiplier = 2;
        public const float CapacityReductionMultiplier = 1.5f;
        public const float MinFillMultiplier = 0.5f;

        public DynArray()
        {
            count = 0;
            MakeArray(16);
        }

        public void MakeArray(int new_capacity)
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
            if(count == capacity)
                MakeArray(capacity * CapacityIncreaseMultiplier);

            array[count] = itm;
            count++;
        }

        public void Insert(T itm, int index)
        {
            if (index < 0 || index > count)
                throw new IndexOutOfRangeException();

            if (capacity == count)
                MakeArray(capacity * CapacityIncreaseMultiplier);

            for (int i = count - 1; i >= index; --i)
                array[i + 1] = array[i];

            array[index] = itm;
            count++;
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            for (int i = index + 1; i < count; i++)
            {
                array[i - 1] = array[i];
            }

            array[count - 1] = default;
            count--;

            if ((float)count / capacity < MinFillMultiplier)
                MakeArray((int)(capacity / CapacityReductionMultiplier));
        }

    }
}