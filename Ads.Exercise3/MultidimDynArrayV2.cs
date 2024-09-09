using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise3
{
    public class MultidimDynArrayV2<T>
    {
        public object[] array;
        public int count;
        public int capacity;

        public const int MinCapacity = 16;
        public const int CapacityIncreaseMultiplier = 2;
        public const float CapacityReductionMultiplier = 1.5f;
        public const float MinFillMultiplier = 0.5f;

        public MultidimDynArrayV2()
        {
            count = 0;
            MakeArray(16);
        }

        public void MakeArray(int capacity)
        {
            capacity = capacity < MinCapacity ? MinCapacity : capacity;

            Array.Resize(ref array, capacity);

            this.capacity = capacity;

            count = count < capacity ? count : capacity;
        }

        public bool IsMultidimDynArray(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            return array[index] is MultidimDynArrayV2<int>;
        }

        public bool IsDynArray(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            return array[index] is DynArray<int>;
        }

        public MultidimDynArrayV2<T> GetMultidimDynArray(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            var item = array[index] as MultidimDynArrayV2<T>;

            return item ?? throw new InvalidOperationException();
        }

        public DynArray<T> GetDynArray(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            var item = array[index] as DynArray<T>;

            return item ?? throw new InvalidOperationException();
        }

        public void AppendDynArray()
        {
            if (count == capacity)
                MakeArray(capacity * CapacityIncreaseMultiplier);

            array[count] = new DynArray<T>();
            count++;
        }

        public void AppendMultidimDynArray()
        {
            if (count == capacity)
                MakeArray(capacity * CapacityIncreaseMultiplier);

            array[count] = new MultidimDynArrayV2<T>();
            count++;
        }

        public void Remove(int dimensionIndex)
        {
            if (dimensionIndex < 0 || dimensionIndex >= count)
                throw new IndexOutOfRangeException();

            for (int i = dimensionIndex + 1; i < count; i++)
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
