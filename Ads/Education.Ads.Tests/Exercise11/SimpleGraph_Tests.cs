using AlgorithmsDataStructures2;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Education.Ads.Tests.Exercise11
{
    public class SimpleGraph_Tests
    {
        [Theory]
        [MemberData(nameof(GetBreadthFirstSearchData))]
        public void Should_BreadthFirstSearch(SimpleGraph<int> graph, int VFrom, int VTo, List<int> resultPath)
        {
            List<Vertex<int>> path = graph.BreadthFirstSearch(VFrom, VTo);

            path.Select(x => x.Value).ShouldBe(resultPath);
        }

        [Theory]
        [MemberData(nameof(GetFindAllCyclesData))]
        public void Should_FindAllCycles(SimpleGraph<int> graph, List<List<int>> result)
        {
            List<List<int>> cycles = graph.FindAllCycles();

            cycles.Count.ShouldBe(result.Count);
            for (int i = 0; i < cycles.Count; i++)
                cycles[i].ShouldBe(result[i]);
        }

        public static IEnumerable<object[]> GetFindAllCyclesData()
        {
            SimpleGraph<int> graph;
            List<List<int>> result;

            // 1. Проверка на границы в пустом графе
            graph = new SimpleGraph(0);
            result = new List<List<int>>(0);
            yield return new object[] { graph, result };

            // 2. Проверка на поиск в пустом графе
            graph = new SimpleGraph(6);
            result = new List<List<int>>(0);
            yield return new object[] { graph, result };

            // 3. Без циклов
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(1, 2);
            result = new List<List<int>>(0)
            {

            };
            yield return new object[] { graph, result };

            // 4. Два цикла
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            result = new List<List<int>>(0)
            {
                new List<int>() { 0, 2, 4 },
                new List<int>() { 0, 4, 1, 2 },
                new List<int>() { 1, 2, 4 },
            };
            yield return new object[] { graph, result };

            //  5.Цикл через весь граф
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(7);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(5, 0);
            result = new List<List<int>>(1)
            {
                new List<int>() { 0, 5, 4, 3, 2, 1 },
            };
            yield return new object[] { graph, result };

            // 6. Сложный граф с циклами, разветвлениями и тупиками
            graph = new SimpleGraph<int>(10);
            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(7);
            graph.AddVertex(8);
            graph.AddVertex(9);

            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(5, 0);
            graph.AddEdge(6, 7);
            graph.AddEdge(7, 8);
            graph.AddEdge(8, 6);
            graph.AddEdge(2, 6);
            graph.AddEdge(4, 7);
            graph.AddEdge(3, 5);
            graph.AddEdge(9, 3);
            graph.AddEdge(9, 9);

            result = new List<List<int>>(4)
            {      
                new List<int>() { 0, 1, 2, 3, 5 },
                new List<int>() { 5, 3, 4 },
                new List<int>() { 0, 1, 2, 6, 7, 4, 5 },       
                new List<int>() { 0, 5, 4, 7, 8, 6, 2, 1 },
                new List<int>() { 1, 2, 3, 4, 5, 0 },
                new List<int>() { 6, 7, 8 },
                new List<int>() { 2, 3, 4, 7, 6 },
                new List<int>() { 3, 4, 7, 8, 6, 2 },
                new List<int>() { 5, 4, 7, 6, 2, 3 },
                new List<int>() { 5, 3, 2, 6, 8, 7, 4 },   
            };
            yield return new object[] { graph, result };
        }

        public static IEnumerable<object[]> GetBreadthFirstSearchData()
        {
            SimpleGraph<int> graph;
            int vFrom;
            int vTo;
            List<int> resultPath;

            // 1. Проверка на границы в пустом графе
            graph = new SimpleGraph(0);
            vFrom = 0;
            vTo = 0;
            resultPath = new List<int>
            {

            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 2. Проверка на поиск отрицательных узлом
            graph = new SimpleGraph(6);
            vFrom = -1;
            vTo = -1;
            resultPath = new List<int>
            {

            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 3. Проверка на поиск узлов за границами (больше) вместимости
            graph = new SimpleGraph(6);
            vFrom = 6;
            vTo = 7;
            resultPath = new List<int>
            {

            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 4. Проверка на поиск в пустом графе
            graph = new SimpleGraph<int>(6);
            vFrom = 0;
            vTo = 2;
            resultPath = new List<int>
            {

            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 5. Проверка на поиск c несуществующим узлом
            graph = new SimpleGraph<int>(6);
            vFrom = 0;
            vTo = 2;
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            graph.RemoveVertex(2);
            resultPath = new List<int>
            {

            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 6. Простой поиск
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            vFrom = 0;
            vTo = 2;
            resultPath = new List<int>
            {
                2, 4
            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 7. Поиск через промежуточный узел
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 4);
            vFrom = 0;
            vTo = 2;
            resultPath = new List<int>
            {
                2, 6, 4
            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 8. Проверка на поиск с циклами
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(3, 4);
            graph.AddEdge(3, 0);
            graph.AddEdge(3, 2);
            vFrom = 0;
            vTo = 2;
            resultPath = new List<int>
            {
                2, 5, 4
            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 9. Проверка на отсутсвие пути с двумя циклами
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(7);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(3, 5);
            vFrom = 0;
            vTo = 4;
            resultPath = new List<int>
            {

            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 10. Два пути, проверка на оптимальный
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(7);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(1, 3);
            graph.AddEdge(4, 2);
            vFrom = 0;
            vTo = 5;
            resultPath = new List<int>
            {
                2, 3, 4, 6, 7
            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 11. Два пути, проверка на оптимальный
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(7);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(1, 3);
            vFrom = 0;
            vTo = 5;
            resultPath = new List<int>
            {
                2, 3, 5, 6, 7
            };
            yield return new object[] { graph, vFrom, vTo, resultPath };
        }
    }
}
