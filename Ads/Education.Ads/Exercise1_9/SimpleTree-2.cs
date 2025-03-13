using System.Collections.Generic;
using System;

namespace AlgorithmsDataStructures2
{
    // Урок 9

    // Задание 2*.
    // Метод, балансирующий чётное двоичное дерево (TryBalanceEvenTree).
    // Сложность временная: O(N), где N - число узлов дерева.
    // Сложность пространственная: O(N), где N - число узлов дерева.
    // Проводим in order обход, получаем отсортированный список узлов.
    // Далее распределяю узлы (выбираю middle) так, чтобы нижний уровень
    // был заполнен слева направо (округляю до большего целого).
    public partial class BST<T>
    {
        public bool TryBalanceEvenTree()
        {
            List<BSTNode<T>> nodes = DeepAllNodes(0);

            if (nodes.Count == 0 || nodes.Count % 2 != 0)
                return false;

            BSTNode<T> rootNode = BalanceEvenTree(nodes, 0, nodes.Count - 1);

            Root = rootNode;

            return true;
        }

        private BSTNode<T> BalanceEvenTree(List<BSTNode<T>> nodes, int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
                return null;

            int middle = (int)Math.Ceiling((startIndex + endIndex) / 2d);

            BSTNode<T> root = nodes[middle];
            root.Parent = null;

            root.LeftChild = BalanceEvenTree(nodes, startIndex, middle - 1);
            if (root.LeftChild != null)
                root.LeftChild.Parent = root;

            root.RightChild = BalanceEvenTree(nodes, middle + 1, endIndex);
            if (root.RightChild != null)
                root.RightChild.Parent = root;

            return root;
        }

        // Оригинальная публичная версия метода (ниже) DeepAllNodesRecursive(int order)
        // возвращала BSTNode (не generic), поэтому сделал универсальную обёртку.
        public List<BSTNode<T>> DeepAllNodes(int order)
        {
            if (Root == null)
                return new List<BSTNode<T>>(0);

            if (order < 0 || order > 2)
                return new List<BSTNode<T>>(0);

            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            DeepAllNodesRecursive(order, Root, nodes);

            return nodes;
        }

        /* 
        public List<BSTNode> DeepAllNodesRecursive(int order)
        {
            if (Root == null)
                return new List<BSTNode>(0);

            if (order < 0 || order > 2)
                return new List<BSTNode>(0);

            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            DeepAllNodesRecursive(order, Root, nodes);

            // Оригинальная версия приводила List<BSTNode<T>>
            // к List<BSTNode>.
            return ConvertToBstNode(nodes);
        }
        */
    }

    // Задание 3*
    // Метод, определяющий кол-во чётных поддеревьев для заданного подузла (CountEvenTrees).
    // Сложность временная: O(N), где N - число узлов дерева (поддерева).
    // Сложность пространственная: O(N), где N - число узлов дерева (поддерева).
    // Рекурсивно для каждого узла считаем размер его поддерева.
    // Для каждого узла проверяется, не является ли он четным поддеревом.
    public partial class SimpleTree<T>
    {   
        public int CountEvenTrees(SimpleTreeNode<T> root)
            => CountEvenTrees(root, out int _);

        private int CountEvenTrees(SimpleTreeNode<T> root, out int treeSize)
        {
            treeSize = 1;

            int evenCount = 0;

            if (root.Children != null)
            {
                foreach (SimpleTreeNode<T> child in root.Children)
                {
                    evenCount += CountEvenTrees(child, out int subTreeSize);

                    treeSize += subTreeSize;
                }
            }

            if (treeSize % 2 == 0)
                evenCount++;

            return evenCount;
        }
    }
}
