using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsDataStructures2
{
    // ========= Урок 7 ===========

    // Задание 1.
    // Реализовать бинарную пирамиду (операция Make-Heap).
    // В пирамиде поддерживаю счётчик числа элементов, что упрощает работу с пирамидой в других методах.
    // Сложность по времени: O(N* log2(N)), где N - число элементов входного массива(log2(N) - высота пирамиды).
    // Сложность по памяти: O(N) - где N, число элементов входного массива.
    // (код в основном файле)


    // Задание 2.
    // Методы удаления максимально приоритетного элемента и вставки нового.

    // Удаление:
    // Сложность по времени: O(log2(N)), где N - число элементов пирамиды.
    // Сложность по памяти: O(1).
    // (код в основном файле)

    // Добавление:
    // Сложность по времени: O(log2(N)), где N - число элементов пирамиды.
    // Сложность по памяти: O(1).
    // (код в основном файле)


    // Задание 3.
    // Метод проверки, что массив содержит корректную пирамиду.
    // Сложность по времени: O(log2(N)), где N - число элементов пирамиды.
    // Сложность по памяти: O(1).
    // (код в основном файле)
    public partial class Heap
    {
        public static bool IsHeap(int[] heapArray)
        {
            if (heapArray.Length == 0)
                return false;

            double deep = Math.Log(heapArray.Length + 1, 2) - 1;
            if (deep != Math.Truncate(deep))
                return false;

            int last = heapArray.Length - 1;
            for (; last >= 0; last--)
                if (heapArray[last] != EmptyKey)
                    break;

            for (int i = last; i > 0; i--)
            {
                if (heapArray[i] <= EmptyKey)
                    return false;

                int parent = GetParentIndex(i);

                if (heapArray[i] > heapArray[parent])
                    return false;
            }

            return true;
        }
    }

    // Задание 4.
    // Метод поиска максимального элемента в диапазоне значений.
    // Сложность по времени: O(N), где N - число элементов массива.
    // Сложность по памяти: O(N) - где N - число элементов массива (за счёт рекурсивного стека вызова).
    // (код в основном файле)
    public partial class Heap
    {
        public int GetMaxInRange(int minValue, int maxValue)
        {
            if (HeapArray.Length == 0)
                return EmptyKey;

            if (minValue > maxValue)
                return EmptyKey;

            return GetMaxInRange(minValue, maxValue, 0);
        }

        private int GetMaxInRange(int minValue, int maxValue, int index)
        {
            if (index >= HeapArray.Length)
                return EmptyKey;

            int key = HeapArray[index];

            if (key <= maxValue && key >= minValue)
                return key;

            if (key < minValue)
                return EmptyKey;

            int left = GetLeftChildIndex(index);
            int right = GetRightChildIndex(index);

            int leftMax = GetMaxInRange(minValue, maxValue, left);
            int rightMax = GetMaxInRange(minValue, maxValue, right);

            return Math.Max(leftMax, rightMax);
        }
    }

    // Задание 5.
    // Подумать над эффективным алгортмом поиска в пирамиде по заданному условию.
    // Свойства пирамиды говорят нам о том, что подузел текущего узла всегда меньше или равен ему.
    // Из этого вытекают следующие оптимизации:

    // Для поиска по условию "Больше заданного":
    // - обход сверху-вниз.Если текущий узел меньше заданного, то исключаем все поддерево из поиска.

    // Для поиска по условию "Меньше заданного":
    // - обход сниху-вверх.Если текущий узел больше заданного, то вся родительская ветка исключается из поиска.

    // Это позволяет исключить неподходящие ветки.

    // Для поиска сразу по двум условиям можно комбинировать данные подходы - провести
    // параллельный обход сверху-вниз и сниху-вверх навстречу друг-другу.При этом если
    // у нас исключается поддерево/ветка с одной стороны, то с противоположной мы их
    // тоже исключаем.

    // Сложность останется также O(N), но для НЕ худших случаем она уменьшится за счёт
    // обхода только части пирамиды.
    // Другие оптимизации возможны если у нас дополнительные знания о характере
    // распределения данных в пирамиде, в таком случае имеет смысл выбирать с какой
    // стороны организовать поиск на этой основе. Может быть в какой-то ситуации выгодно
    // начинать поиск вообще с середины (среднего уровня) кучи и т.д.


    // Задание 6.
    // Метод объединения текущей пирамиды с пирамидой параметром

    // ПЕРВОНЧАЛЬНАЯ НЕПРАВИЛЬНАЯ ИДЕЯ (реализация здесь уже правильная)
    // Дополнительно добавил метод, выводящий кол-во элементов в пирамиде.
    // Основная идея в том, что если добавлять элементы в пирамиду в
    // отсортированном порядке, то сложность вставки O(1), а GetMax как
    // раз выдает их в отсортированном порядке. Достаточно доставать
    // элементы из обеих пирамид и добавлять в порядке убывания в новую пирамиду.

    // (только внешний интерфейс, без доступа к массиву напрямую).
    // Сложность: O(N* log2(N)+M* log2(M)) по времени,
    // O(N + M) по памяти, где N, M - число элементов пирамид на входе.
    public partial class Heap
    {
        public Heap Union(Heap secondHeap)
        {
            int firstCount = GetCount();
            int secondCount = secondHeap.GetCount();

            int size = firstCount + secondCount;

            int depth = GetMinDepthBySize(size);

            Heap heap = new Heap();
            heap.MakeHeap(Array.Empty<int>(), depth);

            int firstKey = GetMax();
            int secondKey = secondHeap.GetMax();

            for (int i = 0; i < size; i++)
            {
                if (firstKey > secondKey)
                {
                    heap.Add(firstKey);
                    firstKey = GetMax();
                }
                else
                {
                    heap.Add(secondKey);
                    secondKey = secondHeap.GetMax();
                }
            }

            return heap;
        }

        public Heap UnionV2(Heap secondHeap)
        {
            Heap firstCopy = Copy();
            Heap secondCopy = secondHeap.Copy();

            List<int> keys = new List<int>();

            int firstKey = firstCopy.GetMax();
            int secondKey = secondCopy.GetMax();

            while (firstKey != EmptyKey || secondKey != EmptyKey)
            {
                if (firstKey > secondKey)
                {
                    keys.Add(firstKey);
                    firstKey = firstCopy.GetMax();
                }
                else
                {
                    keys.Add(secondKey);
                    secondKey = secondCopy.GetMax();
                }
            }

            int depth = GetMinDepthBySize(keys.Count);

            Heap heap = new Heap();
            heap.MakeHeap(keys.ToArray(), depth);

            return heap;
        }

        public Heap Copy()
        {
            Heap heap = new Heap();
            heap.HeapArray = new int[HeapArray.Length];

            Array.Copy(HeapArray, heap.HeapArray, HeapArray.Length);
            heap._count = _count;

            return heap;
        }

        public int GetCount()
        {
            return _count;
        }

        private static int GetMinDepthBySize(int size)
        {
            if (size == 0)
                return 0;

            return (int)Math.Ceiling(Math.Log(size + 1, 2) - 1);
        }
    }

    // ========================================================================

    // Рефлексия по уроку 5.

    // Задание 2.
    // Оценить, насколько поиск узла в дереве, представленном в виде массива,
    // эффективнее (или неэффективнее) поиска узла в классическом дереве с указателями.

    // Ответ можно дополнить тем, что как правило заполненность деревьев в проекте
    // примерно известна, поэтому тут можно смотреть насколько например актуальна память, 
    // или хотим поэффективнее кэши задействовать(когда дерево (почти) полное),
    // тогда массив лучше.
}
