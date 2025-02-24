using AlgorithmsDataStructures2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Education.Ads.Exercise8;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise8
{
    public class DirectedGraph_Tests
    {
        [Theory]
        [MemberData(nameof(GetCtorGraphData))]
        public void Should_Ctor_Graph(int size)
        {
            DirectedGraph graph = new DirectedGraph(size);

            graph.vertex.ShouldNotBeNull();
            graph.vertex.Length.ShouldBe(size);
            graph.vertex.ShouldAllBe(v => v == null);
            graph.m_adjacency.GetLength(0).ShouldBe(size);
            graph.m_adjacency.GetLength(1).ShouldBe(size);
            foreach (var adj in graph.m_adjacency)
                adj.ShouldBe(0);
            graph.max_vertex.ShouldBe(size);

        }

        [Theory]
        [MemberData(nameof(GetAddVertexData))]
        public void Should_AddVertex(DirectedGraph graph, int vertex, int?[] vertexRes, int[,] m_adjacencyRes)
        {
            graph.AddVertex(vertex);

            graph.vertex.Select(x => x?.Value).ShouldBe(vertexRes);
            graph.m_adjacency.ShouldBe(m_adjacencyRes);
        }

        [Theory]
        [MemberData(nameof(GetAddEdgeData))]
        public void Should_AddEdge(DirectedGraph graph, int v1, int v2, int[,] m_adjacency)
        {
            graph.AddEdge(v1, v2);

            graph.m_adjacency.ShouldBe(m_adjacency);
        }

        [Theory]
        [MemberData(nameof(GetRemoveVertexData))]
        public void Should_RemoveVertex(DirectedGraph graph, int v, int?[] vertexRes, int[,] m_adjacencyRes)
        {
            graph.RemoveVertex(v);

            graph.vertex.Select(x => x?.Value).ShouldBe(vertexRes);
            graph.m_adjacency.ShouldBe(m_adjacencyRes);
        }

        [Theory]
        [MemberData(nameof(GetRemoveEdgeData))]
        public void Should_RemoveEdge(DirectedGraph graph, int v1, int v2, int?[] vertexRes, int[,] m_adjacencyRes)
        {
            graph.RemoveEdge(v1, v2);

            graph.vertex.Select(x => x?.Value).ShouldBe(vertexRes);
            graph.m_adjacency.ShouldBe(m_adjacencyRes);
        }

        [Theory]
        [MemberData(nameof(GetIsEdgeData))]
        public void Should_IsEdge(DirectedGraph graph, int v1, int v2, bool res)
        {
            graph.IsEdge(v1, v2).ShouldBe(res);
        }

        public static IEnumerable<object[]> GetIsEdgeData()
        {
            // 1: Пустой
            DirectedGraph graph = new DirectedGraph(0);
            int v1 = 0;
            int v2 = 0;
            yield return new object[] { graph, v1, v2, false };

            // 2: Один элемент
            graph = new DirectedGraph(1);
            graph.AddVertex(10);
            graph.AddEdge(0, 0);
            v1 = 0;
            v2 = 0;
            yield return new object[] { graph, v1, v2, true };

            // 3: За пределы индекса
            graph = new DirectedGraph(1);
            graph.AddVertex(10);
            v1 = 1;
            v2 = 0;
            yield return new object[] { graph, v1, v2, false };

            // 4: Четыре элемента но не тот порядок
            graph = new DirectedGraph(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            graph.RemoveVertex(2);
            v1 = 4;
            v2 = 1;
            yield return new object[] { graph, v1, v2, false };

            // 5: Четыре элемента (тот порядок)
            graph = new DirectedGraph(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            graph.RemoveVertex(2);
            v1 = 1;
            v2 = 4;
            yield return new object[] { graph, v1, v2, true };

            // 6: Нет связи
            graph = new DirectedGraph(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            graph.RemoveVertex(2);
            v1 = 0;
            v2 = 1;
            yield return new object[] { graph, v1, v2, false };
        }

        public static IEnumerable<object[]> GetRemoveEdgeData()
        {
            // 1: Пустой
            DirectedGraph graph = new DirectedGraph(0);
            int v1 = 0;
            int v2 = 0;
            int?[] m_vertex = Array.Empty<int?>();
            int[,] m_adjacency = new int[0, 0];
            yield return new object[] { graph, v1, v2, m_vertex, m_adjacency };

            // 2: Один элемент
            graph = new DirectedGraph(1);
            graph.AddVertex(10);
            v1 = 0;
            v2 = 0;
            m_vertex = new int?[] { 10 };
            m_adjacency = new int[,] { { 0 } };
            yield return new object[] { graph, v1, v2, m_vertex, m_adjacency };

            // 3: Удаление несуществующих за пределы индекса
            graph = new DirectedGraph(1);
            graph.AddVertex(10);
            v1 = 1;
            v2 = 0;
            m_vertex = new int?[] { 10 };
            m_adjacency = new int[,] { { 0 } };
            yield return new object[] { graph, v1, v2, m_vertex, m_adjacency };

            // 4: Четыре элемента (не то направление)
            graph = new DirectedGraph(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            graph.RemoveVertex(2);
            v1 = 4;
            v2 = 0;
            m_vertex = new int?[] { 2, 3, null, 5, 6, null };
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, v1, v2, m_vertex, m_adjacency };

            // 5: Четыре элемента (то направление)
            graph = new DirectedGraph(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            graph.RemoveVertex(2);
            v1 = 0;
            v2 = 4;
            m_vertex = new int?[] { 2, 3, null, 5, 6, null };
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, v1, v2, m_vertex, m_adjacency };
        }

        public static IEnumerable<object[]> GetRemoveVertexData()
        {
            // 1: Пустой
            DirectedGraph graph = new DirectedGraph(0);
            int v = 0;
            int?[] m_vertex = new int?[0];
            int[,] m_adjacency = new int[0, 0];
            yield return new object[] { graph, v, m_vertex, m_adjacency };

            // 2: Один элемент
            graph = new DirectedGraph(1);
            graph.AddVertex(10);
            v = 0;
            m_vertex = new int?[] { null };
            m_adjacency = new int[,] { { 0 } };
            yield return new object[] { graph, v, m_vertex, m_adjacency };

            // 3: Удаление несуществующего элемента
            graph = new DirectedGraph(1);
            graph.AddVertex(10);
            v = 1;
            m_vertex = new int?[] { 10 };
            m_adjacency = new int[,] { { 0 } };
            yield return new object[] { graph, v, m_vertex, m_adjacency };

            // 4: Три элемента подряд
            graph = new DirectedGraph(6);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(10);
            graph.AddEdge(0, 2);
            graph.AddEdge(1, 2);
            graph.AddEdge(3, 1);
            v = 1;
            m_vertex = new int?[] { 5, null, 10, null, null, null };
            m_adjacency = new int[,]
            {
                { 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, v, m_vertex, m_adjacency };

            // 5: Пять элементов подряд
            graph = new DirectedGraph(6);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddEdge(4, 0);
            graph.AddEdge(1, 4);
            graph.AddEdge(3, 2);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 0);
            v = 2;
            m_vertex = new int?[] { 2, 3, null, 5, 6, null };
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, v, m_vertex, m_adjacency };
        }

        public static IEnumerable<object[]> GetAddEdgeData()
        {
            // 1: Пустой
            DirectedGraph graph = new DirectedGraph(0);
            int v1 = 0;
            int v2 = 0;
            int[,] m_adjacency = new int[0, 0];
            yield return new object[] { graph, v1, v2, m_adjacency };

            // 2: Без вершин
            graph = new DirectedGraph(1);
            v1 = 0;
            v2 = 0;
            m_adjacency = new int[,] { { 0 } };
            yield return new object[] { graph, v1, v2, m_adjacency };

            // 3: Между двумя вершинами
            graph = new DirectedGraph(6);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(10);
            v1 = 0;
            v2 = 2;
            m_adjacency = new int[,]
            {
                { 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, v1, v2, m_adjacency };

            // 4: Одна несуществующая вершина
            graph = new DirectedGraph(6);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(10);
            v1 = 1;
            v2 = 5;
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, v1, v2, m_adjacency };

            // 5: Вершины из середины
            // [ null, null, 5, 6, 10, null ]
            graph = new DirectedGraph(6);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(10);
            graph.RemoveVertex(2);
            graph.RemoveVertex(1);
            v1 = 3;
            v2 = 4;
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, v1, v2, m_adjacency };
        }

        public static IEnumerable<object[]> GetAddVertexData()
        {
            // 1: Пустой
            DirectedGraph graph = new DirectedGraph(0);
            int vertex = 10;
            int?[] m_vertex = new int?[0];
            int[,] m_adjacency = new int[0, 0];
            yield return new object[] { graph, vertex, m_vertex, m_adjacency };

            // 2: Один элемент
            graph = new DirectedGraph(1);
            vertex = 10;
            m_vertex = new int?[] { 10 };
            m_adjacency = new int[,] { { 0 } };
            yield return new object[] { graph, vertex, m_vertex, m_adjacency };

            // 3: Три элемента подряд
            graph = new DirectedGraph(6);
            graph.vertex[0] = new Vertex(5);
            graph.vertex[1] = new Vertex(6);
            vertex = 10;
            m_vertex = new int?[] { 5, 6, 10, null, null, null };
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, vertex, m_vertex, m_adjacency };

            // 4: Вставка в начало при частичном заполнении с середины
            graph = new DirectedGraph(6);
            graph.vertex[2] = new Vertex(5);
            graph.vertex[3] = new Vertex(6);
            vertex = 10;
            m_vertex = new int?[] { 10, null, 5, 6, null, null };
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, vertex, m_vertex, m_adjacency };

            // 5: Вставка в начало при частичном заполнении с конца
            graph = new DirectedGraph(6);
            graph.vertex[4] = new Vertex(5);
            graph.vertex[5] = new Vertex(6);
            vertex = 10;
            m_vertex = new int?[] { 10, null, null, null, 5, 6 };
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, vertex, m_vertex, m_adjacency };

            // 5: Три элемента подряд с пропуском первых двух
            graph = new DirectedGraph(6);
            graph.vertex[0] = new Vertex(5);
            graph.vertex[1] = new Vertex(6);
            graph.vertex[2] = new Vertex(7);
            graph.vertex[3] = new Vertex(8);
            graph.vertex[4] = new Vertex(9);
            graph.vertex[5] = new Vertex(11);
            vertex = 10;
            m_vertex = new int?[] { 5, 6, 7, 8, 9, 11 };
            m_adjacency = new int[,]
            {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };
            yield return new object[] { graph, vertex, m_vertex, m_adjacency };
        }

        public static IEnumerable<object[]> GetCtorGraphData()
        {
            // 1
            int size = 0;
            yield return new object[] { size };

            // 2
            size = 1;
            yield return new object[] { size };

            // 3
            size = 2;
            yield return new object[] { size };

            // 4
            size = 5;
            yield return new object[] { size };

            // 5
            size = 10;
            yield return new object[] { size };
        }
    }
}
