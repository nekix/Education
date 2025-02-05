using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class BSTNode
    {
        public int NodeKey; // ключ узла
        public BSTNode Parent; // родитель или null для корня
        public BSTNode LeftChild; // левый потомок
        public BSTNode RightChild; // правый потомок	
        public int Level; // глубина узла

        public BSTNode(int key, BSTNode parent)
        {
            NodeKey = key;
            Parent = parent;
            LeftChild = null;
            RightChild = null;
        }
    }


    public class BalancedBST
    {
        public BSTNode Root; // корень дерева

        public BalancedBST()
        {
            Root = null;
        }

        public void GenerateTree(int[] a)
        {
            int[] sorted = new int[a.Length];
            a.CopyTo(sorted, 0);
            Array.Sort(sorted);

            Root = GenerateBBSTArray(sorted, null, 0, 0, 0, sorted.Length - 1);
        }

        public bool IsBalanced(BSTNode root_node)
        {
            if (root_node == null)
                return true;

            if (root_node.LeftChild != null)
                if (root_node.NodeKey <= root_node.LeftChild.NodeKey)
                    return false;

            if (root_node.RightChild != null)
                if (root_node.NodeKey > root_node.RightChild.NodeKey)
                    return false;

            return IsBalanced(root_node.LeftChild) && IsBalanced(root_node.RightChild);
        }

        private BSTNode GenerateBBSTArray(int[] sorted, BSTNode parent, int level, int root, int left, int right)
        {
            if (root >= sorted.Length)
                return null;

            if (left > right)
                return null;

            int oldRoot = (right - left) / 2 + left;

            var node = new BSTNode(sorted[oldRoot], parent);
            node.Level = level;

            node.LeftChild = GenerateBBSTArray(sorted, node, level + 1, GetLeftChildIndex(root), left, oldRoot - 1);
            node.RightChild = GenerateBBSTArray(sorted, node, level + 1, GetRightChildIndex(root), oldRoot + 1, right);

            return node;
        }

        private static int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private static int GetRightChildIndex(int index)
            => 2 * index + 2;



    }
}

