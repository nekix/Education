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
        private readonly int[] _capacities;

        private readonly int _dimensionsCount;

        public const int MinCapacity = 16;
        public const int CapacityIncreaseMultiplier = 2;

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

            // Проверка до возможного масштабирования намеренная,
            // т.к. иначе выход за пределелы может произойти
            // после нескольких расширений
            for(int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] < 0)
                    throw new IndexOutOfRangeException();

                // При превышении индексом вместимости измерения
                // более чем на 1 прервать выполнения
                if (_capacities[i] < indexes[i])
                    throw new IndexOutOfRangeException();
            }

            // Проверка масштабирования размерностей
            for (int i = 0; i < indexes.Length; i++)
            {
                // При превышении на 1
                // расширить измерение
                if (_capacities[i] == indexes[i])
                {
                    ResizeDimension(i, CapacityIncreaseMultiplier);
                }
                 
                // Проверка на выход за пределы count в последнем измерении
                if (i == indexes.Length - 1)
                {
                    var tempCountIndex = GetCountIndex(
                    indexes.Take(indexes.Length - 1)
                        .ToArray());

                    if (_counts[tempCountIndex] < indexes[i])
                        throw new IndexOutOfRangeException();
                }
            }

            // Смещение элементов и вставка нового.
            var itemIndex = GetItemIndex(indexes);

            var countIndex = GetCountIndex(
                indexes.Take(indexes.Length - 1)
                    .ToArray());

            for (int i = itemIndex + _counts[countIndex] - 1 - indexes.Last(); i >= itemIndex; --i)
                _items[i + 1] = _items[i];

            _items[itemIndex] = itm;
            _counts[countIndex]++;
        }

        public void Remove(params int[] indexes)
        {
            if (indexes.Length != _dimensionsCount)
                throw new IndexOutOfRangeException();

            var countIndex = GetCountIndex(
                indexes.Take(indexes.Length - 1)
                    .ToArray());

            // Проверка на выход за кол-во элементов
            if (_counts[countIndex] <= indexes.Last())
                throw new IndexOutOfRangeException();

            // Смещение элементов и вставка нового
            var itemIndex = GetItemIndex(indexes);

            for (int i = itemIndex + 1; i < itemIndex + _counts[countIndex] - indexes.Last(); i++)
                _items[i - 1] = _items[i];

            _items[itemIndex + _counts[countIndex] - indexes.Last() - 1] = default;
            _counts[countIndex]--;

            // Здесь можно было бы проверить массив на
            // "избыточность"

            // Это потребует несколько раз перебрать массив count
            // в поисках максимального числа элементов по каждому
            // измерению и сравнения его dimension для измерения

            // Заием можно будет вызывать метод ResizeDimension,
            // с коээффицентом уменьшения массива
        }

        private void ResizeDimension(int dimension, double resizeMultiplier)
        {
            _items = ResizeArray(_items, _capacities, dimension, _dimensionsCount, resizeMultiplier);

            // Расширение _counts если измерение не одно (в таком случае всего один _counts элемент)
            // и в случае если расширение не последнего элементе (не влияет на _counts)
            if (_dimensionsCount != 1 && dimension != _dimensionsCount - 1)
                _counts = ResizeArray(_counts, _capacities, dimension, _dimensionsCount - 1, resizeMultiplier);

            _capacities[dimension] = (int)(_capacities[dimension] * resizeMultiplier);
        }

        private static TArray[] ResizeArray<TArray>(TArray[] sourceArray, int[] capacities, int dimension, int dimensionsCount, double resizeMultiplier)
        {
            if (dimension < 0 || dimension >= dimensionsCount)
                throw new ArgumentException();

            // 1. Создаем массив нового размера
            var destArray = new TArray[(int)(sourceArray.Length * resizeMultiplier)];

            // 2. Считаем смещение элементов оригинального массива
            var itemsOffset = 1;
            for (int i = dimensionsCount - 1; i >= dimension; i--)
                itemsOffset *= capacities[i];

            // 3. Считаем смещение элементов в новом массиве
            var newItemsOffset = (int)(itemsOffset * resizeMultiplier);

            // 4 Считаем кол-во копируемых элементов за раз,
            // вариативность для поддержки расширения/сужения
            var copyItemsCount = itemsOffset > newItemsOffset
                ? newItemsOffset
                : itemsOffset;

            // 5. Считаем число операций копирования
            // пачек элементов
            var copyOperationCount = 1;
            for (int i = 0; i < dimension; i++)
                copyOperationCount *= capacities[i];

            // 6 Копируем _items пачками в новый массив
            for (int i = 0; i < copyOperationCount; i++)
            {
                Array.Copy(
                    sourceArray,
                    itemsOffset * i,
                    destArray,
                    newItemsOffset * i,
                    copyItemsCount);
            }

            return destArray;
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
    }
}
