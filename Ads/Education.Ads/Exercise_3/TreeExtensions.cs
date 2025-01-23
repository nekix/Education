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

            return RecoverTree(0, preorder.Length, 0, inorder.Length, preorder, inorder);
        }

        private static BSTNode RecoverTree(int preLeft, int preRigth, int inLeft, int inRight, int[] preorder, int[] inorder)
        {
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

            BSTNode node = new BSTNode { NodeKey = root };
            
            if (inRootIndex > 0)
            {
                BSTNode leftNode = RecoverTree(preLeft + 1, inRootIndex + 1, inLeft, inRootIndex, preorder, inorder);
                leftNode.Parent = node;
                node.LeftChild = leftNode;
            }   

            if (inRootIndex + 1 < inRight - inLeft)
            {
                BSTNode rightNode = RecoverTree(inRootIndex + 1, preRigth, inRootIndex + 1, inRight, preorder, inorder);
                rightNode.Parent = node;
                node.RightChild = rightNode;
            }      

            return node;
        }
    }
}
