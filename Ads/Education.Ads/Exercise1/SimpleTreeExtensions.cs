using Ads.Exercise6;
using AlgorithmsDataStructures2;
using System.Collections.Generic;
using System.Linq;

namespace Education.Ads.Exercise1
{
    public static class SimpleTreeExtensions
    {
        public static SimpleTree<T> UpdateNodesLevelsRecursive<T>(this SimpleTree<T> tree)
        {
            if(tree.Root != null)
                UpdateNodesLevelsRecursive(tree.Root, 0);

            return tree;
        }

        internal static void UpdateNodesLevelsRecursive<T>(this SimpleTreeNode<T> node, int nodeLevel)
        {
            node.Level = nodeLevel;

            if (node.Children == null)
                return;

            foreach (var child in node.Children)
                UpdateNodesLevelsRecursive(child, nodeLevel + 1);
        }

        public static SimpleTree<T> UpdateNodesLevelsIterative<T>(this SimpleTree<T> tree)
        {
            Queue<SimpleTreeNode<T>> nodesQueue = new Queue<SimpleTreeNode<T>>();

            nodesQueue.Enqueue(tree.Root);

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

            return tree;
        }

        public static bool IsSymmetricallyRecursive<T>(this SimpleTree<T> tree)
        {
            if (tree.Root == null)
                return true;

            return IsSymmetricallyRecursive(tree.Root.Children, tree.Root.Children);
        }

        public static bool IsSymmetricallyRecursive<T>(List<SimpleTreeNode<T>> leftChildren, List<SimpleTreeNode<T>> rightChildren)
        {
            if (leftChildren == null && rightChildren == null)
                return true;

            if (leftChildren == null || rightChildren == null)
                return false;

            if (leftChildren.Count != rightChildren.Count)
                return false;

            for (int i = 0, j = leftChildren.Count - 1; i < j; i++, j--)
            {
                if (!leftChildren[i].NodeValue.Equals(rightChildren[j].NodeValue))
                    return false;

                return IsSymmetricallyRecursive(leftChildren[i].Children, rightChildren[j].Children);
            }

            return true;
        }

        public static bool IsSymmetricallyIterative<T>(this SimpleTree<T> tree)
        {
            if (tree.Root == null)
                return true;

            // Т.к. по умолчанию в C# нет реализации деки, я использовал свою из первой части курса АСД
            ImprovedDeque<SimpleTreeNode<T>> nodesDeque = new ImprovedDeque<SimpleTreeNode<T>>();
            nodesDeque.AddFront(tree.Root);

            while (nodesDeque.Size() > 0)
            {
                SimpleTreeNode<T> leftNode = nodesDeque.RemoveFront();

                if (nodesDeque.Size() != 0)
                {
                    SimpleTreeNode<T> rightNode = nodesDeque.RemoveTail();

                    if (!leftNode.NodeValue.Equals(rightNode.NodeValue))
                        return false;

                    if (leftNode.Children == null && leftNode.Children == null)
                        continue;

                    if (leftNode.Children.Count != rightNode.Children.Count)
                        return false;

                    foreach (SimpleTreeNode<T> child in rightNode.Children)
                        nodesDeque.AddTail(child);
                }

                if (leftNode.Children != null)
                    foreach (SimpleTreeNode<T> child in leftNode.Children)
                        nodesDeque.AddTail(child);
            }

            return true;
        }
    }
}
