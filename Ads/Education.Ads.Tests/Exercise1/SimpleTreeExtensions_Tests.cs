using AlgorithmsDataStructures2;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Education.Ads.Tests.Exercise1
{
    public class SimpleTreeExtensions_Tests
    {
        [Theory]
        [MemberData(nameof(UpdateNodesLevelsData))]
        public void Should_UpdateNodesLevelsRecursive(SimpleTree<int> tree, Action<SimpleTree<int>> checkAction)
        {
            tree.UpdateNodesLevelsRecursive();

            checkAction(tree);
        }

        [Theory]
        [MemberData(nameof(UpdateNodesLevelsData))]
        public void Should_UpdateNodesLevelsIterative(SimpleTree<int> tree, Action<SimpleTree<int>> checkAction)
        {
            tree.UpdateNodesLevelsIterative();

            checkAction(tree);
        }

        [Theory]
        [MemberData(nameof(IsSymmetricallyData))]
        public void Should_IsSymmetricallyRecursive(SimpleTree<int> tree, bool isSymmetrically)
        {
            tree.IsSymmetricallyRecursive().ShouldBe(isSymmetrically);
        }

        [Theory]
        [MemberData(nameof(IsSymmetricallyData))]
        public void Should_IsSymmetricallyIterative(SimpleTree<int> tree, bool isSymmetrically)
        {
            tree.IsSymmetricallyIterative().ShouldBe(isSymmetrically);
        }

        public static IEnumerable<object[]> IsSymmetricallyData()
        {
            // 1: Пустое дерево
            yield return new object[] { new SimpleTree<int>(null), true };

            // 2: Только root node
            yield return new object[] { new SimpleTree<int>(new SimpleTreeNode<int>(0, null)), true };

            // 3: Root node и одна дочерняя
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(1, tree.Root));
            yield return new object[] { tree, true };

            // 4: Root node и две дочерние - симметричные
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(1, tree.Root));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(1, tree.Root));
            yield return new object[] { tree, true };

            // 5: Root node и две дочерние - несимметричные
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(1, tree.Root));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(2, tree.Root));
            yield return new object[] { tree, false };

            // 6: Асимметричное дерево (разное число нод)
            SimpleTreeNode<int> root = new SimpleTreeNode<int>(1, null);
            SimpleTreeNode<int> node1 = new SimpleTreeNode<int>(2, root);
            SimpleTreeNode<int> node2 = new SimpleTreeNode<int>(3, root);
            SimpleTreeNode<int> node3 = new SimpleTreeNode<int>(4, node1);
            SimpleTreeNode<int> node4 = new SimpleTreeNode<int>(5, node2);
            SimpleTreeNode<int> node5 = new SimpleTreeNode<int>(6, node3);
            tree = new SimpleTree<int>(root);
            tree.AddChild(root, node1);
            tree.AddChild(root, node2);
            tree.AddChild(node1, node3);
            tree.AddChild(node2, node4);
            tree.AddChild(node3, node5);
            yield return new object[] { tree, false };

            // 7: Симметричное древо (две основные расходящиеся ветви)
            root = new SimpleTreeNode<int>(1, null);
            node1 = new SimpleTreeNode<int>(2, root);
            node2 = new SimpleTreeNode<int>(2, root);
            node3 = new SimpleTreeNode<int>(3, node1);
            node4 = new SimpleTreeNode<int>(3, node2);
            node5 = new SimpleTreeNode<int>(4, node3);
            var node6 = new SimpleTreeNode<int>(4, node4);
            tree = new SimpleTree<int>(root);
            tree.AddChild(root, node1);
            tree.AddChild(root, node2);
            tree.AddChild(node1, node3);
            tree.AddChild(node2, node4);
            tree.AddChild(node3, node5);
            tree.AddChild(node4, node6);
            yield return new object[] { tree, true };

            // 8: Асимметричное дерево (разные значения нод)
            root = new SimpleTreeNode<int>(0, null);
            tree = new SimpleTree<int>(root);
            node1 = new SimpleTreeNode<int>(1, root);
            node2 = new SimpleTreeNode<int>(1, root);
            tree.AddChild(root, node1);
            tree.AddChild(root, node2);
            var node31 = new SimpleTreeNode<int>(3, null);
            var node32 = new SimpleTreeNode<int>(4, null);
            var node33 = new SimpleTreeNode<int>(3, null);
            var node34 = new SimpleTreeNode<int>(4, null);
            tree.AddChild(node1, node31);
            tree.AddChild(node1, node32);
            tree.AddChild(node2, node33);
            tree.AddChild(node2, node34);
            yield return new object[] { tree, false };

            // 9: Асимметричное дерево (разные значения нод)
            root = new SimpleTreeNode<int>(0, null);
            tree = new SimpleTree<int>(root);
            node1 = new SimpleTreeNode<int>(1, root);
            node2 = new SimpleTreeNode<int>(1, root);
            tree.AddChild(root, node1);
            tree.AddChild(root, node2);
            node31 = new SimpleTreeNode<int>(7, null);
            node32 = new SimpleTreeNode<int>(9, null);
            node33 = new SimpleTreeNode<int>(8, null);
            node34 = new SimpleTreeNode<int>(7, null);
            tree.AddChild(node1, node31);
            tree.AddChild(node1, node32);
            tree.AddChild(node2, node33);
            tree.AddChild(node2, node34);
            yield return new object[] { tree, false };

            // 10: Симетричное дерево (разные значения нод)
            root = new SimpleTreeNode<int>(0, null);
            tree = new SimpleTree<int>(root);
            node1 = new SimpleTreeNode<int>(1, root);
            node2 = new SimpleTreeNode<int>(1, root);
            tree.AddChild(root, node1);
            tree.AddChild(root, node2);
            node31 = new SimpleTreeNode<int>(3, null);
            node32 = new SimpleTreeNode<int>(4, null);
            node33 = new SimpleTreeNode<int>(4, null);
            node34 = new SimpleTreeNode<int>(3, null);
            tree.AddChild(node1, node31);
            tree.AddChild(node1, node32);
            tree.AddChild(node2, node33);
            tree.AddChild(node2, node34);
            yield return new object[] { tree, true };

            // 11 Симметричное дерево (есть центральная ветвь)
            root = new SimpleTreeNode<int>(0, null);
            tree = new SimpleTree<int>(root);
            node1 = new SimpleTreeNode<int>(1, root);
            node2 = new SimpleTreeNode<int>(2, root);
            node3 = new SimpleTreeNode<int>(1, root);
            tree.AddChild(root, node1);
            tree.AddChild(root, node2);
            tree.AddChild(root, node3);
            node31 = new SimpleTreeNode<int>(3, null);
            node32 = new SimpleTreeNode<int>(4, null);
            node33 = new SimpleTreeNode<int>(4, null);
            node34 = new SimpleTreeNode<int>(3, null);
            var node35 = new SimpleTreeNode<int>(9, null);
            tree.AddChild(node1, node31);
            tree.AddChild(node1, node32);
            tree.AddChild(node2, node35);
            tree.AddChild(node3, node33);
            tree.AddChild(node3, node34);
            yield return new object[] { tree, true };

            // 12: Асимметричное дерево (после центральной ноды разные значения)
            root = new SimpleTreeNode<int>(0, null);
            tree = new SimpleTree<int>(root);
            node1 = new SimpleTreeNode<int>(1, root);
            node2 = new SimpleTreeNode<int>(1, root);
            node3 = new SimpleTreeNode<int>(2, root);
            tree.AddChild(root, node1);
            tree.AddChild(root, node2);
            tree.AddChild(root, node3);
            node31 = new SimpleTreeNode<int>(3, null);
            node32 = new SimpleTreeNode<int>(4, null);
            node33 = new SimpleTreeNode<int>(4, null);
            node34 = new SimpleTreeNode<int>(3, null);
            node35 = new SimpleTreeNode<int>(7, null);
            var node36 = new SimpleTreeNode<int>(9, null);
            var node37 = new SimpleTreeNode<int>(7, null);
            tree.AddChild(node1, node31);
            tree.AddChild(node1, node32);
            tree.AddChild(node2, node33);
            tree.AddChild(node2, node34);
            tree.AddChild(node3, node35);
            tree.AddChild(node3, node36);
            tree.AddChild(node3, node37);
            yield return new object[] { tree, false };
        }

        public static IEnumerable<object[]> UpdateNodesLevelsData()
        {
            var tree = new SimpleTree<int>(null);
            var treeRoot = new SimpleTreeNode<int>(0, null);
            tree.Root = treeRoot;
            var rootChild = new SimpleTreeNode<int>(1, null);
            rootChild.Children = new List<SimpleTreeNode<int>>
            {
                { new SimpleTreeNode<int>(3, rootChild) },
                { new SimpleTreeNode<int>(4, rootChild) }
            };
            tree.Root.Children = new List<SimpleTreeNode<int>>
            {
                rootChild,
                new SimpleTreeNode<int>(5, tree.Root)
            };
            rootChild.Parent = tree.Root;
            Action<SimpleTree<int>> action = (treeAct) =>
            {
                treeAct.Root.Level.ShouldBe(0);
                treeAct.Root.Children.ForEach(c => c.Level.ShouldBe(1));
                tree.Root.Children
                    .SelectMany(c => c.Children ?? new List<SimpleTreeNode<int>>())
                    .ToList()
                    .ForEach(c => c.Level.ShouldBe(2));
            };
            yield return new object[] { tree, action };
        }
    }
}
