using System.Collections.Generic;
using AlgorithmsDataStructures2;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise9
{
    public class SimpleTree_Tests
    {
        [Theory]
        [MemberData(nameof(GetEvenTreesData))]
        public void Should_EvenTrees(SimpleTree<int> tree, List<int> vertices)
        {
            tree.EvenTrees().ShouldBe(vertices);
        }

        [Theory]
        [MemberData(nameof(GetCountEvenTreesData))]
        public void Should_CountEvenTrees(SimpleTree<int> tree, SimpleTreeNode<int> node, int result)
        {
            tree.CountEventTrees(node).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetCountEvenTreesData()
        {
            SimpleTree<int> tree;
            SimpleTreeNode<int> node;
            int res;

            // 1: Единственный узел
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            node = tree.Root;
            res = 0;
            yield return new object[] { tree, node, res };

            // 2: Два узла (берём корневой)
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(2, null));
            node = tree.Root;
            res = 1;
            yield return new object[] { tree, node, res };

            // 3: Два узла (берём дочерний)
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(2, null));
            node = tree.Root.Children[0];
            res = 0;
            yield return new object[] { tree, node, res };

            // 4: Большое дерево (Root узел)  
            //               __________________________(500)________________________
            //              /                      /                 \              \
            //        ____510____                 520            _____530_____      531
            //       /           \               /   \          /             \  
            //     540           550           560  562     __570__           580
            //    /   \        /  |  \         /           /   |   \          /  \
            //  590   591    600 601 602     610         620  621  622      630  631

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
            node = tree.Root;
            res = 7;
            yield return new object[] { tree, node, res };

            // 5: Большое дерево ("средний" узел)  
            //               ___________________________500_________________________
            //              /                      /                 \              \
            //        ____510____                 520            ____(530)____      531
            //       /           \               /   \          /             \  
            //     540           550           560  562     __570__           580
            //    /   \        /  |  \         /           /   |   \          /  \
            //  590   591    600 601 602     610         620  621  622      630  631

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
            node = tree.Root.Children[2];
            res = 2;
            yield return new object[] { tree, node, res };

            // 6: Большое дерево (лист)  
            //               ___________________________500_________________________
            //              /                      /                 \              \
            //        ____510____                 520            ____(530)____      531
            //       /           \               /   \          /             \  
            //     540           550           560  562     __570__           580
            //    /   \        /  |  \         /           /   |   \          /  \
            //  590   591    600 601 602     610         620  621  622      630  631

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
            node = tree.Root.Children[3];
            res = 0;
            yield return new object[] { tree, node, res };
        }

        public static IEnumerable<object[]> GetEvenTreesData()
        {
            // 1: Пустое дерево
            SimpleTree<int> tree = new SimpleTree<int>(null);
            List<int> vertices = new List<int> { };
            yield return new object[] { tree, vertices };

            // 2: Один элемент
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            vertices = new List<int> { };
            yield return new object[] { tree, vertices };

            // 3: Дерево из урока
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(6, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(3, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(2, null));
            tree.AddChild(tree.Root.Children[0], new SimpleTreeNode<int>(8, null));
            tree.AddChild(tree.Root.Children[1], new SimpleTreeNode<int>(4, null));
            tree.AddChild(tree.Root.Children[2], new SimpleTreeNode<int>(7, null));
            tree.AddChild(tree.Root.Children[2], new SimpleTreeNode<int>(5, null));
            tree.AddChild(tree.Root.Children[0].Children[0], new SimpleTreeNode<int>(10, null));
            tree.AddChild(tree.Root.Children[0].Children[0], new SimpleTreeNode<int>(9, null));
            vertices = new List<int> { 1, 6, 1, 3 };
            yield return new object[] { tree, vertices };

            // 4: Дерево из урока с доп узлом (нечетное)
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(6, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(3, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(2, null));
            tree.AddChild(tree.Root.Children[0], new SimpleTreeNode<int>(8, null));
            tree.AddChild(tree.Root.Children[1], new SimpleTreeNode<int>(4, null));
            tree.AddChild(tree.Root.Children[2], new SimpleTreeNode<int>(7, null));
            tree.AddChild(tree.Root.Children[2], new SimpleTreeNode<int>(5, null));
            tree.AddChild(tree.Root.Children[0].Children[0], new SimpleTreeNode<int>(10, null));
            tree.AddChild(tree.Root.Children[0].Children[0], new SimpleTreeNode<int>(9, null));
            tree.AddChild(tree.Root.Children[2].Children[1], new SimpleTreeNode<int>(13, null));
            vertices = new List<int> { };
            yield return new object[] { tree, vertices };

            // 5: Большое дерево
            //               ___________________________500_________________________
            //              /                      /                 \              \
            //        ____510____                 520            _____530_____       531
            //       /           \               /   \          /             \  
            //     540           550           560  562     __570__           580
            //    /   \        /  |  \         /           /   |   \          /  \
            //  590   591    600 601 602     610         620  621  622      630  631

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
            vertices = new List<int> { 510, 550, 500, 510, 520, 560, 500, 520, 530, 570, 500, 530 };
            yield return new object[] { tree, vertices };

            // 6: Большое дерево, нечетное
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(500, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(510, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(520, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(530, null));
            tree.AddChild(tree.Root, new SimpleTreeNode<int>(531, null));
            tree.AddChild(tree.Root.Children[0], new SimpleTreeNode<int>(540, null));
            tree.AddChild(tree.Root.Children[0], new SimpleTreeNode<int>(550, null));
            tree.AddChild(tree.Root.Children[1], new SimpleTreeNode<int>(560, null));
            tree.AddChild(tree.Root.Children[1], new SimpleTreeNode<int>(561, null));
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
            vertices = new List<int> { };
            yield return new object[] { tree, vertices };
        }
    }
}
