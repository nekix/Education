using AlgorithmsDataStructures;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
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
}