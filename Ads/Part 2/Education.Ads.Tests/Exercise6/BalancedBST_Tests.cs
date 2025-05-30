﻿extern alias Exercise6;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Exercise6.AlgorithmsDataStructures2;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise6
{
    public class BalancedBST_Tests
    {
        [Theory]
        [MemberData(nameof(GetGenerateBBSTArrayData))]
        public void Should_GenerateTree(int[] a, int[] keys, int?[] parents, int[] levels)
        {
            var tree = new BalancedBST();
            
            tree.GenerateTree(a);

            var nodes = WideAllNodes(tree.Root);
            
            nodes.Select(x => x.NodeKey)
                .ToArray()
                .ShouldBe(keys);

            nodes.Select(x => x.Level)
                .ToArray()
                .ShouldBe(levels);

            nodes.Select(x => x.Parent?.NodeKey)
                .ToArray()
                .ShouldBe(parents);
        }

        [Theory]
        [MemberData(nameof(GetIsSearchTreeData))]
        public void Should_IsSearchTree(BSTNode node, BSTNode checkedNode, bool result)
        {
            var tree = new BalancedBST();
            tree.Root = node;

            tree.IsSearchTree(checkedNode).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(GetIsBalancedData))]
        public void Should_IsBalanced(BSTNode rootNode, BSTNode checkedNode, bool result)
        {
            var tree = new BalancedBST();
            tree.Root = rootNode;

            tree.IsBalanced(checkedNode).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(GetIsBalanceIntData))]
        public void Should_IsBalancedInt(int[] a)
        {
            var tree = new BalancedBST();

            tree.GenerateTree(a);

            tree.IsBalanced(tree.Root).ShouldBe(true);
        }

        public static IEnumerable<object[]> GetGenerateBBSTArrayData()
        {
            // 1: Пустой массив
            int[] a = new int[] { };
            int[] keys = new int[] { };
            int?[] parents = new int?[] { };
            int[] levels = new int[] { };
            yield return new object[] { a, keys, parents, levels };

            // 2: Один элемент
            a = new int[] { 7 };
            keys = new int[] { 7 };
            parents = new int?[] { null };
            levels = new int[] { 0 };
            yield return new object[] { a, keys, parents, levels };

            // 3: Древо глубины 1
            a = new int[] { 12, 7, 1 };
            keys = new int[] { 7, 1, 12 };
            parents = new int?[] { null, 7, 7 };
            levels = new int[] { 0, 1, 1 };
            yield return new object[] { a, keys, parents, levels };

            // 4: Древо глубины 3 и полностью заполненное
            a = new int[] { 7, 1, 12, 5, 10, 3, 14, 8, 0, 6, 13, 2, 4, 9, 11 };
            keys = new int[] { 7, 3, 11, 1, 5, 9, 13, 0, 2, 4, 6, 8, 10, 12, 14 };
            parents = new int?[] { null, 7, 7, 3, 3, 11, 11, 1, 1, 5, 5, 9, 9, 13, 13 };
            levels = new int[] { 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3 };
            yield return new object[] { a, keys, parents, levels };

            // 5: Уже отсортированный
            a = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            keys = new int[] { 7, 3, 11, 1, 5, 9, 13, 0, 2, 4, 6, 8, 10, 12, 14 };
            parents = new int?[] { null, 7, 7, 3, 3, 11, 11, 1, 1, 5, 5, 9, 9, 13, 13 };
            levels = new int[] { 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3 };
            yield return new object[] { a, keys, parents, levels };

            // 6: Дерево из массива картинки предыдущего урока
            a = new int[] { 50, 25, 75, 10, 30, 60, 90, 5, 15, 27, 35, 55, 65, 85, 95 };
            keys = new int[] { 50, 25, 75, 10, 30, 60, 90, 5, 15, 27, 35, 55, 65, 85, 95 };
            parents = new int?[] { null, 50, 50, 25, 25, 75, 75, 10, 10, 30, 30, 60, 60, 90, 90 };
            levels = new int[] { 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3 };
            yield return new object[] { a, keys, parents, levels };
        }

        public static IEnumerable<object[]> GetIsSearchTreeData()
        {
            // 1: Один уровень, правильное
            BSTNode node = new BSTNode(50, null);
            BSTNode checkedNode = node;
            yield return new object[] { node, checkedNode, true };

            // 2: Двухуровневое правильное
            node = new BSTNode(50, null);
            node.LeftChild = new BSTNode(25, node);
            node.RightChild = new BSTNode(75, node);
            checkedNode = node;
            yield return new object[] { node, checkedNode, true };

            // 3: Двухуровневое правильное
            node = new BSTNode(50, null);
            node.LeftChild = new BSTNode(25, node);
            node.RightChild = new BSTNode(50, node);
            checkedNode = node;
            yield return new object[] { node, checkedNode, true };

            // 4: Двухуровневое правильное
            node = new BSTNode(50, null);
            node.LeftChild = new BSTNode(50, node);
            node.RightChild = new BSTNode(75, node);
            checkedNode = node;
            yield return new object[] { node, checkedNode, false };

            // 5: Двухуровневое неправильное (левая равна родительской)
            node = new BSTNode(50, null);
            node.LeftChild = new BSTNode(50, node);
            node.RightChild = new BSTNode(75, node);
            checkedNode = node;
            yield return new object[] { node, checkedNode, false };

            // 6: Двухуровневое неправильное (левая равна родительской)
            // но проверяется правильное поддерево
            node = new BSTNode(50, null);
            node.LeftChild = new BSTNode(50, node);
            node.RightChild = new BSTNode(75, node);
            checkedNode = node.LeftChild;
            yield return new object[] { node, checkedNode, true };

            // 7: Двухуровневое неправильное (правая меньше родительской)
            node = new BSTNode(50, null);
            node.LeftChild = new BSTNode(25, node);
            node.RightChild = new BSTNode(30, node);
            checkedNode = node;
            yield return new object[] { node, checkedNode, false };
            
            // 8: Четырёхуровневый правильный (только правая ветвь)
            node = new BSTNode(50, null);
            node.RightChild = new BSTNode(75, node);
            node.RightChild.RightChild = new BSTNode(85, node.RightChild);
            node.RightChild.RightChild.RightChild = new BSTNode(90, node.RightChild.RightChild);
            checkedNode = node;
            yield return new object[] { node, checkedNode, true };

            // 9: Четырёхуровневый неправильный (нижний левый меньше остальных)
            node = new BSTNode(50, null);
            node.RightChild = new BSTNode(75, node);
            node.RightChild.RightChild = new BSTNode(85, node.RightChild);
            node.RightChild.RightChild.LeftChild = new BSTNode(25, node.RightChild.RightChild);
            checkedNode = node;
            yield return new object[] { node, checkedNode, false };

            // 10: Четырёхуровневое правильное
            node = GetDefaultTree();
            checkedNode = node;
            yield return new object[] { node, checkedNode, true };

            // 11: Четырёхуровневое неправильное
            node = GetDefaultTree();
            node.LeftChild.RightChild.RightChild.NodeKey = 64;
            checkedNode = node;
            yield return new object[] { node, checkedNode, false };

            // 12: Четырёхуровневое неправильное
            // но проверяется правильное поддерево
            node = GetDefaultTree();
            node.LeftChild.RightChild.RightChild.NodeKey = 64;
            checkedNode = node.RightChild;
            yield return new object[] { node, checkedNode, true };

            // 13: Четырёхуровневое неправильное
            node = GetDefaultTree();
            node.RightChild.LeftChild.LeftChild.NodeKey = 45;
            checkedNode = node;
            yield return new object[] { node, checkedNode, false };

            // 14: Четырёхуровневое правильное
            node = GetDefaultTree();
            node.RightChild.LeftChild.RightChild.NodeKey = 60;
            checkedNode = node;
            yield return new object[] { node, checkedNode, true };
        }
        public static IEnumerable<object[]> GetIsBalanceIntData()
        {
            return GetGenerateBBSTArrayData().Select(d => new object[] { d[0] });
        }

        public static IEnumerable<object[]> GetIsBalancedData()
        {
            // 1: Один уровень
            BSTNode rootNode = new BSTNode(50, null);
            BSTNode checkNode = rootNode;
            yield return new object[] { rootNode, checkNode, true };

            // 2: Сбалансированный Двухуровневый полностью заполненный
            rootNode = new BSTNode(50, null);
            rootNode.LeftChild = new BSTNode(25, rootNode);
            rootNode.RightChild = new BSTNode(75, rootNode);
            checkNode = rootNode;
            yield return new object[] { rootNode, checkNode, true };

            // 3: Сбалансированный двухуровневый не полностью заполненный
            rootNode = new BSTNode(50, null);
            rootNode.LeftChild = new BSTNode(25, rootNode);
            checkNode = rootNode;
            yield return new object[] { rootNode, checkNode, true };

            // 4: Не сбалансированный трёхуровневый
            rootNode = new BSTNode(50, null);
            rootNode.LeftChild = new BSTNode(25, rootNode);
            rootNode.LeftChild.LeftChild = new BSTNode(5, rootNode);
            checkNode = rootNode;
            yield return new object[] { rootNode, checkNode, false };

            // 5: Не сбалансированный трёхуровневый
            // но проверяется сбалансированное поддерево
            rootNode = new BSTNode(50, null);
            rootNode.LeftChild = new BSTNode(25, rootNode);
            rootNode.LeftChild.LeftChild = new BSTNode(5, rootNode);
            checkNode = rootNode.LeftChild;
            yield return new object[] { rootNode, checkNode, true };

            // 6: Сбалансированный полностью заполненный 4-х уровневый
            rootNode = GetDefaultTree();
            checkNode = rootNode;
            yield return new object[] { rootNode, checkNode, true };

            // 7: Сбалансированный полностью заполненный 4-х уровневый
            // и проверяется сбалансированное поддерево
            rootNode = GetDefaultTree();
            checkNode = rootNode.RightChild.RightChild;
            yield return new object[] { rootNode, checkNode, true };

            // 8: Сбалансированный не полностью заполненный 4-х уровневый
            rootNode = GetDefaultTree();
            DetachFromParent(rootNode.RightChild.RightChild.LeftChild);
            DetachFromParent(rootNode.LeftChild.LeftChild.LeftChild);
            DetachFromParent(rootNode.RightChild.LeftChild.LeftChild);
            checkNode = rootNode;
            yield return new object[] { rootNode, checkNode, true };

            // 9: Не Сбалансированный 4-х уровневый массив
            rootNode = GetDefaultTree();
            DetachFromParent(rootNode.RightChild.RightChild);
            DetachFromParent(rootNode.LeftChild.LeftChild.LeftChild);
            DetachFromParent(rootNode.RightChild.LeftChild.LeftChild);
            checkNode = rootNode;
            yield return new object[] { rootNode, checkNode, false };

            // 10: Не Сбалансированный 4-х уровневый массив,
            // и проверяется сбалансированное поддерево
            rootNode = GetDefaultTree();
            DetachFromParent(rootNode.RightChild.RightChild);
            DetachFromParent(rootNode.LeftChild.LeftChild.LeftChild);
            DetachFromParent(rootNode.RightChild.LeftChild.LeftChild);
            checkNode = rootNode.LeftChild;
            yield return new object[] { rootNode, checkNode, true };

            // 11: Не Сбалансированный 4-х уровневый массив,
            // но проверяется не сбалансированное поддерево
            rootNode = GetDefaultTree();
            DetachFromParent(rootNode.RightChild.RightChild);
            DetachFromParent(rootNode.LeftChild.LeftChild.LeftChild);
            DetachFromParent(rootNode.RightChild.LeftChild.LeftChild);
            checkNode = rootNode.RightChild;
            yield return new object[] { rootNode, checkNode, false };
        }

        private List<BSTNode> WideAllNodes(BSTNode root)
        {
            if (root == null)
                return new List<BSTNode>();

            List<BSTNode> nodesResult = new List<BSTNode>();
            Queue<BSTNode> nodesQueue = new Queue<BSTNode>();

            nodesQueue.Enqueue(root);

            while (nodesQueue.Count != 0)
            {
                BSTNode currentNode = nodesQueue.Dequeue();

                nodesResult.Add(currentNode);

                if (currentNode.LeftChild != null)
                    nodesQueue.Enqueue(currentNode.LeftChild);
                if (currentNode.RightChild != null)
                    nodesQueue.Enqueue(currentNode.RightChild);
            }
            return nodesResult;
        }

        private static BSTNode GetDefaultTree()
        {
            BSTNode node = new BSTNode(50, null);
            node.LeftChild = new BSTNode(25, node);
            node.RightChild = new BSTNode(75, node);

            node.LeftChild.LeftChild = new BSTNode(10, node.LeftChild);
            node.LeftChild.RightChild = new BSTNode(30, node.LeftChild);

            node.RightChild.LeftChild = new BSTNode(60, node.RightChild);
            node.RightChild.RightChild = new BSTNode(90, node.RightChild);

            node.LeftChild.LeftChild.LeftChild = new BSTNode(5, node.LeftChild.LeftChild);
            node.LeftChild.LeftChild.RightChild = new BSTNode(15, node.LeftChild.LeftChild);

            node.LeftChild.RightChild.LeftChild = new BSTNode(27, node.LeftChild.RightChild);
            node.LeftChild.RightChild.RightChild = new BSTNode(35, node.LeftChild.RightChild);

            node.RightChild.LeftChild.LeftChild = new BSTNode(55, node.RightChild.LeftChild);
            node.RightChild.LeftChild.RightChild = new BSTNode(65, node.RightChild.LeftChild);

            node.RightChild.RightChild.LeftChild = new BSTNode(85, node.RightChild.RightChild);
            node.RightChild.RightChild.RightChild = new BSTNode(95, node.RightChild.RightChild);

            return node;
        }

        private static void DetachFromParent(BSTNode node)
        {
            if (node.Parent.LeftChild == node)
                node.Parent.LeftChild = null;
            else if (node.Parent.RightChild == node)
                node.Parent.RightChild = null;

            node.Parent = null;
        }
    }
}
