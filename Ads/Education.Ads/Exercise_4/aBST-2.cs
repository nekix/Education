using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsDataStructures2
{
    // ========= Урок 4  ===========

    // Задание 2.
    // Поиск LCA.
    // Индесы массива упрощают поиск по сравнению с классической рекурсивной реализацией
    // за счёт отсутсвия необходимости для каждого узла перемещаться к связынным узлам
    // через ссылки. Вместо этого достаточного простого вычисления индекса. Также это даёт
    // выигрышь в производительности, т.к.у нас нет обращения к разным областям памяти
    // (переход к ноде -> получение ссылки на потомка -> переход к ноде), вместо этого
    // мы работаем с одним массивом и перемещаемся только в его рамках.
    // Реализовал несколько версий:
    // 2.1 - Если на вход приходят сразу индексы узлов. В таком случае достаточно перемещаться вверх
    // по обеим ветвям и искать когда оба индекса укажут на один узел.
    //    Сложность временная: O(H), где H высота дерева (расстояние от самой глубокого узла до искомого).
    //    Сложность по памяти: O(1).
    // 2.2 - Если на входе ключи. Рекурсивная реализация со спуском из root вниз и последовательными
    // проверками. Исхожу из того, что передаваемые ключи могут не существовать в дереве.
    //    Сложность временная: O(H), где H высота дерева (расстояние от root до самого глубокого узла).
    //    Сложность по памяти: O(H), где H высота дерева(расстояние от root до самого глубокого узла).
    // 2.3 - Если на входе ключи. Итеративная реализация со спуском из root вниз и
    // последовательными проверками. Исхожу из того, что передаваемые ключи могут не существовать в дереве.
    //    Сложность временная: O(H), где H высота дерева (расстояние от root до самого глубокого узла).
    // Сложность по памяти: O(1).
    public partial class aBST
    {
        public int? GetLcaIndexByIndexes(int firstIndex, int secondIndex)
        {
            if (firstIndex >= Tree.Length || secondIndex >= Tree.Length)
                return null;

            while (firstIndex != secondIndex)
            {
                if (firstIndex < secondIndex)
                    secondIndex = GetParentIndex(secondIndex);
                else
                    firstIndex = GetParentIndex(firstIndex);
            }

            return firstIndex;
        }

        public int? GetLcaIndexByKeysRecursive(int firstKey, int secondKey)
        {
            if (Tree.Length == 0 || Tree[0] == null)
                return null;

            return GetLcaIndexByKeysRecursive(firstKey, secondKey, 0);
        }

        private int? GetLcaIndexByKeysRecursive(int firstKey, int secondKey, int index)
        {
            if (index >= Tree.Length)
                return null;

            if (Tree[index] == null)
                return null;

            if (Tree[index] == firstKey)
            {
                if (IsExistKey(secondKey, index))
                    return index;

                return null;
            }

            if (Tree[index] == secondKey)
            {
                if (IsExistKey(firstKey, index))
                    return index;

                return null;
            }

            if (Tree[index] > firstKey && Tree[index] < secondKey)
                return index;

            if (Tree[index] < firstKey && Tree[index] > secondKey)
                return index;

            index = Tree[index] > firstKey
                ? GetLeftChildIndex(index)
                : GetRightChildIndex(index);

            return GetLcaIndexByKeysRecursive(firstKey, secondKey, index);
        }

        public int? GetLcaIndexByKeysIterative(int firstKey, int secondKey)
        {
            if (Tree.Length == 0)
                return null;

            if (Tree[0] == firstKey || Tree[0] == secondKey)
                return 0;

            int currentIndex = 0;

            while (true)
            {
                if (currentIndex >= Tree.Length)
                    return null;

                if (Tree[currentIndex] == null)
                    return null;

                if (Tree[currentIndex] == firstKey)
                {
                    if (IsExistKey(secondKey, currentIndex))
                        return currentIndex;

                    return null;
                }

                if (Tree[currentIndex] == secondKey)
                {
                    if (IsExistKey(firstKey, currentIndex))
                        return currentIndex;

                    return null;
                }

                // Two left
                if (Tree[currentIndex] > firstKey && Tree[currentIndex] > secondKey)
                    currentIndex = GetLeftChildIndex(currentIndex);
                // Two right
                else if (Tree[currentIndex] < firstKey && Tree[currentIndex] < secondKey)
                    currentIndex = GetRightChildIndex(currentIndex);
                // One left, one right
                else
                    return currentIndex;
            }
        }

        private bool IsExistKey(int key, int startNode)
        {
            int? index = FindKeyIndex(key, startNode);

            return index >= 0 && Tree[index.Value] != null;
        }
    }

    // Задание 3.
    // Оптимизированный обход в ширину.
    // По факту, это будет просто последовательный обход
    // массива с пропуском пустых ячеек.
    // Сложность временная: O(N), где N - размер массива.
    // Сложность по памяти: O(N), где N - размер массива.
    public partial class aBST
    {
        public List<int> WideAllNodes()
        {
            List<int> nodes = new List<int>();

            foreach (int? node in Tree)
                if (node.HasValue)
                    nodes.Add(node.Value);

            return nodes;
        }
    }

    // ========================================================================

    // Рефлексия по уроку 2.

    // Реализация метода Count в BST<T>.
    // При отправке на сервер получил ошибку: Исключение при вызове Count() для непустого дерева.
    // Была ошибка System.Collections.Generic.Stack`1.Push(T item) точнее OutOfMemoryException

    // Оказалось, что ошибка на самом деле была в методе DeleteNodeByKey (при формировании 
    // тестового дерева использовался в т.ч. этот метод).

    // Ошибка: в методе DeleteNodeByKey я не реализовал очистку ссылки на узел-преемник
    // из его родительского узла для случая, когда удаляется узел с двумя дочерними,
    // а его узел-преемник не имеет правого потомка.
    // Мои тесты не отлавили эту ошибку по причине, что именно случай для узла-преемника
    // без правого потомка я не реализовал в тестовом сценарии.

    // Также я заметил, что моим тестам не хватает сценариев, при которых задействуется
    // не только тестируемый в данный момент метод класса, но и другие методы данного
    // класса(особенно для формирования например тестового дерева). Их применение позволило
    // бы перекрестно выявить ошибки и кейсы, которые иначе я могу случайно пропустить.
    // На будущее я буду пробовать данный подход чтобы снизить вероятность пропуска таких вариантов.


    // Задание 2.
    // Метод поиска путей от корня к листьям, длина которых равна заданной.
    // Задание 3.
    // Метод поиска путей от корня к листьям с максимальной суммой значений узлов по пути. 
    // Для этих заданий лучше объеденить обе проверки в один рекурсивный алгоритм,
    // а логику конкретных проверок делегируем автономным функциям (генерализация/обобщение).
    // В рамках исправлений я вынес реализацию заданий в рекурсивный алгоритм. Также я вынес функцию проверки на лист в отдельный метод.
    // Я думал о том, как вынести обе проверки в один алгоритм, но не нашел способа, который обеспечивал бы при этом для
    // задания 2 прерывание обхода дерева при достижении необходимой глубины(когда текущая глубина равна заданной, но узел не лист).
    // Но вообще возможность есть как за счёт IEnumerable, который позволяет прервать обход раньше.
}
