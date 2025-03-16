using AlgorithmsDataStructures2;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Education.Ads.Tests.Exercise12
{
    public class SimpleGraphExtensions_Tests
    {
        [Theory]
        [MemberData(nameof(GetWeakVerticesV2Data))]
        public void Should_WeakVerticesV2(SimpleGraph<int> graph, List<int> weakVerticesValues)
        {
            graph.WeakVerticesV2().Select(v => v.Value).ShouldBe(weakVerticesValues);
        }

        public static IEnumerable<object[]> GetWeakVerticesV2Data()
        {
            SimpleGraph<int> graph;
            List<int> weakVerticesValues;

            // 1. Пустой граф нулевой вместимости
            graph = new SimpleGraph<int>(0);
            weakVerticesValues = new List<int>(0);
            yield return new object[] { graph, weakVerticesValues };

            // 2 Пустой граф вместимости 6
            graph = new SimpleGraph<int>(6);
            weakVerticesValues = new List<int>(0);
            yield return new object[] { graph, weakVerticesValues };

            // 3 Граф без треугольников
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 0);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            weakVerticesValues = new List<int>() { 1, 2, 3, 4, 5, 6 };
            yield return new object[] { graph, weakVerticesValues };

            // 4 Граф без треугольников с пропущенными узлами
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.RemoveVertex(1);
            graph.RemoveVertex(4);
            graph.AddEdge(0, 0);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            weakVerticesValues = new List<int>() { 1, 3, 4, 6 };
            yield return new object[] { graph, weakVerticesValues };

            // 5. Граф с одним треугольником
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            weakVerticesValues = new List<int> { 4, 5, 6 };
            yield return new object[] { graph, weakVerticesValues };

            // 6. Граф с двумя треугольниками
            graph = new SimpleGraph<int>(6);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(5, 3);
            weakVerticesValues = new List<int>();
            yield return new object[] { graph, weakVerticesValues };

            // 7. Все вершины в треугольниках
            graph = new SimpleGraph<int>(4);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 0);
            graph.AddEdge(0, 2);
            graph.AddEdge(1, 3);
            weakVerticesValues = new List<int>();
            yield return new object[] { graph, weakVerticesValues };

            // 8. Сложный граф
            graph = new SimpleGraph<int>(12);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(7);
            graph.AddVertex(8);
            graph.AddVertex(9);
            graph.AddVertex(10);
            graph.AddVertex(11);
            graph.AddVertex(12);
            // Треугольник 1 (0,1,2)
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            // Треугольник 2 (3,4,5)
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(5, 3);
            // Цепочка из четырёх узлов без треугольников (6-7-8-9)
            graph.AddEdge(6, 7);
            graph.AddEdge(7, 8);
            graph.AddEdge(8, 9);
            // Одинокие вершины (10, 11)
            graph.AddVertex(10);
            graph.AddVertex(11);
            weakVerticesValues = new List<int> { 7, 8, 9, 10, 11, 12 };
            yield return new object[] { graph, weakVerticesValues };
        }
    }
}
