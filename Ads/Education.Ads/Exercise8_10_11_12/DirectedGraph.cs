using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
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
}
