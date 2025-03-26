using System.Collections.Generic;
using System;
using Ads.Exercise6;

namespace AlgorithmsDataStructures2
{
    // ========= Урок 1 ===========

    // Задание 1.
    // Метод, который перебирает дерево и прописывает узлам их уровни.
    // Реализовал как рекурсивную, так итеративную версию:
    // Сложность временная составляет O(N), где N - число узлов дерева.
    public partial class SimpleTree<T>
    {
        public SimpleTree<T> UpdateNodesLevelsRecursive()
        {
            if (Root != null)
                UpdateNodesLevelsRecursive(Root, 0);

            return this;
        }

        private void UpdateNodesLevelsRecursive(SimpleTreeNode<T> node, int nodeLevel)
        {
            node.Level = nodeLevel;

            if (node.Children == null)
                return;

            foreach (SimpleTreeNode<T> child in node.Children)
                UpdateNodesLevelsRecursive(child, nodeLevel + 1);
        }

        public SimpleTree<T> UpdateNodesLevelsIterative()
        {
            Queue<SimpleTreeNode<T>> nodesQueue = new Queue<SimpleTreeNode<T>>();

            nodesQueue.Enqueue(Root);

            int level = 0;
            int nodesLevelCount = 1;

            while (nodesQueue.Count != 0)
            {
                SimpleTreeNode<T> currentNode = nodesQueue.Dequeue();

                currentNode.Level = level;

                nodesLevelCount--;

                if (currentNode.Children != null)
                {
                    foreach (SimpleTreeNode<T> child in currentNode.Children)
                    {
                        nodesQueue.Enqueue(child);
                    }
                }

                if (nodesLevelCount == 0)
                {
                    level++;
                    nodesLevelCount = nodesQueue.Count;
                }
            }

            return this;
        }
    }


    // Задание 2.
    // Придумать способ поддержки уровня узлов без анализа всего дерева.
    // Можно организовать присвоение уровней узлам при операциях, которые могут
    // привести к изменению уровня узлов в дереве.В нашем случае это методы AddChild,
    // MoveNode, а также сам конструктор дерева.В методах AddChild и MoveNode можно
    // вызывать метод из задания 1, в качестве параметра передавать добавляемый узел
    // и перемещаемый узел соответственно (т.к.могут добавить/переместить узел, которые
    // уже может содержать дочерние узлы). В конструкторе также вызывается метод
    // задания 1, которому передается root node(на случай, если у root node есть дети).
    // В таком случае происходит перебор в лучшем случае одного узла.
    // Реализовано в файле Exercise1/SimpleTree.cs (чтобы проходили тесты на сервере).

    // Сложность при операции добавления узла без потомков или создании дерева с Root узлом без потомков: O(1).
    // При добавлении узла с потомками или переносе узла с потомками в новое место: O(N),
    // где N - число дочерних узлов добавляемого/переносимого узла (т.к. нужно обновить
    // уровень узла всем потомкам узла). 


    // Задание 3.
    // Метод проверки симметричности дерева относительно корня. Реализовал как рекурсивную,
    // так итеративную версию. В итеративной версии использовал деку из первого курса АСД.
    // Сложность: O(N), где N - число узлов дерева.
    public partial class SimpleTree<T>
    {
        public bool IsSymmetricallyIterative()
        {
            if (Root == null)
                return true;

            // Т.к. по умолчанию в C# нет реализации деки, я использовал свою из первой части курса АСД
            ImprovedDeque<SimpleTreeNode<T>> nodesDeque = new ImprovedDeque<SimpleTreeNode<T>>();
            nodesDeque.AddFront(Root);

            while (nodesDeque.Size() > 0)
            {
                SimpleTreeNode<T> leftNode = nodesDeque.RemoveFront();

                if (nodesDeque.Size() != 0)
                {
                    SimpleTreeNode<T> rightNode = nodesDeque.RemoveTail();

                    if (!leftNode.NodeValue.Equals(rightNode.NodeValue))
                        return false;

                    if (leftNode.Children == null && leftNode.Children == null)
                        continue;

                    if (leftNode.Children.Count != rightNode.Children.Count)
                        return false;

                    for (int i = rightNode.Children.Count - 1; i >= 0; i--)
                        nodesDeque.AddTail(rightNode.Children[i]);
                }

                if (leftNode.Children != null)
                    foreach (SimpleTreeNode<T> child in leftNode.Children)
                        nodesDeque.AddFront(child);
            }

            return true;
        }

        public bool IsSymmetricallyRecursive()
        {
            if (Root == null)
                return true;

            return IsSymmetricallyRecursive(Root.Children);
        }

        private bool IsSymmetricallyRecursive(List<SimpleTreeNode<T>> children)
        {
            if (children == null || children.Count == 0)
                return true;

            for (int i = 0, j = children.Count - 1; i < j; i++, j--)
            {
                if (!children[i].NodeValue.Equals(children[j].NodeValue))
                    return false;

                // Ветви зеркально расходятся
                if (!IsSymmetricallyRecursive(children[i].Children, children[j].Children))
                    return false;
            }

            // Средний ветвь (если есть)
            if (children.Count % 2 != 0)
                return IsSymmetricallyRecursive(children[children.Count / 2].Children);

            return true;
        }

        private bool IsSymmetricallyRecursive(List<SimpleTreeNode<T>> leftChildren, List<SimpleTreeNode<T>> rightChildren)
        {
            if (leftChildren == null && rightChildren == null)
                return true;

            if (leftChildren == null || rightChildren == null)
                return false;

            if (leftChildren.Count != rightChildren.Count)
                return false;

            for (int i = 0, j = leftChildren.Count - 1; i < leftChildren.Count; i++, j--)
            {
                if (!leftChildren[i].NodeValue.Equals(rightChildren[j].NodeValue))
                    return false;

                if (!IsSymmetricallyRecursive(leftChildren[i].Children, rightChildren[j].Children))
                    return false;
            }

            return true;
        }
    }


    // ========= Урок 9 ===========

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

    // ========================================================================

    // Рефлексия по уроку 7.

    // Задание 5.
    // Можно было бы ускорить использованием хеширования на базе массива
    // или хеш таблицы.

    // Задание 6.
    // В изначальной реализации мой способ разрушал первоначальные кучи.
    // Правильным способом будет создание копии первоначальой кучи
    // (это и будет доп. метод)
    // (а в функциональном программировании вообще все структуры
    // иммутабельны и передаются только по значению, своим копированием,
    // и таких проблем не возникает в принципе)..
}
