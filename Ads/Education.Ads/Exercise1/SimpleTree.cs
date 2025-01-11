using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class SimpleTreeNode<T>
    {
        public T NodeValue; // значение в узле
        public SimpleTreeNode<T> Parent; // родитель или null для корня
        public List<SimpleTreeNode<T>> Children; // список дочерних узлов или null
        public int Level;

        public SimpleTreeNode(T val, SimpleTreeNode<T> parent)
        {
            NodeValue = val;
            Parent = parent;
            Children = null;
            Level = -1;
        }
    }

    public class SimpleTree<T>
    {
        public SimpleTreeNode<T> Root; // корень, может быть null

        public SimpleTree(SimpleTreeNode<T> root)
        {
            Root = root;
        }

        public void AddChild(SimpleTreeNode<T> ParentNode, SimpleTreeNode<T> NewChild)
        {
            // В предположении, что ParentNode принадлежит данному дереву.

            if (ParentNode.Children == null)
                ParentNode.Children = new List<SimpleTreeNode<T>>();

            ParentNode.Children.Add(NewChild);
            NewChild.Parent = ParentNode;
        }

        public void DeleteNode(SimpleTreeNode<T> NodeToDelete)
        {
            // В предположении, что NodeToDelete принадлежит данному дереву.

            if (NodeToDelete == Root)
                return;

            NodeToDelete.Parent.Children.Remove(NodeToDelete);
            NodeToDelete.Parent = null;
        }

        public List<SimpleTreeNode<T>> GetAllNodes()
        {
            List<SimpleTreeNode<T>> nodes = new List<SimpleTreeNode<T>>();

            nodes.Add(Root);

            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].Children != null)
                    nodes.AddRange(nodes[i].Children);

            return nodes;
        }

        public List<SimpleTreeNode<T>> FindNodesByValue(T val)
        {
            Stack<SimpleTreeNode<T>> nodesStack = new Stack<SimpleTreeNode<T>>();
            List<SimpleTreeNode<T>> results = new List<SimpleTreeNode<T>>();

            nodesStack.Push(Root);

            while (nodesStack.Count != 0)
            {
                SimpleTreeNode<T> currentNode = nodesStack.Pop();

                if (currentNode.NodeValue.Equals(val))
                    results.Add(currentNode);

                if (currentNode.Children == null) continue;

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                {
                    nodesStack.Push(child);
                }
            }

            return results;
        }

        public void MoveNode(SimpleTreeNode<T> OriginalNode, SimpleTreeNode<T> NewParent)
        {
            // В предположении, что OriginalNode принадлежит данному дереву.

            if (OriginalNode == Root)
                return;

            OriginalNode.Parent.Children.Remove(OriginalNode);
            OriginalNode.Parent = NewParent;

            if (NewParent.Children == null)
                NewParent.Children = new List<SimpleTreeNode<T>>();
            NewParent.Children.Add(OriginalNode);
        }

        public int Count()
        {
            Stack<SimpleTreeNode<T>> nodes = new Stack<SimpleTreeNode<T>>();

            nodes.Push(Root);

            int nodesCount = 1;

            while (nodes.Count != 0)
            {
                SimpleTreeNode<T> currentNode = nodes.Pop();

                if (currentNode.Children == null) continue;

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                {
                    nodes.Push(child);
                    nodesCount++;
                }
            }

            return nodesCount;
        }

        public int LeafCount()
        {
            Stack<SimpleTreeNode<T>> nodes = new Stack<SimpleTreeNode<T>>();

            nodes.Push(Root);

            int leafCount = 0;

            while (nodes.Count != 0)
            {
                SimpleTreeNode<T> currentNode = nodes.Pop();

                if (currentNode.Children == null)
                {
                    leafCount++;
                    continue;
                }

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                {
                    nodes.Push(child);
                }
            }

            return leafCount;
        }

    }

}
