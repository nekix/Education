using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace AlgorithmsDataStructures2
{
    public partial class BST<T>
    {
        public List<BSTNode<T>> DeepAllNodes(int order)
        {
            if (Root == null)
                return new List<BSTNode<T>>(0);

            if (order < 0 || order > 2)
                return new List<BSTNode<T>>(0);

            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            DeepAllNodesRecursive(order, Root, nodes);

            return nodes;
        }

        public bool TryBalanceEvenTree()
        {
            List<BSTNode<T>> nodes = DeepAllNodes(0);

            if (nodes.Count == 0 || nodes.Count % 2 != 0)
                return false;

            BSTNode<T> rootNode = BalanceEvenTree(nodes, 0, nodes.Count - 1);

            Root = rootNode;

            return true;
        }

        public BSTNode<T> BalanceEvenTree(List<BSTNode<T>> nodes, int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
                return null;

            if (startIndex == endIndex)
                return nodes[startIndex];

            int middle =  (startIndex + endIndex) / 2;

            BSTNode<T> root = nodes[middle];

            root.LeftChild = BalanceEvenTree(nodes, startIndex, middle - 1);
            if (root.LeftChild != null)
                root.LeftChild.Parent = root;

            root.RightChild = BalanceEvenTree(nodes, middle + 1, endIndex);
            if (root.RightChild != null)
                root.RightChild.Parent = root;

            return root;
        }
    }
}
