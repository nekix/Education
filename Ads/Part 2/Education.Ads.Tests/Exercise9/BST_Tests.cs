using AlgorithmsDataStructures2;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Education.Ads.Tests.Exercise9
{
    public class BST_Tests
    {
        [Theory]
        [MemberData(nameof(GetTryBalanceEvenTreeData))]
        public void Should_TryBalanceEvenTree(BST<int> bst, List<int> wideNodes, bool res)
        {
            bst.TryBalanceEvenTree().ShouldBe(res);

            var widedNodes = bst.WideAllNodes();

            widedNodes.Count.ShouldBe(wideNodes.Count);

            if (res)
            {
                bool childrenEnd = false;
                for (int i = 0; i < widedNodes.Count; i++)
                {
                    widedNodes[i].NodeKey.ShouldBe(wideNodes[i]);
                    if (childrenEnd)
                    {
                        widedNodes[i].LeftChild.ShouldBeNull();
                        widedNodes[i].RightChild.ShouldBeNull();
                    }
                    else if (widedNodes[i].LeftChild == null || widedNodes[i].RightChild == null)
                    {
                        childrenEnd = true;
                        widedNodes[i].RightChild.ShouldBeNull();
                    }
                }
            }
            else
            {
                widedNodes.Select(n => n.NodeKey).ShouldBe(wideNodes);
            }
        }

        public static IEnumerable<object[]> GetTryBalanceEvenTreeData()
        {
            BST<int> bst;
            List<int> wideNodes;
            bool res;

            // 1: Пустое дерево
            bst = new BST<int>(null);
            wideNodes = new List<int>(0);
            res = false;

            yield return new object[] { bst, wideNodes, res };

            // 2: Один узел
            bst = new BST<int>(new BSTNode<int>(1, 10, null));
            wideNodes = new List<int>() { 1 };
            res = false;
            yield return new object[] { bst, wideNodes, res };

            // 3: Два узла (смещено вправо)
            bst = new BST<int>(new BSTNode<int>(1, 10, null));
            bst.AddKeyValue(2, 20);
            wideNodes = new List<int>() { 2, 1 };
            res = true;
            yield return new object[] { bst, wideNodes, res };

            // 4: Два узла (смещено влево)
            bst = new BST<int>(new BSTNode<int>(2, 10, null));
            bst.AddKeyValue(1, 20);
            wideNodes = new List<int>() { 2, 1 };
            res = true;
            yield return new object[] { bst, wideNodes, res };

            // 5: Три узла (нечетное)
            bst = new BST<int>(new BSTNode<int>(1, 10, null));
            bst.AddKeyValue(2, 20);
            bst.AddKeyValue(3, 30);
            wideNodes = new List<int>() { 1, 2, 3 };
            res = false;
            yield return new object[] { bst, wideNodes, res };

            // 6: Четыре узла (смещены вправо)
            bst = new BST<int>(new BSTNode<int>(1, 10, null));
            bst.AddKeyValue(2, 20);
            bst.AddKeyValue(3, 30);
            bst.AddKeyValue(4, 40);
            wideNodes = new List<int>() { 3, 2, 4, 1 };
            res = true;
            yield return new object[] { bst, wideNodes, res };

            // 7: Четыре узла (смещены влево)
            bst = new BST<int>(new BSTNode<int>(4, 10, null));
            bst.AddKeyValue(3, 20);
            bst.AddKeyValue(2, 30);
            bst.AddKeyValue(1, 40);
            wideNodes = new List<int>() { 3, 2, 4, 1 };
            res = true;
            yield return new object[] { bst, wideNodes, res };

            // 8: Четыре узла (уже сбалансированы)
            bst = new BST<int>(new BSTNode<int>(3, 10, null));
            bst.AddKeyValue(2, 20);
            bst.AddKeyValue(4, 30);
            bst.AddKeyValue(1, 40);
            wideNodes = new List<int>() { 3, 2, 4, 1 };
            res = true;
            yield return new object[] { bst, wideNodes, res };

            // 9: Четыре узла (случайное дерево)
            bst = new BST<int>(new BSTNode<int>(1, 10, null));
            bst.AddKeyValue(3, 20);
            bst.AddKeyValue(2, 30);
            bst.AddKeyValue(4, 40);
            wideNodes = new List<int>() { 3, 2, 4, 1 };
            res = true;
            yield return new object[] { bst, wideNodes, res };

            // 10：Большое дерево (нечетное)
            bst = GetDefaultTree();
            bst.DeleteNodeByKey(8);
            bst.DeleteNodeByKey(4);
            bst.DeleteNodeByKey(6);
            bst.DeleteNodeByKey(7);
            bst.DeleteNodeByKey(5);
            bst.DeleteNodeByKey(1);
            bst.DeleteNodeByKey(15);
            bst.DeleteNodeByKey(12);
            bst.DeleteNodeByKey(14);
            bst.DeleteNodeByKey(13);
            wideNodes = new List<int>() { 9, 2, 10, 3, 11 };
            res = false;
            yield return new object[] { bst, wideNodes, res };

            // 11：Большое дерево (четноё)
            bst = GetDefaultTree();
            bst.DeleteNodeByKey(8);
            bst.DeleteNodeByKey(4);
            bst.DeleteNodeByKey(6);
            bst.DeleteNodeByKey(7);
            bst.DeleteNodeByKey(5);
            bst.DeleteNodeByKey(1);
            bst.DeleteNodeByKey(15);
            bst.DeleteNodeByKey(12);
            bst.DeleteNodeByKey(14);
            bst.DeleteNodeByKey(13);
            bst.AddKeyValue(25, 1001);
            wideNodes = new List<int>() { 10, 3, 25, 2, 9, 11 };
            res = true;
            yield return new object[] { bst, wideNodes, res };
        }

        public static BST<int> GetDefaultTree()
        {
            BST<int> bst = new BST<int>(new BSTNode<int>(8, 100, null));
            bst.AddKeyValue(4, 101);
            bst.AddKeyValue(12, 102);
            bst.AddKeyValue(2, 103);
            bst.AddKeyValue(6, 104);
            bst.AddKeyValue(10, 105);
            bst.AddKeyValue(14, 106);
            bst.AddKeyValue(1, 107);
            bst.AddKeyValue(3, 108);
            bst.AddKeyValue(5, 109);
            bst.AddKeyValue(7, 110);
            bst.AddKeyValue(9, 111);
            bst.AddKeyValue(11, 112);
            bst.AddKeyValue(13, 114);
            bst.AddKeyValue(15, 115);

            return bst;
        }
    }
}
