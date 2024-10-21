using AlgorithmsDataStructures;
using System.Collections.Generic;
using System.Linq;

namespace Ads.Exercise11
{
    public static class BloomFilterReverser
    {
        public class RecoverStringsResult
        {
            public HashSet<string> CleanResult { get; set; }

            public HashSet<string> FalsePositiveResults { get; set; }

            public HashSet<string> CollisionResults { get; set; }
        }

        /// <summary>
        /// Восстановление множества при условии,
        /// что известно все множество возможных элементов.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="candidates">The candidates to filter set.</param>
        /// <returns></returns>
        public static RecoverStringsResult RecoverStrings(this BloomFilter filter, IEnumerable<string> candidates)
        {
            // Множество позволяет не беспокоится о повторяемости элементов, 
            // а также упрощает операции объеденения и исключения.
            // N-мерный массив позволяет создать удобную адрессацию
            // т.к. индексами являются значения хеш-функции
            HashSet<string>[,] array = new HashSet<string>[filter.filter_len, filter.filter_len];

            // Добавление всех определенных механизмом
            // фильтра Блюма элементов (в т.ч. коллизии,
            // ложноположительные)
            foreach (string candidate in candidates)
            {
                // Данную часть потенциально можно оптимизировать
                // за счёт рефлексии для вызова private методов
                // и снижения дублирования вызовов Hash1() и Hash2()
                if (!filter.IsValue(candidate)) continue;

                // Хеши как индексы в массиве элементов
                // Элементы с одним индексом 
                int hash1 = filter.Hash1(candidate);
                int hash2 = filter.Hash2(candidate);

                if (array[hash1, hash2] == null)
                    array[hash1, hash2] = new HashSet<string>();

                array[hash1, hash2].Add(candidate);
            }

            // Выделение отдельно коллизий и ложноположительных
            HashSet<string> collisionResults = GetCollisions(array);
            HashSet<string> falsePositiveResults = GetFalsePositive(array);

            // Выделение определяемых однозначно элементов (эталонных)
            HashSet<string> cleanResult = new HashSet<string>();
            foreach (HashSet<string> set in array)
                if(set != null)
                    cleanResult.UnionWith(set);
            
            cleanResult.ExceptWith(falsePositiveResults);
            cleanResult.ExceptWith(collisionResults);

            // Формирование итого результата
            return new RecoverStringsResult()
            {
                CleanResult = cleanResult,
                FalsePositiveResults = falsePositiveResults,
                CollisionResults = collisionResults,
            };
        }

        /// <summary>
        /// Выделяет коллизии из массива.
        /// Коллизиями явлются ТОЛЬКО: item1Hash1 == item2Hash1 && item1Hash2 == item2Hash2.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static HashSet<string> GetCollisions(HashSet<string>[,] array)
        {
            HashSet<string> collisionResults = new HashSet<string>();

            foreach (HashSet<string> set in array)
            {
                if (set != null && set.Count != 1)
                    collisionResults.UnionWith(set);
            }

            return collisionResults;
        }

        /// <summary>
        /// Выделить только ложно-положительные элементы
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        private static HashSet<string> GetFalsePositive(HashSet<string>[,] array)
        {
            int length = array.GetLength(0);

            HashSet<string> falsePositiveResults = new HashSet<string>();

            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    CheckAndAddFalsePositive(array, falsePositiveResults, i, j);

            return falsePositiveResults;
        }

        /// <summary>
        /// Проверяет элменты HashSet под указанным индексом
        /// в массиве array на ложно-положительтность.
        /// Если элемент ложно-положительный, то добавляет его
        /// в falsePositive.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="falsePositive">The false positive.</param>
        /// <param name="iIndex">Index of the i.</param>
        /// <param name="jIndex">Index of the j.</param>
        private static void CheckAndAddFalsePositive(HashSet<string>[,] array, HashSet<string> falsePositive, int iIndex, int jIndex)
        {
            int length = array.GetLength(0);

            if (array[iIndex, jIndex] == null) return;

            // Элемент с совпадающими хешами, только в обратном порядке
            if (array[jIndex, iIndex] != null)
            {
                falsePositive.UnionWith(array[iIndex, jIndex]);
                falsePositive.UnionWith(array[jIndex, iIndex]);

                return;
            }

            bool leftFirst = false;
            bool leftSecond = false;

            bool rightFirst = false;
            bool rightSecond = false;

            // Поиск пар элементов, в которой у каждого хотя бы один
            // из индексов (хешей) равен индексам iIndex и jIndex
            // (то есть добавив их в блум фильтр они займут в том числе
            // iIndex и jIndex позиции).
            for (int k = 0; k < length; k++)
            {
                if (k != jIndex && array[iIndex, k] != null)
                    leftFirst = true;
                if (array[jIndex, k] != null)
                    leftSecond = true;
                if (array[k, iIndex] != null)
                    rightFirst = true;
                if (k != iIndex && array[k, jIndex] != null)
                    rightSecond = true;

                if (leftFirst && rightFirst && leftSecond && rightSecond)
                    break;
            }

            if ((leftFirst && leftSecond) || (leftFirst && rightSecond) || (rightFirst && rightSecond))
            {
                falsePositive.UnionWith(array[iIndex, jIndex]);
            }        
        }
    }
}
