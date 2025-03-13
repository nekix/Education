using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public partial class SimpleTreeNode<T>
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

            Level = parent != null 
                ? parent.Level + 1
                : -1;
        }
    }

    public partial class SimpleTree<T>
    {
        public SimpleTreeNode<T> Root; // корень, может быть null

        public SimpleTree(SimpleTreeNode<T> root)
        {
            Root = root;

            if(Root != null)
                UpdateNodesLevelsRecursive(Root, 0);
        }

        public void AddChild(SimpleTreeNode<T> ParentNode, SimpleTreeNode<T> NewChild)
        {
            // В предположении, что ParentNode принадлежит данному дереву.

            if (ParentNode.Children == null)
                ParentNode.Children = new List<SimpleTreeNode<T>>();

            ParentNode.Children.Add(NewChild);
            NewChild.Parent = ParentNode;

            UpdateNodesLevelsRecursive(NewChild, ParentNode.Level + 1);
        }

        public void DeleteNode(SimpleTreeNode<T> NodeToDelete)
        {
            // В предположении, что NodeToDelete принадлежит данному дереву.

            if (NodeToDelete == Root)
            {
                Root = null;
                return;
            }

            NodeToDelete.Parent.Children.Remove(NodeToDelete);
            NodeToDelete.Parent = null;
        }

        public List<SimpleTreeNode<T>> GetAllNodes()
        {
            List<SimpleTreeNode<T>> nodes = new List<SimpleTreeNode<T>>();
            
            if (Root != null)
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

            if (Root != null)
                nodesStack.Push(Root);

            while (nodesStack.Count != 0)
            {
                SimpleTreeNode<T> currentNode = nodesStack.Pop();

                if (currentNode.NodeValue.Equals(val))
                    results.Add(currentNode);

                if (currentNode.Children == null) continue;

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                    nodesStack.Push(child);
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

            UpdateNodesLevelsRecursive(OriginalNode, NewParent.Level + 1);
        }

        public int Count()
        {
            if (Root == null)
                return 0;

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
            if (Root == null)
                return 0;

            Stack<SimpleTreeNode<T>> nodes = new Stack<SimpleTreeNode<T>>();

            nodes.Push(Root);

            int leafCount = 0;

            while (nodes.Count != 0)
            {
                SimpleTreeNode<T> currentNode = nodes.Pop();

                if (currentNode.Children == null || currentNode.Children.Count == 0)
                {
                    leafCount++;
                    continue;
                }

                foreach (SimpleTreeNode<T> child in currentNode.Children)
                    nodes.Push(child);
            }

            return leafCount;
        }

        public SimpleTree<T> UpdateNodesLevelsRecursive()
        {
            if (Root != null)
                UpdateNodesLevelsRecursive(Root, 0);

            return this;
        }

        private void UpdateNodesLevelsRecursive(SimpleTreeNode<T> node, int nodeLevel)
        {
            node.Level = nodeLevel;

            if (node.Children == null)
                return;

            foreach (SimpleTreeNode<T> child in node.Children)
                UpdateNodesLevelsRecursive(child, nodeLevel + 1);
        }

        public SimpleTree<T> UpdateNodesLevelsIterative()
        {
            Queue<SimpleTreeNode<T>> nodesQueue = new Queue<SimpleTreeNode<T>>();

            nodesQueue.Enqueue(Root);

            int level = 0;
            int nodesLevelCount = 1;

            while (nodesQueue.Count != 0)
            {
                SimpleTreeNode<T> currentNode = nodesQueue.Dequeue();

                currentNode.Level = level;

                nodesLevelCount--;

                if (currentNode.Children != null)
                {
                    foreach (SimpleTreeNode<T> child in currentNode.Children)
                    {
                        nodesQueue.Enqueue(child);
                    }
                }

                if (nodesLevelCount == 0)
                {
                    level++;
                    nodesLevelCount = nodesQueue.Count;
                }
            }

            return this;
        }

        public bool IsSymmetricallyRecursive()
        {
            if (Root == null)
                return true;

            return IsSymmetricallyRecursive(Root.Children);
        }

        private bool IsSymmetricallyRecursive(List<SimpleTreeNode<T>> children)
        {
            if(children == null || children.Count == 0)
                return true;

            for (int i = 0, j = children.Count - 1; i < j; i++, j--)
            {
                if (!children[i].NodeValue.Equals(children[j].NodeValue))
                    return false;

                // Ветви зеркально расходятся
                if (!IsSymmetricallyRecursive(children[i].Children, children[j].Children))
                    return false;
            }

            // Средний ветвь (если есть)
            if (children.Count % 2 != 0)
                return IsSymmetricallyRecursive(children[children.Count / 2].Children);

            return true;
        }

        private bool IsSymmetricallyRecursive(List<SimpleTreeNode<T>> leftChildren, List<SimpleTreeNode<T>> rightChildren)
        {
            if (leftChildren == null && rightChildren == null)
                return true;

            if (leftChildren == null || rightChildren == null)
                return false;

            if (leftChildren.Count != rightChildren.Count)
                return false;

            for (int i = 0, j = leftChildren.Count - 1; i < leftChildren.Count; i++, j--)
            {
                if (!leftChildren[i].NodeValue.Equals(rightChildren[j].NodeValue))
                    return false;

                if (!IsSymmetricallyRecursive(leftChildren[i].Children, rightChildren[j].Children))
                    return false;
            }

            return true;
        }

        public List<T> EvenTrees()
        {
            if (Root == null)
                return new List<T>(0);

            List<T> vertices = new List<T>();

            if (EvenTrees(Root, vertices) % 2 != 0)
                return new List<T>(0);

            return vertices;
        }

        private int EvenTrees(SimpleTreeNode<T> root, List<T> vertices)
        {
            int count = 1;

            if (root.Children == null)
                return count;
            
            foreach (SimpleTreeNode<T> child in root.Children)
            {
                int childCount = EvenTrees(child, vertices);

                if (childCount % 2 == 0)
                {
                    vertices.Add(root.NodeValue);
                    vertices.Add(child.NodeValue);
                }
                else
                {
                    count += childCount;
                }
            }

            return count;
        }
    }

}
