using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace AlgorithmsDataStructures2
{
    public class Vertex<T>
    {
        public bool Hit;
        public T Value;
        public Vertex(T val)
        {
            Value = val;
            Hit = false;
        }
    }

    public class Vertex : Vertex<int>
    {
        public Vertex(int val) : base(val)
        {
        }
    }

    public partial class SimpleGraph<T>
    {
        public Vertex<T>[] vertex;
        public int[,] m_adjacency;
        public int max_vertex;

        public SimpleGraph(int size)
        {
            max_vertex = size;
            m_adjacency = new int[size, size];
            vertex = new Vertex<T>[size];
        }

        public void AddVertex(T value)
        {
            int slot = Array.IndexOf(vertex, null);

            if (slot != -1)
                vertex[slot] = new Vertex<T>(value);
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

        public List<Vertex<T>> DepthFirstSearch(int VFrom, int VTo)
        {
            if (!IsInRange(VFrom) || !IsInRange(VTo))
                return new List<Vertex<T>>(0);

            if (vertex[VFrom] == null || vertex[VTo] == null)
                return new List<Vertex<T>>(0);

            // 0: Cleared
            Stack<int> path = new Stack<int>();

            ResetHits();

            // 1: Select the VFrom vertex
            int current = VFrom;
            while (path.Count != 0 || !vertex[current].Hit)
            {
                // 4.2.2 No unvisited vertices left
                // Go to step 5.
                if (vertex[current].Hit)
                {
                    // 5. Remove the top node from the stack
                    path.Pop();

                    // 5.1 Stack is empty, exit
                    if (path.Count == 0)
                        break;

                    // 5.2 Take the new top node as the VFrom one
                    current = path.Peek();
                }
                else
                {
                    // 2: Mark as visited
                    vertex[current].Hit = true;

                    // 3: Put on the stack
                    path.Push(current);

                    // 4: Check for the presence of the target vertex
                    if (IsEdge(current, VTo))
                    {
                        // 4.1 Return the result
                        path.Push(VTo);
                        break;
                    }
                }

                // 4.2 Find the next unvisited vertex
                for (int i = 0; i < vertex.Length; i++)
                {
                    if (IsEdge(current, i) && vertex[i].Hit == false)
                    {
                        // 4.2.1 Take this as the VFrom
                        // and go to step 2
                        current = i;
                        break;
                    }
                }
            }

            List<Vertex<T>> vertexPath = StackReversePathToVertexList(path);

            return vertexPath;
        }

        public List<Vertex<T>> DepthFirstSearchRecursive(int VFrom, int VTo)
        {
            if (!IsInRange(VFrom) || !IsInRange(VTo))
                return new List<Vertex<T>>(0);

            if (vertex[VFrom] == null || vertex[VTo] == null)
                return new List<Vertex<T>>(0);

            // 0 Cleared
            Stack<int> path = new Stack<int>();

            ResetHits();

            // 1: Select the VFrom vertex
            if (TryDepthFirstSearchRecursive(VFrom, VTo, path))
                return StackReversePathToVertexList(path);

            return new List<Vertex<T>>(0);
        }

        protected bool TryDepthFirstSearchRecursive(int VFrom, int VTo, Stack<int> path)
        {
            // 2 Mark as visited
            vertex[VFrom].Hit = true;

            // 3 Put on the stack
            path.Push(VFrom);

            // 4 Check for the presence of the target vertex
            if (IsEdge(VFrom, VTo))
            {
                // 4.1 Return the result
                path.Push(VTo);
                return true;
            }

            // 4.2 Find the next unvisited vertex
            for (int i = 0; i < vertex.Length; i++)
            {
                if (IsEdge(VFrom, i) && !vertex[i].Hit)
                {
                    // 4.2.1 Take this as the VFrom
                    // and go to step 2
                    if (TryDepthFirstSearchRecursive(i, VTo, path))
                        return true;
                }
            }

            // 4.2.2 No unvisited vertices left
            // 5. Remove the top node from the stack
            path.Pop();

            return false;
        } 

        protected bool IsInRange(int index)
        {
            if (index < 0 || index >= vertex.Length)
                return false;

            return true;
        }

        private List<Vertex<T>> StackReversePathToVertexList(Stack<int> path)
        {
            List<Vertex<T>> result = new List<Vertex<T>>(path.Count);
            while (path.Count != 0)
            {
                int index = path.Pop();
                result.Add(vertex[index]);
            }

            ReverseList(result);

            return result;
        }

        private void ReverseList(List<Vertex<T>> result)
        {
            for (int i = 0, j = result.Count - 1; i < j; i++, j--)
                (result[i], result[j]) = (result[j], result[i]);
        }

        public List<Vertex<T>> BreadthFirstSearch(int VFrom, int VTo)
        {
            if (!IsInRange(VFrom) || !IsInRange(VTo))
                return new List<Vertex<T>>(0);

            if (vertex[VFrom] == null || vertex[VTo] == null)
                return new List<Vertex<T>>(0);

            ResetHits();

            Queue<int> nextVertexes = new Queue<int>();
            int[] preVertex = new int[vertex.Length];

            preVertex[VFrom] = -1;
            preVertex[VTo] = -1;

            nextVertexes.Enqueue(VFrom);
            vertex[VFrom].Hit = true;

            while (nextVertexes.Count != 0)
            {
                int current = nextVertexes.Dequeue();

                if (IsEdge(current, VTo))
                {
                    vertex[VTo].Hit = true;
                    preVertex[VTo] = current;
                    break;
                }

                for (int i = 0; i < vertex.Length; i++)
                {
                    if (current == i)
                        continue;

                    if (IsEdge(current, i) && !vertex[i].Hit)
                    {
                        nextVertexes.Enqueue(i);
                        vertex[i].Hit = true;
                        preVertex[i] = current;
                    }
                }
            }

            if (preVertex[VTo] == -1)
                return new List<Vertex<T>>(0);

            return RecoverBreadthFirstSearchPath(preVertex, VTo, VTo);
        }

        private void ResetHits()
        {
            foreach (Vertex<T> item in vertex)
                if (item != null)
                    item.Hit = false;
        }

        private List<Vertex<T>> RecoverBreadthFirstSearchPath(int[] preVertex, int VFrom, int VTo)
        {
            Stack<Vertex<T>> revertPath = new Stack<Vertex<T>>();

            for (int i = VTo; i != -1; i = preVertex[i])
                revertPath.Push(vertex[i]);

            return new List<Vertex<T>>(revertPath);
        }

        public List<Vertex<T>> WeakVertices()
        {
            List<Vertex<T>> weakVertices = new List<Vertex<T>>(vertex.Length);

            ResetHits();

            for (int i = 0; i < vertex.Length; i++)
            {
                if (vertex[i] == null)
                    continue;

                if (vertex[i].Hit)
                    continue;

                if (CheckIsWeak(i))
                    weakVertices.Add(vertex[i]);
            }

            return weakVertices;
        }

        private bool CheckIsWeak(int v)
        {
            bool isWeak = true;

            for (int i = v + 1; i < vertex.Length; i++)
            {
                if (vertex[i] == null)
                    continue;

                if (!IsEdge(v, i))
                    continue;

                for (int j = i + 1; j < vertex.Length; j++)
                {
                    if (vertex[j] == null)
                        continue;

                    if (IsEdge(v, j) && IsEdge(i, j))
                    {
                        vertex[i].Hit = true;
                        vertex[j].Hit = true;
                        isWeak = false;
                    }
                }
            }

            return isWeak;
        }
    }

    public class SimpleGraph : SimpleGraph<int>
    {
        public SimpleGraph(int size) : base(size)
        {
        }
    }
}