using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmsDataStructures2
{
    public class SimpleTreeNode<T>
    {
        public T NodeValue; // значение в узле
        public SimpleTreeNode<T> Parent; // родитель или null для корня
        public List<SimpleTreeNode<T>> Children; // список дочерних узлов или null

        public SimpleTreeNode(T val, SimpleTreeNode<T> parent)
        {
            NodeValue = val;
            Parent = parent;
            Children = null;
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

            if(ParentNode.Children == null)
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

            foreach (SimpleTreeNode<T> node in nodes)
                if (node.Children != null)
                    nodes.AddRange(node.Children);

            return nodes;
        }

        public List<SimpleTreeNode<T>> FindNodesByValue(T val)
        {
            Stack<SimpleTreeNode<T>> nodesStack = new Stack<SimpleTreeNode<T>>();
            List<SimpleTreeNode<T>> results = new List<SimpleTreeNode<T>>();

            nodesStack.Push(Root);

            int stackCount = 1;

            while (stackCount != 0)
            {
                SimpleTreeNode<T> currentNode = nodesStack.Pop();
                stackCount--;

                if (currentNode.Equals(val))
                    results.Add(currentNode);

                if (currentNode.Children == null) continue;

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                {
                    nodesStack.Push(child);
                    stackCount++;
                }
            }

            return results;
        }

        public void MoveNode(SimpleTreeNode<T> OriginalNode, SimpleTreeNode<T> NewParent)
        {
            // В предположении, что OriginalNode принадлежит данному дереву.

            NewParent.Parent = OriginalNode.Parent;
            NewParent.Children.Add(OriginalNode);

            OriginalNode.Parent.Children.Remove(OriginalNode);
            OriginalNode.Parent.Children.Add(NewParent);
            
            OriginalNode.Parent = NewParent;
        }

        public int Count()
        {
            Stack<SimpleTreeNode<T>> nodes = new Stack<SimpleTreeNode<T>>();

            nodes.Push(Root);

            int nodesCount = 1;
            int stackCount = 1;

            while (stackCount != 0)
            {
                SimpleTreeNode<T> currentNode = nodes.Pop();
                stackCount--;

                if (currentNode.Children == null) continue;

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                {
                    nodes.Push(child);
                    stackCount++;
                    nodesCount++;
                }
            }

            return nodesCount;
        }

        public int LeafCount()
        {
            Stack<SimpleTreeNode<T>> nodes = new Stack<SimpleTreeNode<T>>();

            nodes.Push(Root);

            int leafCount = 1;
            int stackCount = 1;

            while (stackCount != 0)
            {
                SimpleTreeNode<T> currentNode = nodes.Pop();
                stackCount--;

                if (currentNode.Children == null)
                {
                    leafCount++;
                    continue;
                }

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                {
                    nodes.Push(child);
                    stackCount++;
                }
            }

            return leafCount;
        }

    }

}
