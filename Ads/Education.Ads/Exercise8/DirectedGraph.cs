using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmsDataStructures2;

namespace Education.Ads.Exercise8
{
    public class DirectedGraph : SimpleGraph
    {
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
    }
}
