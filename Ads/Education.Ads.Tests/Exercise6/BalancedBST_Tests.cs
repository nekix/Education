extern alias Exercise6;
using System.Collections.Generic;
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

        public void Should_IsBalanced(int[] a, bool result)
        {

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
            parents = new int?[] { null, 50, 50, 25, 25, 75, 75, 10, 10, 37, 37, 62, 62, 84, 84 };
            levels = new int[] { 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3 };
            yield return new object[] { a, keys, parents, levels };
        }
    }
}
