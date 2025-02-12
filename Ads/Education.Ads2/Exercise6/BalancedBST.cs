using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class BSTNode
    {
        public int NodeKey;
        public BSTNode Parent;
        public BSTNode LeftChild;
        public BSTNode RightChild;
        public int Level;

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
        public BSTNode Root;

        public BalancedBST()
        {
            Root = null;
        }

        public void GenerateTree(int[] a)
        {
            int[] sorted = new int[a.Length];
            a.CopyTo(sorted, 0);
            Array.Sort(sorted);

            Root = GenerateTree(sorted, null, 0, 0, 0, sorted.Length - 1);
        }

        private BSTNode GenerateTree(int[] sorted, BSTNode parent, int level, int root, int left, int right)
        {
            if (root >= sorted.Length)
                return null;

            if (left > right)
                return null;

            int oldRoot = (right - left) / 2 + left;

            BSTNode node = new BSTNode(sorted[oldRoot], parent);
            node.Level = level;

            node.LeftChild = GenerateTree(sorted, node, level + 1, GetLeftChildIndex(root), left, oldRoot - 1);
            node.RightChild = GenerateTree(sorted, node, level + 1, GetRightChildIndex(root), oldRoot + 1, right);

            return node;
        }

        private static int GetLeftChildIndex(int index)
            => 2 * index + 1;

        private static int GetRightChildIndex(int index)
            => 2 * index + 2;

        public bool IsSearchTree(BSTNode root_node)
        {
            if (root_node == null)
                return true;

            return IsSearchTree(root_node.LeftChild, int.MinValue, root_node.NodeKey, false) 
                && IsSearchTree(root_node.RightChild, root_node.NodeKey, int.MaxValue, true);
        }

        private bool IsSearchTree(BSTNode root_node, int min, int max, bool isRight)
        {
            if (root_node == null)
                return true;

            if (root_node.NodeKey < min)
                return false;

            if (!isRight && root_node.NodeKey == min)
                return false;

            if (root_node.NodeKey >= max)
                return false;

            return IsSearchTree(root_node.LeftChild, min, root_node.NodeKey, false) 
                && IsSearchTree(root_node.RightChild, root_node.NodeKey, max, true);
        }

        public bool IsBalanced(BSTNode root_node)
        {
            return GetBalancedHeight(root_node) != -1;
        }

        private int GetBalancedHeight(BSTNode root)
        {
            if (root == null)
                return 0;

            int leftMax = GetBalancedHeight(root.LeftChild);
            if (leftMax == -1)
                return -1;

            int rightMax = GetBalancedHeight(root.RightChild);
            if (rightMax == -1)
                return -1;

            if (Math.Abs(leftMax - rightMax) > 1)
                return -1;

            return Math.Max(leftMax, rightMax) + 1;
        }
    }
}

