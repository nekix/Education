using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class Vertex
    {
        public int Value;
        public Vertex(int val)
        {
            Value = val;
        }
    }

    public class SimpleGraph
    {
        public Vertex[] vertex;
        public int[,] m_adjacency;
        public int max_vertex;

        public SimpleGraph(int size)
        {
            max_vertex = size;
            m_adjacency = new int[size, size];
            vertex = new Vertex[size];
        }

        public void AddVertex(int value)
        {
            int slot = Array.IndexOf(vertex, null);

            if (slot != -1)
                vertex[slot] = new Vertex(value);
        }
        
        public void RemoveVertex(int v)
        {
            if (!IsInRange(v))
                return;

            for (int i = 0; i < max_vertex; i++)
            {
                m_adjacency[v, i] = 0;
                m_adjacency[i, v] = 0;
            }

            vertex[v] = null;
        }

        public bool IsEdge(int v1, int v2)
        {
            if (!(IsInRange(v1) && IsInRange(v2)))
                return false;

            if (vertex[v1] == null || vertex[v2] == null)
                return false;

            return m_adjacency[v1, v2] == 1;
        }

        public virtual void AddEdge(int v1, int v2)
        {
            if (!(IsInRange(v1) && IsInRange(v2)))
                return;

            if (vertex[v1] == null || vertex[v2] == null)
                return;

            m_adjacency[v1, v2] = 1;
            m_adjacency[v2, v1] = 1;
        }

        public virtual void RemoveEdge(int v1, int v2)
        {
            if (!(IsInRange(v1) && IsInRange(v2)))
                return;

            m_adjacency[v1, v2] = 0;
            m_adjacency[v2, v1] = 0;
        }

        protected bool IsInRange(int index)
        {
            if (index < 0 || index >= m_adjacency.Length)
                return false;

            return true;
        }
    }
}