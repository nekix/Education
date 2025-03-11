using AlgorithmsDataStructures2;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Education.Ads.Tests.Exercise11
{
    public class SimpleTree_Tests
    {
        [Theory]
        [MemberData(nameof(GetMaxPathLength))]
        public void Should_GetMaxPathLength(SimpleTree<int> tree, int res)
        {
            tree.GetMaxPathLength().ShouldBe(res);
        }

        public static IEnumerable<object[]> GetMaxPathLength()
        {
            SimpleTree<int> tree;
            int res;

            // 1: Пустое дерево
            tree = new SimpleTree<int>(null);
            res = 0;
            yield return new object[] { tree, res };

            // 2: Единственный узел
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            res = 0;
            yield return new object[] { tree, res };

            // 3: Два узла (берём корневой)
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(2, null));
            res = 1;
            yield return new object[] { tree, res };

            // 4: Большое дерево (Root узел)  
            //               ___________________________500_________________________
            //              /                      /                 \              \
            //        ____510____                 520            _____530_____      531
            //       /           \               /   \          /             \  
            //     540           550           560  562     __570__           580
            //    /   \        /  |  \         /           /   |   \          /  \
            //  590   591    600 601 602     610         620  621  622      630  631
            //                                                 |
            //                                                700
            //                                                 |
            //                                                800

            tree = new SimpleTree<int>(new SimpleTreeNode<int>(500, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(510, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(520, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(530, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(531, null));
            tree.AddChild(tree.Root.Children[0], new SimpleTreeNode<int>(540, null));
            tree.AddChild(tree.Root.Children[0], new SimpleTreeNode<int>(550, null));
            tree.AddChild(tree.Root.Children[1], new SimpleTreeNode<int>(560, null));
            tree.AddChild(tree.Root.Children[1], new SimpleTreeNode<int>(562, null));
            tree.AddChild(tree.Root.Children[2], new SimpleTreeNode<int>(570, null));
            tree.AddChild(tree.Root.Children[2], new SimpleTreeNode<int>(580, null));
            tree.AddChild(tree.Root.Children[0].Children[0], new SimpleTreeNode<int>(590, null));
            tree.AddChild(tree.Root.Children[0].Children[0], new SimpleTreeNode<int>(591, null));
            tree.AddChild(tree.Root.Children[0].Children[1], new SimpleTreeNode<int>(600, null));
            tree.AddChild(tree.Root.Children[0].Children[1], new SimpleTreeNode<int>(601, null));
            tree.AddChild(tree.Root.Children[0].Children[1], new SimpleTreeNode<int>(602, null));
            tree.AddChild(tree.Root.Children[1].Children[0], new SimpleTreeNode<int>(610, null));
            tree.AddChild(tree.Root.Children[2].Children[0], new SimpleTreeNode<int>(620, null));
            tree.AddChild(tree.Root.Children[2].Children[0], new SimpleTreeNode<int>(621, null));
            tree.AddChild(tree.Root.Children[2].Children[0], new SimpleTreeNode<int>(622, null));
            tree.AddChild(tree.Root.Children[2].Children[1], new SimpleTreeNode<int>(630, null));
            tree.AddChild(tree.Root.Children[2].Children[1], new SimpleTreeNode<int>(631, null));
            tree.AddChild(tree.Root.Children[2].Children[0].Children[1], new SimpleTreeNode<int>(700, null));
            tree.AddChild(tree.Root.Children[2].Children[0].Children[1].Children[0], new SimpleTreeNode<int>(800, null));
            res = 8;
            yield return new object[] { tree, res };
        }
    }
}
