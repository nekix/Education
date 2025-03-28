using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise3
{
    public class MultidimDynArray<T>
    {
        public DynArray<T>[] array;
        public int count;
        public int capacity;

        public const int MinCapacity = 16;
        public const int CapacityIncreaseMultiplier = 2;
        public const float CapacityReductionMultiplier = 1.5f;
        public const float MinFillMultiplier = 0.5f;

        public MultidimDynArray()
        {
            count = 0;
            MakeArray(16);
        }

        public void MakeArray(int dimensionsСount)
        {
            dimensionsСount = dimensionsСount < MinCapacity ? MinCapacity : dimensionsСount;

            Array.Resize(ref array, dimensionsСount);

            capacity = dimensionsСount;

            count = count < dimensionsСount ? count : dimensionsСount;
        }

        public T GetItem(int dimensionIndex, int index)
        {
            if (dimensionIndex < 0 || dimensionIndex >= count)
                throw new IndexOutOfRangeException();

            return array[dimensionIndex].GetItem(index);
        }

        public void AppendDimension()
        {
            if (count == capacity)
                MakeArray(capacity * CapacityIncreaseMultiplier);

            array[count] = new DynArray<T>();
            count++;
        }

        public void AppendItem(int dimensionIndex, T itm)
        {
            if (dimensionIndex < 0 || dimensionIndex >= count)
                throw new IndexOutOfRangeException();

            array[dimensionIndex].Append(itm);
        }

        public void Insert(int dimensionIndex, int itmIndex, T itm)
        {
            if (dimensionIndex < 0 || dimensionIndex > count)
                throw new IndexOutOfRangeException();

            array[dimensionIndex].Insert(itm, itmIndex);
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

        public void Remove(int dimensionIndex, int index)
        {
            if (dimensionIndex < 0 || dimensionIndex >= count)
                throw new IndexOutOfRangeException();

            array[dimensionIndex].Remove(index);
        }

    }
}
