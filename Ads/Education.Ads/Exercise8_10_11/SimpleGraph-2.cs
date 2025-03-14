using AlgorithmsDataStructures;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    // Урок 10

    // Задание 1*.
    // Проверить, является ли текущий граф связным (CheckIsConnected).
    // Сложность временная: O (N^2), где N - число узлов.
    // Сложность пространственная: O, где N - число узлов.
    // Пытаемся найти путь от первого узла до остальных,
    // если хоть один не найден, то граф не связный.
    public partial class SimpleGraph<T>
    {
        public bool CheckIsConnected()
        {
            if (vertex.Length < 2)
                return false;

            ResetHits();

            Stack<int> path = new Stack<int>();

            int firstV = -1;
            for (int i = 0; i < vertex.Length; i++)
            {
                if (vertex[i] == null)
                    continue;

                firstV = i;
                break;
            }

            for (int i = firstV; i < vertex.Length; i++)
            {
                if (vertex[i] != null && vertex[i].Hit == false)
                {
                    if (!TryDepthFirstSearchRecursive(firstV, i, path))
                        return false;

                    path.Clear();
                }
            }

            return true;
        }
    }

    // Задание 2*.
    // Найти длину самого длинного простого пути
    // в ориентированном графе.
    // Сложность временная: O (N^2), где N - число узлов.
    // Сложность пространственная: O, где N - число узлов.
    // Пытаемся найти путь от первого узла до остальных,
    // если хоть один не найден, то граф не связный.
    public partial class DirectedGraph
    {

        public int GetMaxSimplePathLength()
        {
            int maxPathCount = 0;

            Stack<int> path = new Stack<int>();

            for (int v = 0; v < vertex.Length; v++)
            {
                if (vertex[v] == null)
                    continue;

                int pathCount = GetMaxSimplePathLength(v, 0);
                if (pathCount > maxPathCount)
                    maxPathCount = pathCount;
            }

            return maxPathCount != 0
                ? maxPathCount - 1
                : 0;
        }

        private int GetMaxSimplePathLength(int v1, int pathCount)
        {
            vertex[v1].Hit = true;

            int maxPathCount = pathCount + 1;

            for (int v2 = 0; v2 < vertex.Length; v2++)
            {
                if (v1 == v2)
                    continue;

                if (vertex[v2] == null)
                    continue;

                if (IsEdge(v1, v2) && !vertex[v2].Hit)
                {
                    int childPath = GetMaxSimplePathLength(v2, pathCount + 1);
                    if (childPath > maxPathCount)
                        maxPathCount = childPath;
                }
            }

            vertex[v1].Hit = false;

            return maxPathCount;
        }
    }

    // ========================================================================

    // Урок 11
    public partial class SimpleTreeNode<T>
    {
        public bool Hit;
    } 

    public partial class SimpleTree<T>
    {
        // Задание 2*.
        // Используя BFS найти максимальное расстояние
        // между двумя узлами в дереве.
        // Сложность временная: O (N), где N - число узлов.
        // Сложность пространственная: O, где N - число узлов.
        // Основная идея в том, что мы находим самый глубокий узел
        // первым поиском, а потом ищем максимально удаленный от него вторым.
        // Для небольшого выигриша в производительности веду подсчёт длины
        // пути на ходу, против варианта с подъёмом вверх от крайних узлов до
        // пересечения и подсчёта общей длины.
        public int GetMaxPathLength()
        {
            if (Root == null)
                return 0;

            SimpleTreeNode<T> firstNode = GetMaxPathNode(Root, true, out int pathLength1);
            var test = GetMaxPathNode(firstNode, false, out int pathLength);

            return pathLength;
        }

        private SimpleTreeNode<T> GetMaxPathNode(SimpleTreeNode<T> root, bool hitValue, out int pathLength)
        {
            Queue<SimpleTreeNode<T>> nextVertexes = new Queue<SimpleTreeNode<T>>();
            nextVertexes.Enqueue(root);
            root.Hit = hitValue;

            int levelItemsCount = 1;
            pathLength = -1;

            SimpleTreeNode<T> current;

            do
            {
                current = nextVertexes.Dequeue();
                levelItemsCount--;

                if (current.Parent?.Hit == !hitValue)
                {
                    current.Parent.Hit = hitValue;
                    nextVertexes.Enqueue(current.Parent);
                }

                if (current.Children != null)
                {
                    for (int i = 0; i < current.Children.Count; i++)
                    {
                        if (current.Children[i].Hit == !hitValue)
                        {
                            current.Children[i].Hit = hitValue;
                            nextVertexes.Enqueue(current.Children[i]);
                        }
                    }
                }

                if (levelItemsCount == 0)
                {
                    pathLength++;
                    levelItemsCount = nextVertexes.Count;
                }
            }
            while (nextVertexes.Count != 0);

            return current;
        }
    }

    // Задание 3*.
    // Метод, который находит все циклы в текущем (неориентированном) графе с использованием BFS.
    public partial class SimpleGraph<T>
    {
        // Сложность временная: O (N^2), где N - число вершин (M - число циклов опускаю,
        // т.к. циклов всегда заметно меньше чем вершин).
        // Сложность пространственная: O (N^2), где N - число вершин
        // (список циклов, очередь, preVertex).
        // Проводим BFS начиная от каждой вершины, ищем уже
        // посещенные узлы как признак цикла. Восстанавливаем цикл по preVertex.
        // Проверяем, новый ли для нас это цикл или уже такой был (отсеиваем дубликаты).

        public List<List<int>> FindAllCycles()
        {
         
            Queue<int> nextVertexes = new Queue<int>();
            int[] preVertex = new int[vertex.Length];

            // O(N)
            ResetHits();
            ClearPreVertex(preVertex);

            List<List<int>> cycles = new List<List<int>>();

            // O(N^2)
            for (int i = 0; i < vertex.Length; i++)
            {
                if (vertex[i] == null || vertex[i].Hit)
                    continue;

                // O(N)
                foreach (List<int> cycle in FindCyclesBFS(nextVertexes, preVertex, i))
                {
                    bool isNew = true;

                    foreach (List<int> existedCycle in cycles)
                    {
                        // O(M)
                        if (!IsEqualPaths(cycle, existedCycle))
                            continue;

                        isNew = false;
                        break;
                    }

                    if (isNew)
                        cycles.Add(cycle);
                }

                // O(N)
                ClearPreVertex(preVertex);
                ResetHits();
            }

            return cycles;
        }

        // O(N)
        private IEnumerable<List<int>> FindCyclesBFS(Queue<int> nextVertexes, int[] preVertex, int v)
        {
            int current = v;

            vertex[current].Hit = true;
            nextVertexes.Enqueue(current);

            while (nextVertexes.Count != 0)
            {
                current = nextVertexes.Dequeue();

                for (int i = 0; i < vertex.Length; i++)
                {
                    if (current == i)
                        continue;

                    if (vertex[i] == null)
                        continue;

                    if (!IsEdge(current, i))
                        continue;

                    if (!vertex[i].Hit)
                    {
                        nextVertexes.Enqueue(i);
                        vertex[i].Hit = true;
                        preVertex[i] = current;
                    }
                    else if (i != preVertex[current])
                    {
                        yield return RecoverCycle(preVertex, current, i);
                    }
                }
            }
        }

        // O(N)
        private List<int> RecoverCycle(int[] preVertex, int current, int hitV)
        {
            List<int> result = new List<int>();
            Deque<int> cycle = new Deque<int>();

            // Собираем цикл двигаясь от current и hitV вершин обратно,
            // к самому началу.
            for (int i = current; i != -1 && preVertex[i] != hitV; i = preVertex[i])
                cycle.AddFront(i);

            for (int i = hitV; i != -1 && preVertex[i] != current; i = preVertex[i])
                cycle.AddTail(i);

            // Необходимо удалить возможные дубли, т.к. по preVertex
            // мы спускаемся до самого начального узла, а цикл мог начаться
            // не от него, а дальше.
            // Соотвественно путь от начальной вершины до точки расхождения
            // не будет входить в цикл.
            int lastV = -1;
            while (cycle.PeekTail() == cycle.PeekFront())
            {
                lastV = cycle.RemoveTail();
                cycle.RemoveFront();
            }

            // Это наша точка расхождения при подъёме,
            // её нужно обязательно захватить.
            if (lastV > -1)
                result.Add(lastV);
                
            while(cycle.Size() != 0)
                result.Add(cycle.RemoveFront());

            return result;
        }

        // O(N)
        private bool IsEqualPaths(List<int> cycle, List<int> existedCycle)
        {
            if (existedCycle.Count != cycle.Count)
                return false;

            bool isEquals = true;

            int shift = cycle.IndexOf(existedCycle[0]);

            // Прямой обход
            for (int i = 1, j = shift + 1; i < cycle.Count; i++, j++)
            {
                if (j == cycle.Count)
                    j = 0;

                if (existedCycle[i] != cycle[j])
                    isEquals = false;
            }

            if (isEquals)
                return true;

            isEquals = true;

            // Обратный обход (вдруг направления различаются)
            for (int i = cycle.Count - 1, j = shift + 1; i > 0; i--, j++)
            {
                if (j == cycle.Count)
                    j = 0;

                if (existedCycle[i] != cycle[j])
                    isEquals = false;
            }

            return isEquals;
        }

        private void ClearPreVertex(int[] preVertex)
        {
            for (int i = 0; i < preVertex.Length; i++)
                preVertex[i] = -1;
        }
    }

    // ========================================================================

    // Рефлексия по уроку 9.

    // Задание 1. Лес из чётных деревьев, из которого удалено максимально возможное количество рёбер.
    // Моя реализация совпала с описанной. Как и для многих рекурсивных алгоритмов
    // при необходимости этот можно было бы превратить в итеративный. Но для этого
    // нужно задействовать стек, т.к. у нас post order обход и нужно хранить текущий путь.


    // https://github.com/nekix/Education/blob/main/Ads/Education.Ads/Exercise1_9/SimpleTree-2.cs
    // Задание 2*. Метод, балансирующий чётное двоичное дерево.
    // В моей реализации есть небольщое отличие, я брал в качестве центрального элемента
    // именно правый центральный, т.к. это позволяет мне смещать узлы на последнем уровне
    // влево (справа всегда меньше узлов получается).


    // Задание 3*. Метод, определяющий кол-во чётных поддеревьев для заданного узла.
    // По началу возникло желание использовать ref параметр, но вспомнив запрет
    // на его использование и попадавшуюся в коде других разработчиков из-за его
    // использования путанницу, я отверг эту идею.
    // Долго думал над выбором между out параметром или чем-то вроде кортежа для
    // возвращаемого значения, но пришел к выводу, что читаемость с out в данном
    // случае будет выше, да и кортежи по моему мнению помимо быстрых набросков
    // на LINQ для проверки данных в дебаге вносят только сложность.
}