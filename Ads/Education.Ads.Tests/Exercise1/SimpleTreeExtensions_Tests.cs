using AlgorithmsDataStructures2;
using Education.Ads.Exercise1;
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
        public void Should_IsSymmetrically(SimpleTree<int> tree, bool isSymmetrically)
        {
            tree.IsSymmetricallyIterative().ShouldBe(isSymmetrically);
        }

        public static IEnumerable<object[]> IsSymmetricallyData()
        {
            yield return new object[] { new SimpleTree<int>(null), true };

            yield return new object[] { new SimpleTree<int>(new SimpleTreeNode<int>(0, null)), true };

            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.Root.Children = new List<SimpleTreeNode<int>> { new SimpleTreeNode<int>(1, tree.Root) };
            yield return new object[] { tree, true };

            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(1, tree.Root));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(1, tree.Root));
            yield return new object[] { tree, true };

            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(1, tree.Root));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(2, tree.Root));
            yield return new object[] { tree, false };

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
        }

        public static IEnumerable<object[]> UpdateNodesLevelsData()
        {
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var node1 = new SimpleTreeNode<int>(1, tree.Root);
            node1.Children = new List<SimpleTreeNode<int>>
            {
                { new SimpleTreeNode<int>(3, node1) },
                { new SimpleTreeNode<int>(4, node1) }
            };
            tree.Root.Children = new List<SimpleTreeNode<int>>
            {
                node1,
                new SimpleTreeNode<int>(5, tree.Root)
            };
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
