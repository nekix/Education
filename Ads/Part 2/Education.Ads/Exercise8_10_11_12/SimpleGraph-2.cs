using AlgorithmsDataStructures;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    // ========= Урок 8 ===========

    // Задание 2.
    // Реализуйте направленный граф, представленный матрицей смежности. 
    // Добавить метод проверки на цикличность.
    // Направленный граф реализовал наследованием от простого графа,
    // только переопределил соответствующие методы AddEdge и RemoveEdge.
    // Сложности методов остались такими же.
    public partial class DirectedGraph : SimpleGraph
    {
        private const int NotVisisted = 0;
        private const int Visited = 1;
        private const int Verified = 2;

        public DirectedGraph(int size) : base(size)
        {
        }

        public override void AddEdge(int v1, int v2)
        {
            if (!(IsInRange(v1) && IsInRange(v2)))
                return;

            if (vertex[v1] == null || vertex[v2] == null)
                return;

            m_adjacency[v1, v2] = 1;
        }

        public override void RemoveEdge(int v1, int v2)
        {
            if (!(IsInRange(v1)))
                return;

            m_adjacency[v1, v2] = 0;
        }

        public bool CheckIsCyclical()
        {
            byte[] vertexes = new byte[vertex.Length];

            for (int v1 = 0; v1 < vertex.Length; v1++)
            {
                if (vertexes[v1] == NotVisisted && CheckIsCyclical(v1, vertexes))
                    return true;
            }

            return false;
        }

        private bool CheckIsCyclical(int v1, byte[] vertexes)
        {
            vertexes[v1] = Visited;

            for (int v2 = 0; v2 < vertex.Length; v2++)
            {
                if (IsEdge(v1, v2))
                {
                    if (vertexes[v2] == Visited)
                        return true;
                    else if (vertexes[v2] == NotVisisted && CheckIsCyclical(v2, vertexes))
                        return true;
                }
            }

            vertexes[v1] = Verified;

            return false;
        }
    }

    // ========================================================================

    // Рефлексия по уроку 6.

    // Задание 3.
    // В первоначальной реализации использовал параметр ref,
    // что является плохой практикой, исправил.
    // Также пропустил банальную ошибку (вместо параметра
    // проверку начинал от Root ноды всего дерева) из-за
    // плохих тестов, исправил, отметил необходимость 
    // лучше готовить тест-кейсы.


    // ========= Урок 10 ===========

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
    public partial class DirectedGraph : SimpleGraph
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

    // Рефлексия по уроку 8.

    // Задание 2.
    // Реализуйте направленный граф, представленный матрицей смежности. 
    // Добавить метод проверки на цикличность.
    // Можно было можно было не рассматривать петли (рёбра из вершины в себя).
    // Можно было бы взять только верхнюю или нижнюю треугольную часть матрицы.

    // ========= Урок 11 ===========

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

            SimpleTreeNode<T> firstNode = GetMaxPathNode(Root, true, out int _);
            _ = GetMaxPathNode(firstNode, false, out int pathLength);

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
                        if (current.Children[i].Hit != !hitValue)
                            continue;

                        current.Children[i].Hit = hitValue;
                        nextVertexes.Enqueue(current.Children[i]);
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


    // ========= Урок 12 ===========


    // Задание 1*. Добавить метод, подсчитывающий число треугольников в графе.
    // Сложность временная: O (N^3), где N - число вершин (три цикла).
    // Сложность пространственная: O (1).
    // (на такой порядок увеличивается максимальное число треугольников
    // от размра графа при больших значениях N).
    // Обходим все узлы в цикле, проверяем каждую пару соседей,
    // если треугольник, то добавляем в список треугольников.

    // Небольшие оптимизиации за счёт двух внутренних циклов,
    // которые начинаются не с 0, а с позиции внешнего цикла + 1.
    // Также небольшая оптимизация по памяти за счёт возврата IEnumerable
    // в GetTriangles(int startV). Это позволяет не создавать список
    // треугольников (List<List<Vertex<T>>>). Также это позволяет
    // избежать дубликатов треугольников.

    // Этот алгоритм подойдет только для неориентированного графа.
    // В ориентированном графе нужно каждый цикл начинать с нуля, чтобы
    // проверить оба направления. К тому же нужно будет учитывать дубликаты
    // треугольников, которые могут появиться.

    public partial class SimpleGraph<T>
    {
        public virtual int CountTriangles()
        {
            int count = 0;

            for (int startV = 0; startV < vertex.Length; startV++)
            {
                if (vertex[startV] == null)
                    continue;

                count += CountTriangles(startV);
            }

            return count;
        }

        private int CountTriangles(int startV)
        {
            int count = 0;

            for (int middleV = startV + 1; middleV < vertex.Length; middleV++)
            {
                if (vertex[middleV] == null)
                    continue;


                if (!IsEdge(startV, middleV))
                    continue;

                for (int endV = middleV + 1; endV < vertex.Length; endV++)
                {

                    if (vertex[endV] == null)
                        continue;

                    if (!IsEdge(startV, endV) || !IsEdge(middleV, endV))
                        continue;

                    count++;
                }
            }

            return count;
        }

        // Это неудачная версия, была идея через Dfs,
        // в котором я бы на каждой итерации цикла хранил предыдущую,
        // текущую и следующую вершины, и проверял бы, что они составляют
        // треугольник. Но концептуалньая проблема идеи (выявленная на тестах
        // на полный граф размера 5 и 6. Для размера 5 например не учитывал треугольник 0, 2, 4)
        // была в том, что при обходе не закрывались все варианты предыдущих вершин
        // для текущей, что приводило к пропусканию некоторых треугольников.
        // После анализа возможностей исправления этого недостатка стало понятно,
        // что обход в три цикла в таком случае будет работать проще и лучше,
        // чем усложненная версия этого подхода.

        // Также рассматривался вариант в Bfs, но для данной задачи он тоже не подходит
        // из-за сложностей учета особых случаев для него и сравнительной простоты
        // обхода в циклах.

        //public int CountTriangles()
        //{
        //    int count = 0;

        //    ResetHits();

        //    List<List<int>> trin = new List<List<int>>();

        //    for (int i = 0; i < vertex.Length; i++)
        //    {
        //        if (vertex[i] == null)
        //            continue;

        //        if (vertex[i].Hit)
        //            continue;

        //        foreach (List<int> triangle in DfsTriangles(i, -1))
        //        {
        //            count++;
        //            trin.Add(triangle);
        //        }

        //    }

        //    return count;
        //}

        //private IEnumerable<List<int>> DfsTriangles(int vCurrent, int vPrev)
        //{
        //    vertex[vCurrent].Hit = true;

        //    for (int vNext = 0; vNext < vertex.Length; vNext++)
        //    {
        //        if (vNext == vCurrent || vNext == vPrev)
        //            continue;

        //        if (vertex[vNext] == null)
        //            continue;

        //        if (!IsEdge(vCurrent, vNext))
        //            continue;

        //        if (vertex[vNext].Hit && IsEdge(vNext, vPrev))
        //        {
        //            yield return new List<int>(3) { vPrev, vCurrent, vNext };
        //        }
        //        else if (!vertex[vNext].Hit)
        //        {
        //            foreach (List<int> triangle in DfsTriangles(vNext, vCurrent))
        //                yield return triangle;
        //        }
        //    }
        //}
    }

    // Задание 2*. Метод поиска узлов, не входящих в треугольники только через интерфейс класса.
    // Сложность временная: O (N^3), где N - число вершин (три цикла при поиске треугольников).
    // Сложность пространственная: O (N^3), где N - число вершин
    // (на такой порядок увеличивается максимальное число треугольников
    // от размра графа при больших значениях N).
    // Обходим граф и получаем список всех вершин (выбрал dfs). После ищем все треугольники и
    // из списка всех вершин вычитаем список треугольников.
    // Для получения списка всех вершин реализовал DepthFirstSearch() метод.
    // Для поиска треугольников сделал метод GetTriangles() как в задании 1* CountTriangles()
    // только возвращающий список треугольников.

    public static class SimpleGraphExtensions
    {
        public static List<Vertex<T>> WeakVerticesV2<T>(this SimpleGraph<T> graph)
        {
            List<Vertex<T>> vertices = graph.DepthFirstSearch();
            List<List<Vertex<T>>> triangles = graph.GetTriangles();

            foreach (List<Vertex<T>> triangle in triangles)
                foreach (Vertex<T> vertex in triangle)
                    vertices.Remove(vertex);

            return vertices;
        }
    }

    public partial class SimpleGraph<T>
    {
        public List<List<Vertex<T>>> GetTriangles()
        {
            List<List<Vertex<T>>> triangles = new List<List<Vertex<T>>>();

            for (int startV = 0; startV < vertex.Length; startV++)
            {
                if (vertex[startV] == null)
                    continue;

                foreach (List<Vertex<T>> triangle in GetTriangles(startV))
                    triangles.Add(triangle);
            }

            return triangles;
        }

        private IEnumerable<List<Vertex<T>>> GetTriangles(int startV)
        {
            for (int middleV = startV + 1; middleV < vertex.Length; middleV++)
            {
                if (vertex[middleV] == null)
                    continue;


                if (!IsEdge(startV, middleV))
                    continue;

                for (int endV = middleV + 1; endV < vertex.Length; endV++)
                {

                    if (vertex[endV] == null)
                        continue;

                    if (!IsEdge(startV, endV) || !IsEdge(middleV, endV))
                        continue;

                    yield return new List<Vertex<T>>() { vertex[startV], vertex[middleV], vertex[endV] };
                }
            }
        }

        public List<Vertex<T>> DepthFirstSearch()
        {
            List<Vertex<T>> dfsVertex = new List<Vertex<T>>();

            for (int i = 0; i < vertex.Length; i++)
            {
                if (vertex[i] == null)
                    continue;

                if (vertex[i].Hit)
                    continue;

                foreach (Vertex<T> vert in DepthFirstSearch(i))
                    dfsVertex.Add(vert);
            }

            return dfsVertex;
        }

        private IEnumerable<Vertex<T>> DepthFirstSearch(int vFrom)
        {
            vertex[vFrom].Hit = true;

            yield return vertex[vFrom];

            for (int vTo = 0; vTo < vertex.Length; vTo++)
            {
                if (vertex[vTo] == null)
                    continue;

                if (vertex[vTo].Hit)
                    continue;

                if (IsEdge(vFrom, vTo) && !vertex[vTo].Hit)
                {
                    foreach (Vertex<T> childV in DepthFirstSearch(vTo))
                        yield return childV;
                }
            }
        }
    }

    // ========================================================================

    // Рефлексия по уроку 10.

    // Задание 1. Проверить связанный ли неориентированный граф.
    // Моя реализация могла быть улучшена, если бы я вынес Dfs в отдельный метод
    // (как сделал при решении задания 2 урока 12) и использовал его для пометки
    // всех пройденных узлов и в конце просто проверил бы, помечены ли все.
    // А не пытался искать с помощью существующего dfs метода путь от каждого узла
    // до каждого отдельно.
    // Выросла-бы читаемость, производительносьть (мне не пришлось бы очищать стек
    // каждый раз как я начиную обход и вприницпе сложность по времени уменьшилась бы
    // с O(N^2) -> O(N)).


    // Задание 2*. Найти длину самого длинного простого пути в оринетированном графе.
    // Моя реализация по большей части совпадает с рекомендованной, но я для хранение 
    // информации о том, что вершина включена в путь использовать поле Hit вершины.
    // Это позволило без поиска по списку (если бы он хранил путь) сразу проверить
    // для вершины, была ли она посещена.
}