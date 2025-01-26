using AlgorithmsDataStructures2;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Education.Ads.Tests.Exercise3
{
    public class BST_Tests
    {
        [Theory]
        [MemberData(nameof(GetWideAllNodesData))]
        public void Should_WideAllNodes(BST<int> tree, List<BSTNode<int>> result)
        { 
            var nodes = tree.WideAllNodes();

            nodes.Count.ShouldBe(result.Count);
            nodes.Select(x => x.NodeKey).ShouldBe(result.Select(x => x.NodeKey));
            nodes.Select(x => x.Parent?.NodeKey).ShouldBe(result.Select(x => x.Parent?.NodeKey));
            nodes.Select(x => x.LeftChild?.NodeKey).ShouldBe(result.Select(x => x.LeftChild?.NodeKey));
            nodes.Select(x => x.RightChild?.NodeKey).ShouldBe(result.Select(x => x.RightChild?.NodeKey));
        }

        [Theory]
        [MemberData(nameof(GetDeepAllNodesInOrderData))]
        [MemberData(nameof(GetDeepAllNodesPostOrderData))]
        [MemberData(nameof(GetDeepAllNodesPreOrderData))]
        public void Should_DeepAllNodesIterative(BST<int> tree, int order, List<BSTNode<int>> result)
        {
            var nodes = tree.DeepAllNodesIterative(order);

            nodes.Count.ShouldBe(result.Count);
            nodes.Select(x => x.NodeKey).ShouldBe(result.Select(x => x.NodeKey));
            nodes.Select(x => x.Parent?.NodeKey).ShouldBe(result.Select(x => x.Parent?.NodeKey));
            nodes.Select(x => x.LeftChild?.NodeKey).ShouldBe(result.Select(x => x.LeftChild?.NodeKey));
            nodes.Select(x => x.RightChild?.NodeKey).ShouldBe(result.Select(x => x.RightChild?.NodeKey));
        }

        [Theory]
        [MemberData(nameof(GetDeepAllNodesInOrderData))]
        [MemberData(nameof(GetDeepAllNodesPostOrderData))]
        [MemberData(nameof(GetDeepAllNodesPreOrderData))]
        public void Should_DeepAllNodesRecursive(BST<int> tree, int order, List<BSTNode<int>> result)
        {
            var nodes = tree.DeepAllNodesIterative(order);

            nodes.Count.ShouldBe(result.Count);
            nodes.Select(x => x.NodeKey).ShouldBe(result.Select(x => x.NodeKey));
            nodes.Select(x => x.Parent?.NodeKey).ShouldBe(result.Select(x => x.Parent?.NodeKey));
            nodes.Select(x => x.LeftChild?.NodeKey).ShouldBe(result.Select(x => x.LeftChild?.NodeKey));
            nodes.Select(x => x.RightChild?.NodeKey).ShouldBe(result.Select(x => x.RightChild?.NodeKey));
        }

        [Theory]
        [MemberData(nameof(GetShouldInvertTreeData))]
        public void Should_InvertTree(BST<int> tree, BST<int> treeCopy)
        {
            tree.InvertTree();

            tree.DeepAllNodesIterative(0).ForEach(n =>
            {
                n.RightChild?.Parent.ShouldBe(n);
                n.LeftChild?.Parent.ShouldBe(n);

                n.RightChild?.NodeKey.ShouldBeLessThan(n.NodeKey);
                n.LeftChild?.NodeKey.ShouldBeGreaterThan(n.NodeKey);
            });
            
            tree.InvertTree();

            tree.CheckEqual(treeCopy).ShouldBeTrue();
        }
             
        public static IEnumerable<object[]> GetWideAllNodesData()
        {
            // 1: Пустое дерево
            var tree = new BST<int>(null);
            var result = new List<BSTNode<int>>();
            yield return new object[] { tree, result };

            // 2: Только Root
            tree = new BST<int>(new BSTNode<int>(8, 100, null));
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node
            };
            yield return new object[] { tree, result };

            // 3: Большое дерево
            tree = GetDefaultTree();
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(6).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(5).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(19).Node,
            };
            yield return new object[] { tree, result };

            // 4: Большое дерево без пары узлов
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(5);
            tree.DeleteNodeByKey(6);
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(19).Node,
            };
            yield return new object[] { tree, result };

            // 5: Большое дерево с парой новых узлов
            tree = GetDefaultTree();
            tree.AddKeyValue(-5, 1001);
            tree.AddKeyValue(-3, 1002);
            tree.AddKeyValue(-1, 1003);
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(6).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(5).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(-5).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(-3).Node,
                tree.FindNodeByKey(19).Node,
                tree.FindNodeByKey(-1).Node,
            };
            yield return new object[] { tree, result };
        }

        public static IEnumerable<object[]> GetDeepAllNodesInOrderData()
        {
            return GetWideAllNodesData()
                .Select(x =>
                {
                    var newX = new object[3];
                    newX[0] = x[0];
                    newX[1] = 0;
                    newX[2] = ((List<BSTNode<int>>)x[1])
                        .OrderBy(y => y.NodeKey)
                        .ToList();
                    return newX;
                }).ToList();
        }

        public static IEnumerable<object[]> GetDeepAllNodesPostOrderData()
        {
            // 1: Пустое дерево
            var tree = new BST<int>(null);
            var result = new List<BSTNode<int>>();
            yield return new object[] { tree, 1, result };

            // 2: Только Root
            tree = new BST<int>(new BSTNode<int>(8, 100, null));
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node
            };
            yield return new object[] { tree, 1, result };

            // 3: Большое дерево
            tree = GetDefaultTree();
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(5).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(6).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(19).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(8).Node,
            };
            yield return new object[] { tree, 1, result };

            // 4: Большое дерево без пары узлов
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(5);
            tree.DeleteNodeByKey(6);
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(19).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(8).Node,
            };
            yield return new object[] { tree, 1, result };

            // 5: Большое дерево с парой новых узлов
            tree = GetDefaultTree();
            tree.AddKeyValue(-5, 1001);
            tree.AddKeyValue(-3, 1002);
            tree.AddKeyValue(-1, 1003);
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(-1).Node,
                tree.FindNodeByKey(-3).Node,
                tree.FindNodeByKey(-5).Node,
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(5).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(6).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(19).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(8).Node,
            };
            yield return new object[] { tree, 1, result };
        }

        public static IEnumerable<object[]> GetDeepAllNodesPreOrderData()
        {
            // 1: Пустое дерево
            var tree = new BST<int>(null);
            var result = new List<BSTNode<int>>();
            yield return new object[] { tree, 2, result };

            // 2: Только Root
            tree = new BST<int>(new BSTNode<int>(8, 100, null));
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node
            };
            yield return new object[] { tree, 2, result };

            // 3: Большое дерево
            tree = GetDefaultTree();
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(6).Node,
                tree.FindNodeByKey(5).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(19).Node,
            };
            yield return new object[] { tree, 2, result };

            // 4: Большое дерево без пары узлов
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(5);
            tree.DeleteNodeByKey(6);
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(19).Node,
            };
            yield return new object[] { tree, 2, result };

            // 5: Большое дерево с парой новых узлов
            tree = GetDefaultTree();
            tree.AddKeyValue(-5, 1001);
            tree.AddKeyValue(-3, 1002);
            tree.AddKeyValue(-1, 1003);
            result = new List<BSTNode<int>>()
            {
                tree.FindNodeByKey(8).Node,
                tree.FindNodeByKey(4).Node,
                tree.FindNodeByKey(2).Node,
                tree.FindNodeByKey(1).Node,
                tree.FindNodeByKey(-5).Node,
                tree.FindNodeByKey(-3).Node,
                tree.FindNodeByKey(-1).Node,
                tree.FindNodeByKey(3).Node,
                tree.FindNodeByKey(6).Node,
                tree.FindNodeByKey(5).Node,
                tree.FindNodeByKey(7).Node,
                tree.FindNodeByKey(12).Node,
                tree.FindNodeByKey(10).Node,
                tree.FindNodeByKey(9).Node,
                tree.FindNodeByKey(11).Node,
                tree.FindNodeByKey(14).Node,
                tree.FindNodeByKey(13).Node,
                tree.FindNodeByKey(15).Node,
                tree.FindNodeByKey(17).Node,
                tree.FindNodeByKey(19).Node,
            };
            yield return new object[] { tree, 2, result };
        }

        public static IEnumerable<object[]> GetShouldInvertTreeData()
        {
            // 1: Пустое дерево
            var tree = new BST<int>(null);
            var treeCopy = new BST<int>(null);
            yield return new object[] { tree, treeCopy };

            // 2: Только Root
            tree = new BST<int>(new BSTNode<int>(8, 100, null));
            treeCopy = new BST<int>(new BSTNode<int>(8, 100, null));
            yield return new object[] { tree, treeCopy };

            // 3: Большое дерево
            tree = GetDefaultTree();
            treeCopy = GetDefaultTree();
            yield return new object[] { tree, treeCopy };

            // 4: Большое дерево без пары узлов
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(5);
            tree.DeleteNodeByKey(6);
            treeCopy = GetDefaultTree();
            treeCopy.DeleteNodeByKey(5);
            treeCopy.DeleteNodeByKey(6);
            yield return new object[] { tree, treeCopy };

            // 5: Большое дерево с парой новых узлов
            tree = GetDefaultTree();
            tree.AddKeyValue(-5, 1001);
            tree.AddKeyValue(-3, 1002);
            tree.AddKeyValue(-1, 1003);
            treeCopy = GetDefaultTree();
            treeCopy.AddKeyValue(-5, 1001);
            treeCopy.AddKeyValue(-3, 1002);
            treeCopy.AddKeyValue(-1, 1003);
            yield return new object[] { tree, treeCopy };
        }

        public static BST<int> GetDefaultTree()
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
