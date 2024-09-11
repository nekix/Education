using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmsDataStructures;

namespace Ads.Exercise3
{
    /// <summary>
    /// Расширяемый массив
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultidimDynArrayV3<T>
    {
        private T[] _items;
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

            _counts = Enumerable.Repeat(0, (int)Math.Pow(MinCapacity, dimensionsСount - 1)).ToArray();
            _capacities = Enumerable.Repeat(MinCapacity, dimensionsСount).ToArray();
            _items = new T[(int)Math.Pow(MinCapacity, dimensionsСount)];

            _dimensionsCount = dimensionsСount;
        }

        public T GetItem(params int[] indexes)
        {
            if (GetCount(indexes.Take(indexes.Length - 1).ToArray()) <= indexes.Last())
                throw new IndexOutOfRangeException();

            var index = GetItemIndex(indexes);

            return _items[index];
        }

        public int GetCount(params int[] indexes)
        {
            var index = GetCountIndex(indexes);

            return _counts[index];
        }

        public int GetCapacity(int dimension)
        {
            if (dimension < 0 || dimension >= _dimensionsCount)
                throw new IndexOutOfRangeException();

            return _capacities[dimension];
        }

        public void Insert(T itm, params int[] indexes)
        {
            if (indexes.Length != _dimensionsCount)
                throw new IndexOutOfRangeException();

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

            var countIndex = GetCountIndex(
                indexes.Take(indexes.Length - 1)
                    .ToArray());

            for (int i = itemIndex + _counts[countIndex] - 1; i >= itemIndex; --i)
                _items[i + 1] = _items[i];

            _items[itemIndex] = itm;
            _counts[countIndex]++;
        }

        private void ExpandDimension(int dimension)
        {
            if (dimension < 0 || dimension >= _dimensionsCount)
                throw new ArgumentException();

            var newItems = new T[_items.Length * CapacityIncreaseMultiplier];

            int itemsOffset = 1;
            for (int i = _dimensionsCount - 1; i >= dimension; i--)
                itemsOffset *= _capacities[i];

            var copyOperationCount = 1;
            for (int i = 0; i < dimension; i++)
                copyOperationCount *= _capacities[i];

            // Копируем _items пачками в новый массив
            for (int i = 0; i < copyOperationCount; i++)
            {
                Array.Copy(
                    _items,
                    itemsOffset * i,
                    newItems,
                    itemsOffset * CapacityIncreaseMultiplier * i,
                    itemsOffset);
            }

            // Не меняем массив колличеств элементов
            // если у нас расширяется последнее измерение
            // и если измерение всего одно
            if (_dimensionsCount != 1 && dimension != _dimensionsCount - 1)
            {
                var newCounts = new int[_counts.Length * CapacityIncreaseMultiplier];

                var countsOffset = 1;
                for (int i = _dimensionsCount - 2; i >= dimension; i--)
                    countsOffset *= _capacities[i];

                // Кол-во операций копирования _counts элементов
                // совпадает с количеством операций копирования _items элементов
                for (int i = 0; i < copyOperationCount; i++)
                {
                    Array.Copy(
                        _counts,
                        countsOffset * i,
                        newCounts,
                        countsOffset * CapacityIncreaseMultiplier * i,
                        countsOffset);
                }

                _counts = newCounts;
            }

            _items = newItems;
            _capacities[dimension] *= CapacityIncreaseMultiplier;
        }

        private int GetItemIndex(params int[] indexes)
        {
            if (indexes.Length != _dimensionsCount)
                throw new IndexOutOfRangeException();

            // Смещение в массиве _items
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

            // Последний индекс смещает без множителя
            indexOffset += indexes.Last();

            return indexOffset;
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

            // Последний индекс смещает без множителя
            indexOffset += indexes.Last();

            return indexOffset;
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
