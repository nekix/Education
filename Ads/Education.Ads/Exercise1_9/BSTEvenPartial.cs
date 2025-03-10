using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public partial class BST<T>
    {
        // Оригинальная публичная версия метода DeepAllNodesRecursive(int order)
        // возвращала BSTNode (не generic), поэтому сделал универсальную обёртку.
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

        private BSTNode<T> BalanceEvenTree(List<BSTNode<T>> nodes, int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
                return null;

            int middle = (int)Math.Ceiling((startIndex + endIndex) / 2d);

            BSTNode<T> root = nodes[middle];
            root.Parent = null;

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
