﻿using AlgorithmsDataStructures2;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Education.Ads.Tests.Exercise2_3
{
    public class BST_Tests
    {
        [Theory]
        [MemberData(nameof(GetFindNodeByKeyData))]
        public void Should_FindNodeByKey(BST<int> tree, int key, BSTFind<int> findResult)
        {
            var result = tree.FindNodeByKey(key);

            if(findResult.Node == null)
            {
                result.Node.ShouldBeNull();
            }
            else
            {
                result.Node.NodeValue.ShouldBe(findResult.Node.NodeValue);
            }
            
            result.NodeHasKey.ShouldBe(findResult.NodeHasKey);
            result.ToLeft.ShouldBe(findResult.ToLeft);
        }

        [Theory]
        [MemberData(nameof(GetAddKeyValueData))]
        public void Should_AddKeyValue(BST<int> tree, int key, int value, BSTFind<int> findResult)
        {
            tree.AddKeyValue(key, value);

            Should_FindNodeByKey(tree, key, findResult);
        }

        [Theory]
        [MemberData(nameof(GetFinMinMaxData))]
        public void Should_FinMinMax(BST<int> tree, BSTNode<int> fromNode, bool findMax, int nodeKey, int nodeValue)
        {
            var node = tree.FinMinMax(fromNode, findMax);

            node.NodeKey.ShouldBe(nodeKey);
            node.NodeValue.ShouldBe(nodeValue);
        }

        [Theory]
        [MemberData(nameof(GetDeleteNodeByKeyData))]
        public void Should_DeleteNodeByKey(BST<int> tree, int key, bool result, Action<BST<int>> checkTree)
        {
            tree.DeleteNodeByKey(key).ShouldBe(result);

            tree.FindNodeByKey(key).NodeHasKey.ShouldBe(false);

            checkTree.Invoke(tree);
        }

        [Theory]
        [MemberData(nameof(GetCountData))]
        public void Should_Count(BST<int> tree, int count)
        {
            tree.Count().ShouldBe(count);
        }

        [Theory]
        [MemberData(nameof(GetCheckEqualData))]
        public void Should_CheckEqual(BST<int> firstTree, BST<int> secondTree, bool result)
        {
            firstTree.CheckEqual(secondTree).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(GetLeafPathsData))]
        public void Should_GetLeafPathsIterative(BST<int> tree, int pathLength, List<List<BSTNode<int>>> paths)
        {
            var result = tree.GetLeafPathsIterative(pathLength);
            
            result.Count.ShouldBe(paths.Count);
            for (int i = 0; i < result.Count; i++)
            {
                result[i].ShouldBe(paths[i]);
            }
        }

        [Theory]
        [MemberData(nameof(GetLeafPathsData))]
        public void Should_GetLeafPathsRecursive(BST<int> tree, int pathLength, List<List<BSTNode<int>>> paths)
        {
            var result = tree.GetLeafPathsRecursive(pathLength);

            result.Count.ShouldBe(paths.Count);
            for (int i = 0; i < result.Count; i++)
            {
                result[i].ShouldBe(paths[i]);
            }
        }

        public static IEnumerable<object[]> GetFindNodeByKeyData()
        {
            // 1: Левый отсутсвующий
            var tree = GetDefaultTree();
            yield return new object[] { tree, 18, new BSTFind<int> { Node = new BSTNode<int>(19, 116, null), ToLeft = true } };

            // 2: Правый отсутсвующий
            tree = GetDefaultTree();
            yield return new object[] { tree, 20, new BSTFind<int> { Node = new BSTNode<int>(19, 116, null) } };

            // 3: Левый присутствующий
            tree = GetDefaultTree();
            yield return new object[] { tree, 9, new BSTFind<int> { Node = new BSTNode<int>(9, 111, null), NodeHasKey = true } };

            // 4: Правый присутствующий
            tree = GetDefaultTree();
            yield return new object[] { tree, 6, new BSTFind<int> { Node = new BSTNode<int>(6, 104, null), NodeHasKey = true } };
        }

        public static IEnumerable<object[]> GetAddKeyValueData()
        {
            // 1: Добавление первого узла (ROOT)
            var tree = new BST<int>(null);
            yield return new object[] { tree, 8, 300, new BSTFind<int> { Node = new BSTNode<int>(8, 300, null), NodeHasKey = true } };

            // 2: Добавление левого узла
            tree = GetDefaultTree();
            yield return new object[] { tree, 18, 300, new BSTFind<int> { Node = new BSTNode<int>(18, 300, null), NodeHasKey = true } };

            // 3: Добавление правого узла
            tree = GetDefaultTree();
            yield return new object[] { tree, 20, 300, new BSTFind<int> { Node = new BSTNode<int>(20, 300, null), NodeHasKey = true } };

            // 4: Добавление существующего узла (неизменность дерева)
            tree = GetDefaultTree();
            yield return new object[] { tree, 7, 300, new BSTFind<int> { Node = new BSTNode<int>(7, 110, null), NodeHasKey = true } };
        }

        public static IEnumerable<object[]> GetFinMinMaxData()
        {
            // 1: С корня максимум
            var tree = GetDefaultTree();
            yield return new object[] { tree, tree.FindNodeByKey(8).Node, true, 19, 116 };

            // 2: С корня минимум
            tree = GetDefaultTree();
            yield return new object[] { tree, tree.FindNodeByKey(8).Node, false, 1, 107 };

            // 3: С поддерева максимум
            tree = GetDefaultTree();
            yield return new object[] { tree, tree.FindNodeByKey(6).Node, true, 7, 110 };

            // 4: С поддерева минимум
            tree = GetDefaultTree();
            yield return new object[] { tree, tree.FindNodeByKey(10).Node, true, 11, 112 };
        }

        public static IEnumerable<object[]> GetDeleteNodeByKeyData()
        {
            // 1: Root узел
            var tree = GetDefaultTree();
            Action<BST<int>> action = (t) =>
            {
                t.Count().ShouldBe(16);
                t.FindNodeByKey(8).NodeHasKey.ShouldBeFalse();
            };
            yield return new object[] { tree, 8, true, action };

            // 2: Удаление НЕ листа (с одним дочерним узлом)
            tree = GetDefaultTree();
            action = (t) =>
            {
                t.Count().ShouldBe(16);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.NodeKey.ShouldBe(17);
            };
            yield return new object[] { tree, 15, true, action };

            // 3: Удаление листа
            tree = GetDefaultTree();
            action = (t) =>
            {
                t.Count().ShouldBe(16);
                t.FindNodeByKey(8).Node.RightChild.LeftChild.LeftChild.ShouldBeNull();
            };
            yield return new object[] { tree, 9, true, action };

            // 4: Несуществующий узел
            tree = GetDefaultTree();
            action = (t) =>
            {
                t.Count().ShouldBe(17);
            };
            yield return new object[] { tree, 16, false, action };

            // 5: Удаление НЕ листа (с двумя потомками, узел приемник с правым потомком)
            tree = GetDefaultTree();
            tree.AddKeyValue(18, 301);
            tree.AddKeyValue(25, 302);
            tree.AddKeyValue(22, 303);
            tree.AddKeyValue(20, 304);
            tree.AddKeyValue(21, 305);
            action = (t) =>
            {
                t.Count().ShouldBe(21);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.NodeKey.ShouldBe(20);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.RightChild.NodeKey.ShouldBe(25);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.RightChild.Parent.NodeKey.ShouldBe(20);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.LeftChild.NodeKey.ShouldBe(18);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.LeftChild.Parent.NodeKey.ShouldBe(20);

                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.RightChild.LeftChild.NodeKey.ShouldBe(22);
                
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.RightChild.LeftChild.LeftChild.NodeKey.ShouldBe(21);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.RightChild.LeftChild.LeftChild.Parent.NodeKey.ShouldBe(22);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.RightChild.LeftChild.Parent.NodeKey.ShouldBe(25);
                t.FindNodeByKey(8).Node.RightChild.RightChild.RightChild.RightChild.RightChild.RightChild.LeftChild.Parent.LeftChild.NodeKey.ShouldBe(22);
            };
            yield return new object[] { tree, 19, true, action };

            // 6: Удаление НЕ листа (с двумя потомками, узел приемник без правого потомка)
            tree = GetDefaultTree();
            action = (t) =>
            {
                t.Count().ShouldBe(16);
            };
            yield return new object[] { tree, 12, true, action };
        }

        public static IEnumerable<object[]> GetCountData()
        {
            // 1: Пустое
            var tree = new BST<int>(null);
            yield return new object[] { tree, 0 };

            // 2: Только root
            tree = new BST<int>(new BSTNode<int>(5, 5, null));
            yield return new object[] { tree, 1 };

            // 3: Заполненное
            tree = GetDefaultTree();
            yield return new object[] { tree, 17 };

            // 4: Заполненное после удаления нескольких узлов
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(17);
            tree.DeleteNodeByKey(1);
            tree.DeleteNodeByKey(12);
            yield return new object[] { tree, 14 };
        }

        public static IEnumerable<object[]> GetCheckEqualData()
        {
            // 1: Оба пустых
            var firstTree = new BST<int>(null);
            var secondTree = new BST<int>(null);
            yield return new object[] { firstTree, secondTree, true };

            // 2: Одно пустое, одно с Root
            firstTree = new BST<int>(null);
            secondTree = new BST<int>(new BSTNode<int>(5, 5, null));
            yield return new object[] { firstTree, secondTree, false };

            // 3: Оба с Root
            firstTree = new BST<int>(new BSTNode<int>(5, 5, null));
            secondTree = new BST<int>(new BSTNode<int>(5, 5, null));
            yield return new object[] { firstTree, secondTree, true };

            // 4: Оба с Root(разные ключи)
            firstTree = new BST<int>(new BSTNode<int>(3, 5, null));
            secondTree = new BST<int>(new BSTNode<int>(5, 5, null));
            yield return new object[] { firstTree, secondTree, false };

            // 5: Оба с Root(разные значения)
            firstTree = new BST<int>(new BSTNode<int>(3, 5, null));
            secondTree = new BST<int>(new BSTNode<int>(5, 5, null));
            yield return new object[] { firstTree, secondTree, false };

            // 6: Оба заполненные
            firstTree = GetDefaultTree();
            secondTree = GetDefaultTree();
            yield return new object[] { firstTree, secondTree, true };

            // 7: Оба заполненные (разные значения в одной ноде)
            firstTree = GetDefaultTree();
            secondTree = GetDefaultTree();
            firstTree.FindNodeByKey(17).Node.NodeValue = 125;
            yield return new object[] { firstTree, secondTree, false };

            // 8: Оба заполненные (разные ключи в одной ноде)
            firstTree = GetDefaultTree();
            secondTree = GetDefaultTree();
            firstTree.FindNodeByKey(17).Node.NodeKey = 18;
            yield return new object[] { firstTree, secondTree, false };

            // 9: Оба заполненные (разноче число нод)
            firstTree = GetDefaultTree();
            secondTree = GetDefaultTree();
            firstTree.FindNodeByKey(10).Node.LeftChild = null;
            yield return new object[] { firstTree, secondTree, false };

            // 10: Оба заполненных дерева, но через разные операции
            firstTree = GetDefaultTree();
            firstTree.DeleteNodeByKey(17);
            firstTree.DeleteNodeByKey(19);
            firstTree.DeleteNodeByKey(8);
            firstTree.DeleteNodeByKey(7);
            var secondRootNode = new BSTNode<int>(8, 100, null);
            secondTree = new BST<int>(secondRootNode);
            secondTree.AddKeyValue(4, 101);
            secondTree.AddKeyValue(12, 102);
            secondTree.AddKeyValue(2, 103);
            secondTree.AddKeyValue(6, 104);
            secondTree.AddKeyValue(10, 105);
            secondTree.AddKeyValue(14, 106);
            secondTree.AddKeyValue(1, 107);
            secondTree.AddKeyValue(3, 108);
            secondTree.AddKeyValue(5, 109);
            secondTree.AddKeyValue(7, 110);
            secondTree.AddKeyValue(9, 111);
            secondTree.AddKeyValue(11, 112);
            secondTree.AddKeyValue(13, 113);
            secondTree.AddKeyValue(15, 114);
            secondTree.AddKeyValue(17, 115);
            secondTree.AddKeyValue(19, 116);
            secondTree.DeleteNodeByKey(8);
            secondTree.DeleteNodeByKey(7);

            yield return new object[] { firstTree, secondTree, false };
        }

        public static IEnumerable<object[]> GetLeafPathsData()
        {
            // 1: Пустое дерево
            var tree = new BST<int>(null);
            yield return new object[] { tree, 1, new List<List<BSTNode<int>>>(0) };

            // 2: Только корневой и длина пути 1
            tree = new BST<int>(new BSTNode<int>(8, 100, null));
            yield return new object[] { tree, 1, new List<List<BSTNode<int>>>(0) };

            // 3: Только корневой и длина пути 2
            tree = new BST<int>(new BSTNode<int>(8, 100, null));
            yield return new object[] { tree, 2, new List<List<BSTNode<int>>>(0) };

            // 4: Основное дерево, только третий уровень
            tree = GetDefaultTree();
            var paths = new List<List<BSTNode<int>>>
            {
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(4).Node,
                    tree.FindNodeByKey(2).Node,
                    tree.FindNodeByKey(1).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(4).Node,
                    tree.FindNodeByKey(2).Node,
                    tree.FindNodeByKey(3).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(4).Node,
                    tree.FindNodeByKey(6).Node,
                    tree.FindNodeByKey(5).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(4).Node,
                    tree.FindNodeByKey(6).Node,
                    tree.FindNodeByKey(7).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(12).Node,
                    tree.FindNodeByKey(10).Node,
                    tree.FindNodeByKey(9).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(12).Node,
                    tree.FindNodeByKey(10).Node,
                    tree.FindNodeByKey(11).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(12).Node,
                    tree.FindNodeByKey(14).Node,
                    tree.FindNodeByKey(13).Node,
                },
            };
            yield return new object[] { tree, 3, paths };


            // 5: Основное дерево, третий уровень
            tree = GetDefaultTree();
            yield return new object[] { tree, 4, new List<List<BSTNode<int>>>(0) };

            // 6: Основное дерево, четвертый уровень
            tree = GetDefaultTree();
            paths = new List<List<BSTNode<int>>>
            {
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(8).Node,
                    tree.FindNodeByKey(12).Node,
                    tree.FindNodeByKey(14).Node,
                    tree.FindNodeByKey(15).Node,
                    tree.FindNodeByKey(17).Node,
                    tree.FindNodeByKey(19).Node,
                },
            };
            yield return new object[] { tree, 5, paths };

            // 7: Основное дерево, удалено 4 узла, второй уровень
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(8);
            tree.DeleteNodeByKey(9);
            tree.DeleteNodeByKey(5);
            tree.DeleteNodeByKey(7);
            paths = new List<List<BSTNode<int>>>
            {
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(10).Node,
                    tree.FindNodeByKey(4).Node,
                    tree.FindNodeByKey(6).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(10).Node,
                    tree.FindNodeByKey(12).Node,
                    tree.FindNodeByKey(11).Node,
                },
            };
            yield return new object[] { tree, 2, paths };

            // 8: Основное дерево, удалено 4 узла, третий уровень
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(8);
            tree.DeleteNodeByKey(9);
            tree.DeleteNodeByKey(5);
            tree.DeleteNodeByKey(7);
            paths = new List<List<BSTNode<int>>>
            {
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(10).Node,
                    tree.FindNodeByKey(4).Node,
                    tree.FindNodeByKey(2).Node,
                    tree.FindNodeByKey(1).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(10).Node,
                    tree.FindNodeByKey(4).Node,
                    tree.FindNodeByKey(2).Node,
                    tree.FindNodeByKey(3).Node,
                },
                new List<BSTNode<int>>
                {
                    tree.FindNodeByKey(10).Node,
                    tree.FindNodeByKey(12).Node,
                    tree.FindNodeByKey(14).Node,
                    tree.FindNodeByKey(13).Node,
                },
            };
            yield return new object[] { tree, 3, paths };
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
