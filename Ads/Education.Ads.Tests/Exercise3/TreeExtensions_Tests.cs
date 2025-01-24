using AlgorithmsDataStructures2;
using Education.Ads.Exercise_3;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // 1: Пустые массивы
            int[] preorder = new int[] { };
            int[] inorder = new int[] { };
            Action<BSTNode> check = (node) =>
            {
                node.ShouldBe(null);
            };
            yield return new object[] { preorder, inorder, check };

            // 2: Разное число элементов
            preorder = new int[] { 1, 2};
            inorder = new int[] { 3 };
            check = (node) =>
            {
                node.ShouldBe(null);
            };
            yield return new object[] { preorder, inorder, check };

            // 3: Один элемент
            preorder = new int[] { 5 };
            inorder = new int[] { 5 };
            check = (node) =>
            {
                node.NodeKey.ShouldBe(5);
                node.LeftChild.ShouldBeNull();
                node.RightChild.ShouldBeNull();
            };
            yield return new object[] { preorder, inorder, check };

            // 4: Вариант из задания
            preorder = new int[] { 1, 2, 4, 5, 3, 6, 7 };
            inorder = new int[] { 4, 2, 5, 1, 6, 3, 7 };
            check = (node) =>
            {
                node.NodeKey.ShouldBe(1);
                node.Parent.ShouldBeNull();

                node.LeftChild.Parent.ShouldBe(node);
                node.LeftChild.NodeKey.ShouldBe(2);

                node.RightChild.Parent.ShouldBe(node);
                node.RightChild.NodeKey.ShouldBe(3);

                node.LeftChild.LeftChild.Parent.ShouldBe(node.LeftChild);
                node.LeftChild.LeftChild.NodeKey.ShouldBe(4);

                node.LeftChild.RightChild.Parent.ShouldBe(node.LeftChild);
                node.LeftChild.RightChild.NodeKey.ShouldBe(5);

                node.RightChild.LeftChild.Parent.ShouldBe(node.RightChild);
                node.RightChild.LeftChild.NodeKey.ShouldBe(6);

                node.RightChild.RightChild.Parent.ShouldBe(node.RightChild);
                node.RightChild.RightChild.NodeKey.ShouldBe(7);

                node.LeftChild.LeftChild.LeftChild.ShouldBeNull();
                node.LeftChild.LeftChild.RightChild.ShouldBeNull();

                node.LeftChild.RightChild.LeftChild.ShouldBeNull();
                node.LeftChild.RightChild.RightChild.ShouldBeNull();

                node.RightChild.LeftChild.LeftChild.ShouldBeNull();
                node.RightChild.LeftChild.RightChild.ShouldBeNull();

                node.RightChild.RightChild.LeftChild.ShouldBeNull();
                node.RightChild.RightChild.RightChild.ShouldBeNull();
            };
            yield return new object[] { preorder, inorder, check };

            // 5: Из большого BST
            var tree = GetDefaultTree();
            preorder = tree.DeepAllNodes(2).Select(n => n.NodeKey).ToArray();
            inorder = tree.DeepAllNodes(0).Select(n => n.NodeKey).ToArray();
            check = (node) =>
            {
                var root = tree.FindNodeByKey(8).Node;
                CompareNodes(root, node);
            };
            yield return new object[] { preorder, inorder, check };
        }

        private static void CompareNodes(BSTNode<int> node, BSTNode secondNode)
        {
            if (node == null || secondNode == null)
            {
                node.ShouldBeNull();
                secondNode.ShouldBeNull();
                return;
            }

            node.NodeKey.ShouldBe(secondNode.NodeKey);

            CompareNodes(node.LeftChild, secondNode.LeftChild);
            CompareNodes(node.RightChild, secondNode.RightChild);
        }

        private static BST<int> GetDefaultTree()
        {
            var root = new BSTNode<int>(8, 100, null);

            var node1 = new BSTNode<int>(4, 101, root);
            root.LeftChild = node1;
            var node2 = new BSTNode<int>(12, 102, root);
            root.RightChild = node2;

            var node11 = new BSTNode<int>(2, 103, node1);
            node1.LeftChild = node11;
            var node12 = new BSTNode<int>(6, 104, node1);
            node1.RightChild = node12;
            var node21 = new BSTNode<int>(10, 105, node2);
            node2.LeftChild = node21;
            var node22 = new BSTNode<int>(14, 106, node2);
            node2.RightChild = node22;

            var node111 = new BSTNode<int>(1, 107, node11);
            node11.LeftChild = node111;
            var node112 = new BSTNode<int>(3, 108, node11);
            node11.RightChild = node112;
            var node121 = new BSTNode<int>(5, 109, node12);
            node12.LeftChild = node121;
            var node122 = new BSTNode<int>(7, 110, node12);
            node12.RightChild = node122;
            var node211 = new BSTNode<int>(9, 111, node21);
            node21.LeftChild = node211;
            var node212 = new BSTNode<int>(11, 112, node21);
            node21.RightChild = node212;
            var node221 = new BSTNode<int>(13, 113, node22);
            node22.LeftChild = node221;
            var node222 = new BSTNode<int>(15, 114, node22);
            node22.RightChild = node222;

            var node2222 = new BSTNode<int>(17, 115, node222);
            node222.RightChild = node2222;

            var node22222 = new BSTNode<int>(19, 116, node2222);
            node2222.RightChild = node22222;
            return new BST<int>(root);
        }
    }
}
