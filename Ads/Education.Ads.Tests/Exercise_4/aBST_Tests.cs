using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmsDataStructures2;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise_4
{
    public class aBST_Tests
    {
        [Theory]
        [MemberData(nameof(GetInitData))]
        public void Should_Init(int depth, int count)
        {
            aBST arrTree = new aBST(depth);

            arrTree.Tree.ShouldNotBeNull();
            arrTree.Tree.Length.ShouldBe(count);
            arrTree.Tree.ShouldAllBe(i => i == null);
        }

        [Theory]
        [MemberData(nameof(GetAddKeyData))]
        public void Should_AddKey(aBST arrTree, int key, int index, int?[] keys)
        {
            arrTree.AddKey(key).ShouldBe(index);

            index.ShouldBeGreaterThan(-2);

            if (index == -1)
                arrTree.Tree.ShouldNotContain(key);
            else
                arrTree.Tree[index].ShouldBe(key);

            arrTree.Tree.ShouldBe(keys);
        }

        [Theory]
        [MemberData(nameof(GetFindKeyData))]
        public void Should_FindKeyIndex(aBST tree, int key, int? index)
        {
            tree.FindKeyIndex(key).ShouldBe(index);
        }

        [Theory]
        [MemberData(nameof(GetLcaIndexByIndexesData))]
        public void Should_GetLcaIndexByIndexes(aBST tree, int firstIndex, int secondIndex, int? result)
        {
            tree.GetLcaIndexByIndexes(firstIndex, secondIndex).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(GetLcaIndexByKeysData))]
        public void Should_GetLcaIndexByKeys(aBST tree, int firstKey, int secondKey, int? result)
        {
            tree.GetLcaIndexByKeys(firstKey, secondKey).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(GetWideAllNodesData))]
        public void Should_WideAllNodes(aBST tree, List<int> result)
        {
            tree.WideAllNodes().ShouldBe(result);
        }

        public static IEnumerable<object[]> GetInitData()
        {
            // 1: Нулевой уровень
            yield return new object[] { 0, 1 };

            // 2: Первый уровень
            yield return new object[] { 1, 3 };

            // 3: Второй уровень
            yield return new object[] { 2, 7 };

            // 4: Третий уровень
            yield return new object[] { 3, 15 };

            // 5: Четвертый уровень
            yield return new object[] { 4, 31 };
        }

        public static IEnumerable<object[]> GetAddKeyData()
        {
            // 1: Нулевая глубина, пустое, несуществующий узел
            var tree = new aBST(0);
            yield return new object[] { tree, 2, 0, new int?[] { 2 } };

            // 2: Нулевая глубина, заполнено полностью, существующий узел
            tree = new aBST(0);
            tree.Tree[0] = 2;
            yield return new object[] { tree, 2, 0, new int?[] { 2 } };

            // 3: Нулевая глубина,заполнено полностью, не существующий узел
            tree = new aBST(0);
            tree.Tree[0] = 3;
            yield return new object[] { tree, 2, -1, new int?[] { 3 } };

            // 4: Как на картинке в уроке
            // 3 глубина, заполнено не полностью
            // новый узел левый
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[14] = 92;
            var comArray = tree.Tree.ToArray();
            comArray[3] = 22;
            yield return new object[] { tree, 22, 3, comArray };

            // 5: Как на картинке в уроке
            // 3 глубина, заполнено не полностью
            // новый узел правый
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[14] = 92;
            comArray = tree.Tree.ToArray();
            comArray[12] = 70;
            yield return new object[] { tree, 70, 12, comArray };

            // 5: Как на картинке в уроке
            // 3 глубина, заполнено не полностью
            // новый узел правый после заполнения
            tree = new aBST(3);
            tree.AddKey(50);
            tree.AddKey(25);
            tree.AddKey(75);
            tree.AddKey(37);
            tree.AddKey(62);
            tree.AddKey(84);
            tree.AddKey(31);
            tree.AddKey(43);
            tree.AddKey(55);
            tree.AddKey(92);
            comArray = new int?[] { 50, 25, 75, null, 37, 62, 84, null, null, 31, 43, 55, null, null, 92 };
            comArray[12] = 70;
            yield return new object[] { tree, 70, 12, comArray };

            // 6: Как на картинке в уроке
            // 3 глубина, заполнено не полностю
            // существующий узел
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[14] = 92;
            comArray = new int?[] { 50, 25, 75, null, 37, 62, 84, null, null, 31, 43, 55, null, null, 92 };
            yield return new object[] { tree, 37, 4, comArray };

            // 7: Как на картинке в уроке
            // 3 глубина, заполнено полностью
            // новый узел
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[3] = 15;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[7] = 5;
            tree.Tree[8] = 20;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[12] = 66;
            tree.Tree[13] = 77;
            tree.Tree[14] = 92;
            comArray = tree.Tree.ToArray();
            yield return new object[] { tree, 39, -1, comArray };

            // 8: Как на картинке в уроке
            // 3 глубина, заполнено полностью
            // существующий узел
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[3] = 15;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[7] = 5;
            tree.Tree[8] = 20;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[12] = 66;
            tree.Tree[13] = 77;
            tree.Tree[14] = 92;
            comArray = tree.Tree.ToArray();
            yield return new object[] { tree, 92, 14, comArray };
        }

        public static IEnumerable<object[]> GetFindKeyData()
        {
            // 1: Нулевая глубина, пустое, несуществующий узел
            var tree = new aBST(0);
            yield return new object[] { tree, 2, 0 };

            // 2: Нулевая глубина, заполнено полностью, существующий узел
            tree = new aBST(0);
            tree.Tree[0] = 2;
            yield return new object[] { tree, 2, 0 };

            // 3: Нулевая глубина,заполнено полностью, не существующий узел
            tree = new aBST(0);
            tree.Tree[0] = 3;
            yield return new object[] { tree, 2, null };

            // 4: Как на картинке в уроке
            // 3 глубина, заполнено не полностью
            // новый узел левый
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[14] = 92;
            yield return new object[] { tree, 22, -3 };

            // 5: Как на картинке в уроке
            // 3 глубина, заполнено не полностью
            // новый узел правый
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[14] = 92;
            yield return new object[] { tree, 70, -12 };

            // 5: Как на картинке в уроке
            // 3 глубина, заполнено не полностью
            // новый узел правый после заполнения
            tree = new aBST(3);
            tree.AddKey(50);
            tree.AddKey(25);
            tree.AddKey(75);
            tree.AddKey(37);
            tree.AddKey(62);
            tree.AddKey(84);
            tree.AddKey(31);
            tree.AddKey(43);
            tree.AddKey(55);
            tree.AddKey(92);
            yield return new object[] { tree, 70, -12 };

            // 6: Как на картинке в уроке
            // 3 глубина, заполнено не полностю
            // существующий узел
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[14] = 92;
            yield return new object[] { tree, 37, 4 };

            // 7: Как на картинке в уроке
            // 3 глубина, заполнено полностью
            // новый узел
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[3] = 15;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[7] = 5;
            tree.Tree[8] = 20;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[12] = 66;
            tree.Tree[13] = 77;
            tree.Tree[14] = 92;
            yield return new object[] { tree, 39, null };

            // 8: Как на картинке в уроке
            // 3 глубина, заполнено полностью
            // существующий узел
            tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[3] = 15;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[7] = 5;
            tree.Tree[8] = 20;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[12] = 66;
            tree.Tree[13] = 77;
            tree.Tree[14] = 92;
            yield return new object[] { tree, 92, 14 };
        }

        public static IEnumerable<object[]> GetLcaIndexByIndexesData()
        {
            // 1: Несуществующий узел
            var tree = GetDefaultTree();
            yield return new object[] { tree, 22, 13, null };

            // 2: Root узел
            tree = GetDefaultTree();
            yield return new object[] { tree, 0, 0, 0 };

            // 3: На одной ветви (левой)
            tree = GetDefaultTree();
            yield return new object[] { tree, 4, 9, 4 };

            // 4: На одной ветви (правой)
            tree = GetDefaultTree();
            yield return new object[] { tree, 10, 4, 4 };

            // 5: На разных ветвях (соседние)
            tree = GetDefaultTree();
            yield return new object[] { tree, 9, 10, 4 };

            // 6: На разных ветвях (средне)
            tree = GetDefaultTree();
            yield return new object[] { tree, 11, 14, 2 };

            // 7: На разных ветвях (далеко)
            tree = GetDefaultTree();
            yield return new object[] { tree, 10, 11, 0 };

            // 8: На разных ветвях (далеко)
            tree = GetDefaultTree();
            yield return new object[] { tree, 10, 14, 0 };
        }

        public static IEnumerable<object[]> GetLcaIndexByKeysData()
        {
            // 1: Несуществующий узел
            var tree = GetDefaultTree();
            yield return new object[] { tree, 22, 13, null };

            // 2: Root узел
            tree = GetDefaultTree();
            yield return new object[] { tree, 50, 50, 0 };

            // 3: На одной ветви (левой)
            tree = GetDefaultTree();
            yield return new object[] { tree, 37, 31, 4 };

            // 4: На одной ветви (правой)
            tree = GetDefaultTree();
            yield return new object[] { tree, 43, 37, 4 };

            // 5: На разных ветвях (соседние)
            tree = GetDefaultTree();
            yield return new object[] { tree, 31, 43, 4 };

            // 6: На разных ветвях (средне)
            tree = GetDefaultTree();
            yield return new object[] { tree, 55, 92, 2 };

            // 7: На разных ветвях (далеко)
            tree = GetDefaultTree();
            yield return new object[] { tree, 43, 55, 0 };

            // 8: На разных ветвях (далеко)
            tree = GetDefaultTree();
            yield return new object[] { tree, 43, 92, 0 };
        }

        public static IEnumerable<object[]> GetWideAllNodesData()
        {
            // 1: Нулевая глубина, пустое, несуществующий узел
            var tree = new aBST(0);
            yield return new object[] { tree, new List<int>(0) };

            // 2: Нулевая глубина, заполнено
            tree = new aBST(0);
            tree.Tree[0] = 2;
            yield return new object[] { tree, new List<int> { 2 } };

            // 3: Как на картинке в уроке
            tree = GetDefaultTree();
            yield return new object[] { tree, new List<int> { 50, 25, 75, 37, 62, 84, 31, 43, 55, 92 } };

            // 4: Как на картинке в уроке
            // но заполнено полностью
            tree = GetDefaultTree();
            tree.Tree[3] = 15;
            tree.Tree[7] = 5;
            tree.Tree[8] = 20;
            tree.Tree[12] = 66;
            tree.Tree[13] = 77;
            yield return new object[] { tree, new List<int> { 50, 25, 75, 15, 37, 62, 84, 5, 20, 31, 43, 55, 66, 77, 92 } };
        }

        private static aBST GetDefaultTree()
        {
            var tree = new aBST(3);
            tree.Tree[0] = 50;
            tree.Tree[1] = 25;
            tree.Tree[2] = 75;
            tree.Tree[4] = 37;
            tree.Tree[5] = 62;
            tree.Tree[6] = 84;
            tree.Tree[9] = 31;
            tree.Tree[10] = 43;
            tree.Tree[11] = 55;
            tree.Tree[14] = 92;
            return tree;
        }
    }
}
