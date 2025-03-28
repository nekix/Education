using AlgorithmsDataStructures2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise2_3
{
    public class BSTInt_Tests
    {
        [Theory]
        [MemberData(nameof(GetMaxValuePathsData))]
        public void Should_GetMaxValuePathsIterative(BSTInt tree, List<List<BSTNode<int>>> paths)
        {
            var results = tree.GetMaxValuePathsIterative();

            results.Count.ShouldBe(paths.Count);
            for (int i = 0; i < paths.Count; i++)
                results[i].ShouldBe(paths[i]);
        }

        [Theory]
        [MemberData(nameof(GetMaxValuePathsData))]
        public void Should_GetMaxValuePathsRecursive(BSTInt tree, List<List<BSTNode<int>>> paths)
        {
            var results = tree.GetMaxValuePathsRecursive();

            results.Count.ShouldBe(paths.Count);
            for (int i = 0; i < paths.Count; i++)
                results[i].ShouldBe(paths[i]);
        }

        [Theory]
        [MemberData(nameof(GetGetLevelWithMaxValueSumData))]
        public void Should_GetLevelWithMaxValueSum(BSTInt tree, int level)
        {
            tree.GetLevelWithMaxValueSum().ShouldBe(level);
        }

        public static IEnumerable<object[]> GetMaxValuePathsData()
        {
            // 1: Пустое дерево
            var tree = new BSTInt(null);
            yield return new object[] { tree, new List<List<BSTNode<int>>>(0) };

            // 2: Только корневой
            tree = new BSTInt(new BSTNode<int>(8, 100, null));
            yield return new object[] { tree, new List<List<BSTNode<int>>>(0) };

            // 3: Основное дерево
            tree = GetDefaultTree();
            var paths = new List<List<BSTNode<int>>>
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
            yield return new object[] { tree, paths };

            // 4: Основное дерево, удалено 2 узла и изменен 1
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(17);
            tree.DeleteNodeByKey(19);
            tree.FindNodeByKey(7).Node.NodeValue += 7;
            paths = new List<List<BSTNode<int>>>
            {
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
                    tree.FindNodeByKey(14).Node,
                    tree.FindNodeByKey(15).Node,
                },
            };
            yield return new object[] { tree, paths };
        }

        public static IEnumerable<object[]> GetGetLevelWithMaxValueSumData()
        {
            // 1: Пустое дерево
            var tree = new BSTInt(null);
            yield return new object[] { tree, -1 };

            // 2: Только корневой
            tree = new BSTInt(new BSTNode<int>(8, 100, null));
            yield return new object[] { tree, 0 };

            // 3: Основное дерево
            tree = GetDefaultTree();
            yield return new object[] { tree, 3 };

            // 4: Основное дерево, удалено 2 узла и изменен 1
            tree = GetDefaultTree();
            tree.DeleteNodeByKey(17);
            tree.FindNodeByKey(19).Node.NodeValue = 100000;
            yield return new object[] { tree, 4 };

            // 5: Два уровня с одинаковой суммой
            tree = GetDefaultTree();
            tree.FindNodeByKey(12).Node.NodeValue = 1399;
            tree.FindNodeByKey(6).Node.NodeValue = 1186;
            yield return new object[] { tree, 1 };
        }

        public static BSTInt GetDefaultTree()
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
            return new BSTInt(root);
        }
    }
}
