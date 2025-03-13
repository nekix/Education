using AlgorithmsDataStructures2;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Education.Ads.Tests.Exercise10
{
    public class DirectedGraph_Tests
    {
        [Theory]
        [MemberData(nameof(GetGetMaxSimplePathLengthData))]
        public void Should_GetMaxSimplePathLength(DirectedGraph graph, int result)
        {
            graph.GetMaxSimplePathLength().ShouldBe(result);
        }

        public static IEnumerable<object[]> GetGetMaxSimplePathLengthData()
        {
            DirectedGraph graph;
            int result;

            // 1: Пустой граф
            graph = new DirectedGraph(0);
            result = 0;
            yield return new object[] { graph, result };

            // 2: Один элемент
            graph = new DirectedGraph(1);
            graph.AddVertex(10);
            graph.AddEdge(0, 0);
            result = 0;
            yield return new object[] { graph, result };

            // 3: Два элемента, без ребёр
            graph = new DirectedGraph(2);
            graph.AddVertex(10);
            graph.AddVertex(11);
            result = 0;
            yield return new object[] { graph, result };

            // 4: Два элемента, с ребром
            graph = new DirectedGraph(2);
            graph.AddVertex(10);
            graph.AddVertex(11);
            graph.AddEdge(0, 1);
            result = 1;
            yield return new object[] { graph, result };

            // 5: Два элемента, с ребром в обратном порядке
            graph = new DirectedGraph(2);
            graph.AddVertex(10);
            graph.AddVertex(11);
            graph.AddEdge(1, 0);
            result = 1;
            yield return new object[] { graph, result };

            // 6: Два элемента, с ребрами
            graph = new DirectedGraph(2);
            graph.AddVertex(10);
            graph.AddVertex(11);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 0);
            result = 1;
            yield return new object[] { graph, result };

            // 7: Граф с циклами
            graph = new DirectedGraph(6);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 0);
            graph.AddEdge(2, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(5, 1);
            result = 5;
            yield return new object[] { graph, result };

            // 8: Граф с вершинами без исходящих рёбер
            graph = new DirectedGraph(6);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 2);
            graph.AddEdge(2, 1);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 1);
            result = 4;
            yield return new object[] { graph, result };

            // 9: Граф с объеденением нескольких случаев
            graph = new DirectedGraph(10);
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

            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);
            graph.AddEdge(5, 6);
            graph.AddEdge(6, 7);
            graph.AddEdge(7, 8);
            graph.AddEdge(8, 9);

            // (альтернативный путь)
            graph.AddEdge(2, 6); 
            // (альтернативный путь)
            graph.AddEdge(4, 8);

            // (циклический путь)
            graph.AddEdge(6, 2);
            // (тупиковая путь)
            graph.AddEdge(7, 10);
            result = 9;
            yield return new object[] { graph, result };
        }
    }
}
