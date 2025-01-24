using AlgorithmsDataStructures2;
using System;
using System.Collections.Generic;

namespace Education.Ads.Exercise_3
{
    public static class TreeExtensions
    {
        public static BSTNode RecoverTree(int[] preorder, int[] inorder)
        {
            if (preorder.Length != inorder.Length)
                return null;

            int count = preorder.Length;

            if (count == 0)
                return null;

            if (preorder[count - 1] != inorder[count - 1])
                return null;

            int preLeft = 0;
            return RecoverTree(ref preLeft, 0, inorder.Length, preorder, inorder);
        }

        private static BSTNode RecoverTree(ref int preLeft, int inLeft, int inRight, int[] preorder, int[] inorder)
        {
            if (inLeft >= inRight)
                return null;

            int root = preorder[preLeft];
            int inRootIndex = -1;

            for (int i = inLeft; i < inRight; i++)
            {
                if (inorder[i] == root)
                {
                    inRootIndex = i;
                    break;
                }
            }

            preLeft++;

            BSTNode node = new BSTNode { NodeKey = root };

            BSTNode leftNode = RecoverTree(ref preLeft, inLeft, inRootIndex, preorder, inorder);
            if (leftNode != null)
            {
                leftNode.Parent = node;
                node.LeftChild = leftNode;
            }

            BSTNode rightNode = RecoverTree(ref preLeft, inRootIndex + 1, inRight, preorder, inorder);
            if (rightNode != null)
            {
                rightNode.Parent = node;
                node.RightChild = rightNode;    
            }      

            return node;
        }
    }
}
