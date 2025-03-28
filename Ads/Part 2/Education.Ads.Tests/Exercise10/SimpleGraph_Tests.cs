using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmsDataStructures2;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise10
{
    public class SimpleGraph_Tests
    {
        [Theory]
        [MemberData(nameof(GetDepthFirstSearchData))]
        public void Should_DepthFirstSearch(SimpleGraph<int> graph, int VFrom, int VTo, List<int> resultPath)
        {
            List<Vertex<int>> path = graph.DepthFirstSearch(VFrom, VTo);

            path.Select(x => x.Value).ShouldBe(resultPath);
        }

        [Theory]
        [MemberData(nameof(GetDepthFirstSearchData))]
        public void Should_DepthFirstSearchRecursive(SimpleGraph<int> graph, int VFrom, int VTo, List<int> resultPath)
        {
            List<Vertex<int>> path = graph.DepthFirstSearchRecursive(VFrom, VTo);

            path.Select(x => x.Value).ShouldBe(resultPath);
        }

        public static IEnumerable<object[]> GetDepthFirstSearchData()
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
            //graph.AddEdge(2, 0);
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
            //graph.AddEdge(2, 0);
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
            //graph.AddEdge(2, 0);
            vFrom = 0;
            vTo = 4;
            resultPath = new List<int>
            {
                
            };
            yield return new object[] { graph, vFrom, vTo, resultPath };

            // 10. Длинный путь
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
            //graph.AddEdge(2, 0);
            vFrom = 0;
            vTo = 5;
            resultPath = new List<int>
            {
                2, 3, 4, 5, 6, 7
            };
            yield return new object[] { graph, vFrom, vTo, resultPath };
        }
    }
}
