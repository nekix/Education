using AlgorithmsDataStructures2;
using Education.Ads.Exercise_3;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Education.Ads.Tests.Exercise3
{
    public class TreeExtensions_Tests
    {
        [Theory]
        [MemberData(nameof(GetRecoverTreeData))]
        public void Should_RecoverTree(int[] preorder, int[] inorder, Action<BSTNode> check)
        {
            var root = TreeExtensions.RecoverTree(preorder, inorder);

            check.Invoke(root);
        }

        public static IEnumerable<object[]> GetRecoverTreeData()
        {
            int[] preorder = new int[] { 1, 2, 4, 5, 3, 6, 7 };
            int[] inorder = new int[] { 4, 2, 5, 1, 6, 3, 7 };

            Action<BSTNode> check = (node) =>
            {
                node.NodeKey.ShouldBe(1);
                node.LeftChild.NodeKey.ShouldBe(2);
                node.LeftChild.LeftChild.NodeKey.ShouldBe(4);
                node.LeftChild.RightChild.NodeKey.ShouldBe(5);
                node.RightChild.NodeKey.ShouldBe(3);
                node.RightChild.LeftChild.NodeKey.ShouldBe(6);
                node.RightChild.RightChild.NodeKey.ShouldBe(7);
            };

            yield return new object[] { preorder, inorder, check };
        }
    }
}
