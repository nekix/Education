using AlgorithmsDataStructures2;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Education.Ads.Tests.Exercise1
{
    public class SimpleTree_Tests
    {
        [Theory]
        [MemberData(nameof(AddChildData))]
        public void Should_AddChild(SimpleTree<int> tree, SimpleTreeNode<int> parentNode, SimpleTreeNode<int> nodeToAdd)
        {
            tree.AddChild(parentNode, nodeToAdd);

            var nodes = new List<SimpleTreeNode<int>>();
            nodes.Add(tree.Root);

            bool isHasNode = false;

            for(int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Equals(nodeToAdd))
                {
                    isHasNode = true;
                    break;
                }

                if (nodes[i].Children != null)
                    nodes.AddRange(nodes[i].Children);
            }

            Assert.True(isHasNode);
        }

        [Theory]
        [MemberData(nameof(DeleteNodeData))]
        public void Should_DeleteNode(SimpleTree<int> tree, SimpleTreeNode<int> nodeToDelete, bool isRootNode, int nodesCount)
        {
            tree.DeleteNode(nodeToDelete);

            var nodes = new List<SimpleTreeNode<int>>();

            if(tree.Root != null)
                nodes.Add(tree.Root);

            bool isHasNode = false;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Equals(nodeToDelete))
                {
                    isHasNode = true;
                    break;
                }

                if (nodes[i].Children != null)
                    nodes.AddRange(nodes[i].Children);
            }

            Assert.False(isHasNode);

            nodes.Count.ShouldBe(nodesCount);
        }

        [Theory]
        [MemberData(nameof(GetAllNodesData))]
        public void Should_GetAllNodes(SimpleTree<int> tree, int count)
        {
            tree.GetAllNodes().Count.ShouldBe(count);
        }

        [Theory]
        [MemberData(nameof(FindNodesByValueData))]
        public void Should_FindNodesByValue(SimpleTree<int> tree, int value, List<SimpleTreeNode<int>> nodes)
        {
            var result = tree.FindNodesByValue(value);
            result = result.OrderBy(x => x.GetHashCode()).ToList(); 
            
            var bRes = result.SequenceEqual(nodes
                    .OrderBy(x => x.GetHashCode()).ToList());
            
            bRes.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(CountData))]
        public void Should_Count(SimpleTree<int> tree, int count)
        {
            tree.Count().ShouldBe(count);
        }

        [Theory]
        [MemberData(nameof(LeafCountData))]
        public void Should_LeafCount(SimpleTree<int> tree, int count)
        {
            tree.LeafCount().ShouldBe(count);
        }

        [Theory]
        [MemberData(nameof(MoveNodeData))]
        public void Should_MoveNode(SimpleTree<int> tree,
            SimpleTreeNode<int> originalNode,
            SimpleTreeNode<int> newParent,
            Action<SimpleTree<int>> checkAction)
        {
            tree.MoveNode(originalNode, newParent);

            checkAction(tree);
        }

        public static IEnumerable<object[]> AddChildData()
        {
            // 1: To ROOT
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var parentNode = tree.Root;
            var nodeToAdd = new SimpleTreeNode<int>(1, null);
            yield return new object[] { tree, parentNode, nodeToAdd };

            // 2: To second level withoud children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            parentNode = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { parentNode };
            nodeToAdd = new SimpleTreeNode<int>(2, null);
            yield return new object[] { tree, parentNode, nodeToAdd };

            // 3: To second level with children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            parentNode = new SimpleTreeNode<int>(1, tree.Root);
            parentNode.Children = new List<SimpleTreeNode<int>> { new SimpleTreeNode<int>(3, parentNode) };
            tree.Root.Children = new List<SimpleTreeNode<int>> { parentNode };
            nodeToAdd = new SimpleTreeNode<int>(2, null);
            yield return new object[] { tree, parentNode, nodeToAdd };
        }

        public static IEnumerable<object[]> DeleteNodeData()
        {
            // 1: Root
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var nodeToDelete = tree.Root;
            yield return new object[] { tree, nodeToDelete, true, 0 };

            // 2: Root child
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            nodeToDelete = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { nodeToDelete };
            yield return new object[] { tree, nodeToDelete, false, 1 };

            // 3: Root child with children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            nodeToDelete = new SimpleTreeNode<int>(1, tree.Root);
            nodeToDelete.Children = new List<SimpleTreeNode<int>> { new SimpleTreeNode<int>(3, nodeToDelete) };
            tree.Root.Children = new List<SimpleTreeNode<int>> { nodeToDelete };
            yield return new object[] { tree, nodeToDelete, false, 1 };
        }

        public static IEnumerable<object[]> GetAllNodesData()
        {
            // 1: Empty
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.Root = null;
            yield return new object[] { tree, 0 };

            // 1:Only root 
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            yield return new object[] { tree, 1 };

            // 2: Root and child
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var rootChild = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 2 };

            // 3: Root and two children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            rootChild = new SimpleTreeNode<int>(1, tree.Root);
            rootChild.Children = new List<SimpleTreeNode<int>> { new SimpleTreeNode<int>(3, rootChild) };
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 3 };
        }

        public static IEnumerable<object[]> CountData()
        {
            // 1: Empty
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.Root = null;
            yield return new object[] { tree, 0 };

            // 1:Only root 
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            yield return new object[] { tree, 1 };

            // 2: Root and child
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var rootChild = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 2 };

            // 3: Root and two children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            rootChild = new SimpleTreeNode<int>(1, tree.Root);
            rootChild.Children = new List<SimpleTreeNode<int>> { new SimpleTreeNode<int>(3, rootChild) };
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 3 };
        }

        public static IEnumerable<object[]> LeafCountData()
        {
            // 1: Empty
            var tree = new SimpleTree<int>(null);
            yield return new object[] { tree, 0 };

            // 2: Only root
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            yield return new object[] { tree, 1 };

            // 3: Only root with empty children list (not null!)
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            tree.Root.Children = new List<SimpleTreeNode<int>>();
            yield return new object[] { tree, 1 };

            // 4: Root with child
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var rootChild = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 1 };

            // 5: Root with child with child
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            rootChild = new SimpleTreeNode<int>(1, tree.Root);
            rootChild.Children = new List<SimpleTreeNode<int>> { new SimpleTreeNode<int>(3, rootChild) };
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 1 };

            // 6: Root with two children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            rootChild = new SimpleTreeNode<int>(1, tree.Root);
            rootChild.Children = new List<SimpleTreeNode<int>>
            {
                { new SimpleTreeNode<int>(3, rootChild) },
                { new SimpleTreeNode<int>(4, rootChild) }
            };
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 2 };

            // 7: Root with two children and one of them with two children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            rootChild = new SimpleTreeNode<int>(1, tree.Root);
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
            yield return new object[] { tree, 3 };
        }

        public static IEnumerable<object[]> FindNodesByValueData()
        {
            // 1: Empty tree
            var tree = new SimpleTree<int>(null);
            yield return new object[] { tree, 0, new List<SimpleTreeNode<int>>() };

            // 2: Tree with right value root
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            yield return new object[] 
            { 
                tree,
                0,
                new List<SimpleTreeNode<int>>() 
                {
                    tree.Root,
                }
            };
            
            // 3: Tree with wrong value root
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            yield return new object[]
            {
                tree,
                1,
                new List<SimpleTreeNode<int>>()
            };

            // 4: Root with child
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var rootChild = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 1, new List<SimpleTreeNode<int>>() { rootChild } };

            // 5: Root with child
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            rootChild = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild };
            yield return new object[] { tree, 1, new List<SimpleTreeNode<int>>() { rootChild, tree.Root } };

            // 6: Root with children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            rootChild = new SimpleTreeNode<int>(1, tree.Root);
            var node31 = new SimpleTreeNode<int>(3, rootChild);
            var node32 = new SimpleTreeNode<int>(3, rootChild);
            rootChild.Children = new List<SimpleTreeNode<int>>
            {
                { node31 },
                { node32 }
            };
            tree.Root.Children = new List<SimpleTreeNode<int>>
            {
                rootChild,
                new SimpleTreeNode<int>(5, tree.Root)
            };
            yield return new object[] { tree, 3, new List<SimpleTreeNode<int>>() { node31, node32 } };
        }

        public static IEnumerable<object[]> MoveNodeData()
        {
            // 1: Move root node (do nothing)
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var rootChild1 = new SimpleTreeNode<int>(1, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>> { rootChild1 };
            Action<SimpleTree<int>> action = (treeAct) =>
            {
                treeAct.Root.NodeValue.ShouldBe(0);
                treeAct.Root.Parent.ShouldBeNull();
                treeAct.Root.Children.ShouldNotBeNull();
                treeAct.Root.Children.Count.ShouldBe(1);
                treeAct.Root.Children[0].ShouldBe(rootChild1);
                treeAct.Root.Children[0].Parent.ShouldBe(treeAct.Root);
                treeAct.Root.Children[0].Children.ShouldBeNull();
            };
            yield return new object[] { tree, tree.Root, rootChild1, action };

            // 2: Move root child in second root child with all children
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(0, null));
            var rootChild2 = new SimpleTreeNode<int>(1, tree.Root);
            rootChild2.Children = new List<SimpleTreeNode<int>>
            {
                { new SimpleTreeNode<int>(3, rootChild2) },
                { new SimpleTreeNode<int>(4, rootChild2) }
            };
            var secondRootChild = new SimpleTreeNode<int>(5, tree.Root);
            tree.Root.Children = new List<SimpleTreeNode<int>>
            {
                rootChild2,
                secondRootChild,
            };
            action = (treeAct) =>
            {
                treeAct.Root.NodeValue.ShouldBe(0);
                treeAct.Root.Parent.ShouldBeNull();
                treeAct.Root.Children.ShouldNotBeNull();
                treeAct.Root.Children.Count.ShouldBe(1);
                treeAct.Root.Children[0].ShouldBe(secondRootChild);
                treeAct.Root.Children[0].Parent.ShouldBe(treeAct.Root);
                treeAct.Root.Children[0].Children.Count.ShouldBe(1);
                treeAct.Root.Children[0].Children[0].ShouldBe(rootChild2);
                treeAct.Root.Children[0].Children[0].Children.Count.ShouldBe(2);
                treeAct.Root.Children[0].Children[0].Children.ShouldAllBe(c => c.NodeValue.Equals(3) || c.NodeValue.Equals(4));
            };
            yield return new object[] { tree, rootChild2, secondRootChild, action };
        }
    }
}