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
        [MemberData(nameof(Data))]
        public void Should_UpdateNodesLevelsRecursive(SimpleTree<int> tree, Action<SimpleTree<int>> checkAction)
        {
            tree.UpdateNodesLevelsRecursive();

            checkAction(tree);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Should_UpdateNodesLevels(SimpleTree<int> tree, Action<SimpleTree<int>> checkAction)
        {
            tree.UpdateNodesLevelsIterative();

            checkAction(tree);
        }

        public static IEnumerable<object[]> Data()
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
