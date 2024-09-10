using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmsDataStructures;

namespace Ads.Exercise3
{
    public class MultidimDynArrayV3<T>
    {
        private T[] _array;
        private int[] _counts;
        private int[] _capacities;

        private int _dimensionsCount;

        public const int MinCapacity = 16;
        public const int CapacityIncreaseMultiplier = 2;
        public const float CapacityReductionMultiplier = 1.5f;
        public const float MinFillMultiplier = 0.5f;

        public MultidimDynArrayV3(int dimensionsСount)
        {
            if (dimensionsСount < 1)
                throw new ArgumentException();

            // Доделать
            _counts = Enumerable.Repeat(0, (int)Math.Pow(MinCapacity, dimensionsСount - 1)).ToArray();
            _capacities = Enumerable.Repeat(MinCapacity, dimensionsСount).ToArray();
            _array = new T[(int)Math.Pow(MinCapacity, dimensionsСount)];

            _dimensionsCount = dimensionsСount;
        }

        //public void MakeArray(int new_capacity)
        //{
        //    new_capacity = new_capacity < MinCapacity ? MinCapacity : new_capacity;

        //    Array.Resize(ref array, new_capacity);

        //    capacity = new_capacity;

        //    count = count < new_capacity ? count : new_capacity;
        //}

        public T GetItem(params int[] indexes)
        {
            var index = GetItemIndex();

            return _array[index];
        }

        private int GetItemIndex(params int[] indexes)
        {
            if (indexes.Length != _dimensionsCount)
                throw new IndexOutOfRangeException();

            // Проверить корректность индексов
            // и выход за пределы последним индексом
            if (indexes.Last() >= GetCount(indexes.Take(indexes.Length - 1).ToArray()))
                throw new IndexOutOfRangeException();

            // Смещение в массиве _array
            // для текущей позиции
            int indexOffset = 0;
            int indexOffsetMultiplier = 1;

            // Первый элемент _capacities пропускаются намеренно
            // т.к. он не влияют на смещение индекса
            for (int i = _capacities.Length - 1; i >= 1; i--)
            {
                if (indexes[i - 1] < 0 || indexes[i - 1] >= _capacities[i - 1])
                    throw new IndexOutOfRangeException();

                // Смещение каждый раз увеличивается,
                // т.к. высшие измерения содержат смещения низжих
                indexOffsetMultiplier *= _capacities[i];
                indexOffset += indexOffsetMultiplier * indexes[i - 1];
            }

            if (indexes.Last() < 0 || indexes.Last() >= _capacities[_capacities.Length - 2])
                throw new IndexOutOfRangeException();

            // Последний индекс смещает без множителя
            indexOffset += indexes.Last();

            return indexOffset;
        }

        public int GetCount(params int[] indexes)
        {
            var index = GetCountIndex(indexes);

            return _counts[index];
        }

        private int GetCountIndex(params int[] indexes)
        {
            if (indexes.Length != _dimensionsCount - 1)
                throw new IndexOutOfRangeException();

            if (indexes.Length == 0)
                return 0;

            // Смещение в массиве _counts
            // для текущей позиции
            int indexOffset = 0;
            int indexOffsetMultiplier = 1;

            // Первый и последний элемент _capacities пропускаются намеренно
            // т.к. они не влияют на смещение индекса
            for (int i = _capacities.Length - 2; i >= 1; i--)
            {
                if (indexes[i - 1] < 0 || indexes[i - 1] >= _capacities[i - 1])
                    throw new IndexOutOfRangeException();

                // Смещение каждый раз увеличивается,
                // т.к. высшие измерения содержат смещения низжих
                indexOffsetMultiplier *= _capacities[i];
                indexOffset += indexOffsetMultiplier * indexes[i - 1];
            }

            if (indexes.Last() < 0 || indexes.Last() >= _capacities[_capacities.Length - 2])
                throw new IndexOutOfRangeException();

            // Последний индекс смещает без множителя
            indexOffset += indexes.Last();

            return indexOffset;
        }

        public int GetCapacity(int dimension)
        {
            if (dimension > _dimensionsCount)
                throw new IndexOutOfRangeException();

            return _capacities[dimension];
        }

        public void Insert(T itm, params int[] indexes)
        {
            if (indexes.Length != _dimensionsCount)
                throw new IndexOutOfRangeException();

            var countIndex = GetCountIndex(
                indexes.Take(indexes.Length - 1)
                    .ToArray());

            // Проверка масштабирования размерности
            for (int i = 0; i < indexes.Length; i++)
            {
                // При превышении индексом вместимости измерения
                // более чем на 1
                // прервать выполнения
                if (_capacities[i] < indexes[i])
                    throw new IndexOutOfRangeException();

                // При превышении на 1
                // расширить измерение
                if (_capacities[i] == indexes[i])
                    ExpandDimension(i);
            }

            var itemIndex = GetItemIndex(indexes);

            for (int i = itemIndex + _counts[countIndex] - 1; i >= itemIndex; --i)
                _array[i + 1] = _array[i];

            _array[itemIndex] = itm;
            _counts[countIndex]++;
        }

        private void ExpandDimension(int dimension)
        {
            _counts = Enumerable.Repeat(0, dimensionsСount).ToArray();
            _capacities = Enumerable.Repeat(MinCapacity, dimensionsСount).ToArray();
            _array = new T[(int)Math.Pow(MinCapacity, dimensionsСount)];

            int globalDimensionOffset = 0;

            if (dimension == 0)
            {
                globalDimensionOffset = _capacities[0] * CapacityIncreaseMultiplier;

                Array.Resize(ref _array, globalDimensionOffset);
                _capacities[0] = globalDimensionOffset;
                

                int countsSize = 0;

                if (_dimensionsCount == 1)
                {
                    countsSize = 1;
                }
                else
                {
                    for (int i = 0; i < _capacities.Length - 1; i++)
                    {
                        countsSize *= _capacities[i];
                    }
                }

                Array.Resize(ref _counts, countsSize);
            }

            for (int i = 0; i < dimension; i++)
            {

            }
        }

        //public void Remove(params int[] index)
        //{
        //    if (index < 0 || index >= count)
        //        throw new IndexOutOfRangeException();

        //    for (int i = index + 1; i < count; i++)
        //    {
        //        array[i - 1] = array[i];
        //    }

        //    array[count - 1] = default;
        //    count--;

        //    if ((float)count / capacity < MinFillMultiplier)
        //        MakeArray((int)(capacity / CapacityReductionMultiplier));
        //}
    }
}
