using System.Collections.Generic;
using System.Linq;

namespace AlgorithmsDataStructures2
{
    // ========= Урок 2 ===========

    // Задание 1.
    // Метод, проверяющий идентичность деревьев. 
    // Сложность O(N), где N-число узлов дерева.
    public partial class BST<T>
    {
        public bool CheckEqual(BST<T> tree)
        {
            if (Root == null)
                return tree.Root == null;

            Stack<BSTNode<T>> nodes = new Stack<BSTNode<T>>();

            nodes.Push(Root);
            nodes.Push(tree.Root);

            while (nodes.Count != 0)
            {
                BSTNode<T> firstNode = nodes.Pop();
                BSTNode<T> secondNode = nodes.Pop();

                if (firstNode.NodeKey != secondNode.NodeKey)
                    return false;

                if (!firstNode.NodeValue.Equals(secondNode.NodeValue))
                    return false;

                if (firstNode.LeftChild != null)
                {
                    if (secondNode.LeftChild == null)
                        return false;

                    nodes.Push(firstNode.LeftChild);
                    nodes.Push(secondNode.LeftChild);
                }
                else if (secondNode.LeftChild != null)
                    return false;

                if (firstNode.RightChild != null)
                {
                    if (secondNode.RightChild == null)
                        return false;

                    nodes.Push(firstNode.RightChild);
                    nodes.Push(secondNode.RightChild);
                }
                else if (secondNode.RightChild != null)
                    return false;
            }

            return true;
        }
    }


    // Задание 2.
    // Метод поиска путей от корня к листьям,
    // длина которых равна заданной. 
    // В качестве найденного маршрута использовал List<BSTNode<T>> и
    // соответсвнно результат работы метода список маршрутов List<List<BSTNode<T>>.
    // Сложность O(N), где N-число узлов дерева.
    public partial class BST<T>
    {
        public List<List<BSTNode<T>>> GetLeafPathsIterative(int pathLength)
        {
            List<List<BSTNode<T>>> leafPaths = new List<List<BSTNode<T>>>();

            if (Root == null)
                return leafPaths;

            if (pathLength < 1)
                return leafPaths;

            Stack<BSTNode<T>> nodes = new Stack<BSTNode<T>>();
            Stack<BSTNode<T>> currentPath = new Stack<BSTNode<T>>();

            nodes.Push(Root);

            while (nodes.Count != 0)
            {
                BSTNode<T> currentNode = nodes.Pop();

                if (currentPath.Count != 0 && currentPath.Peek() != currentNode.Parent)
                    while (currentPath.Peek() != currentNode.Parent)
                        currentPath.Pop();

                currentPath.Push(currentNode);

                if (currentPath.Count - 1 == pathLength)
                {
                    if (CheckIsLeaf(currentNode))
                        leafPaths.Add(currentPath.Reverse().ToList());
                }
                else
                {
                    if (currentNode.RightChild != null)
                        nodes.Push(currentNode.RightChild);

                    if (currentNode.LeftChild != null)
                        nodes.Push(currentNode.LeftChild);
                }
            }

            return leafPaths;
        }

        public List<List<BSTNode<T>>> GetLeafPathsRecursive(int pathLength)
        {
            if (Root == null)
                return new List<List<BSTNode<T>>>(0);

            if (pathLength < 1)
                return new List<List<BSTNode<T>>>(0);

            List<List<BSTNode<T>>> leafPaths = new List<List<BSTNode<T>>>();
            List<BSTNode<T>> currentPath = new List<BSTNode<T>>(pathLength);

            GetLeafPathsRecursive(Root, pathLength, currentPath, leafPaths);

            return leafPaths;
        }

        private void GetLeafPathsRecursive(BSTNode<T> startNode, int pathLength, List<BSTNode<T>> currentPath, List<List<BSTNode<T>>> leafPaths)
        {
            if (startNode == null)
                return;

            currentPath.Add(startNode);

            if (currentPath.Count - 1 == pathLength)
            {
                if (CheckIsLeaf(startNode))
                    leafPaths.Add(currentPath.ToList());
            }
            else
            {
                GetLeafPathsRecursive(startNode.LeftChild, pathLength, currentPath, leafPaths);
                GetLeafPathsRecursive(startNode.RightChild, pathLength, currentPath, leafPaths);
            }

            currentPath.RemoveAt(currentPath.Count - 1);
        }

        protected bool CheckIsLeaf(BSTNode<T> node)
            => node.RightChild == null && node.LeftChild == null;
    }


    // Задание 3.
    // Метод поиска путей от корня к листьям
    // с максимальной суммой значений узлов по пути. 
    // Сложность O(N), где N-число узлов дерева.
    // Для реализации сделал отдельный класс BSTInt, который
    // конкретизирует BST<int>(для того, чтобы была доступа
    // операция подсчёта суммы значений узлов).
    public partial class BSTInt : BST<int>
    {
        public BSTInt(BSTNode<int> node) : base(node)
        {

        }

        public List<List<BSTNode<int>>> GetMaxValuePathsIterative()
        {
            List<List<BSTNode<int>>> leafPaths = new List<List<BSTNode<int>>>();

            if (Root == null)
                return leafPaths;

            if (Root.LeftChild == null && Root.RightChild == null)
                return leafPaths;

            Stack<BSTNode<int>> nodes = new Stack<BSTNode<int>>();
            Stack<BSTNode<int>> currentPath = new Stack<BSTNode<int>>();

            nodes.Push(Root);

            int maxSum = Root.NodeValue;
            int currentSum = 0;

            while (nodes.Count != 0)
            {
                BSTNode<int> currentNode = nodes.Pop();

                if (currentPath.Count != 0 && currentPath.Peek() != currentNode.Parent)
                    while (currentPath.Peek() != currentNode.Parent)
                        currentSum -= currentPath.Pop().NodeValue;

                currentPath.Push(currentNode);
                currentSum += currentNode.NodeValue;

                if (CheckIsLeaf(currentNode))
                {
                    if (maxSum == currentSum)
                    {
                        leafPaths.Add(currentPath.Reverse().ToList());
                    }
                    else if (maxSum < currentSum)
                    {
                        maxSum = currentSum;
                        leafPaths.Clear();
                        leafPaths.Add(currentPath.Reverse().ToList());
                    }
                }
                else
                {
                    if (currentNode.RightChild != null)
                        nodes.Push(currentNode.RightChild);

                    if (currentNode.LeftChild != null)
                        nodes.Push(currentNode.LeftChild);
                }
            }

            return leafPaths;
        }

        public List<List<BSTNode<int>>> GetMaxValuePathsRecursive()
        {
            if (Root == null)
                return new List<List<BSTNode<int>>>(0);

            if (Root.LeftChild == null && Root.RightChild == null)
                return new List<List<BSTNode<int>>>(0);

            List<List<BSTNode<int>>> leafPaths = new List<List<BSTNode<int>>>();
            List<BSTNode<int>> currentPath = new List<BSTNode<int>>();

            GetMaxValuePathsRecursive(Root, 0, Root.NodeValue, currentPath, leafPaths);

            return leafPaths;
        }

        private int GetMaxValuePathsRecursive(BSTNode<int> startNode, int currentSum, int maxSum, List<BSTNode<int>> currentPath, List<List<BSTNode<int>>> leafPaths)
        {
            if (startNode == null)
                return maxSum;

            currentPath.Add(startNode);
            currentSum += startNode.NodeValue;

            if (CheckIsLeaf(startNode))
            {
                if (maxSum == currentSum)
                {
                    leafPaths.Add(currentPath.ToList());
                }
                else if (maxSum < currentSum)
                {
                    maxSum = currentSum;
                    leafPaths.Clear();
                    leafPaths.Add(currentPath.ToList());
                }
            }
            else
            {
                maxSum = GetMaxValuePathsRecursive(startNode.LeftChild, currentSum, maxSum, currentPath, leafPaths);
                maxSum = GetMaxValuePathsRecursive(startNode.RightChild, currentSum, maxSum, currentPath, leafPaths);
            }

            currentPath.RemoveAt(currentPath.Count - 1);
            return maxSum;
        }
    }

    // ========= Урок 3 ===========

    // Задание 3.
    // Алгоритм инвертирования дерева. Инвертирует текущее дерево:
    // слева от главного узла будут значения больше него, а справа меньше.
    // Применил обход в ширину.
    public partial class BST<T>
    {
        public void InvertTree()
        {
            if (Root == null)
                return;

            foreach (BSTNode<T> node in WideAllNodes(Root))
                SwapChildren(node);
        }

        protected void SwapChildren(BSTNode<T> currentNode)
        {
            (currentNode.LeftChild, currentNode.RightChild) = (currentNode.RightChild, currentNode.LeftChild);
        }
    }

    // Задание 4.
    // Метод, определяющий уровень с максимальной суммой значений узлов.
    public partial class BSTInt : BST<int>
    {
        public int GetLevelWithMaxValueSum()
        {
            if (Root == null)
                return -1;

            Queue<BSTNode<int>> nodesQueue = new Queue<BSTNode<int>>();

            nodesQueue.Enqueue(Root);

            int level = 0;

            int maxSum = Root.NodeValue;
            int maxLevel = 0;

            while (nodesQueue.Count != 0)
            {
                int sum = 0;

                foreach (BSTNode<int> bstNode in WideCurrentItems(nodesQueue))
                    sum += bstNode.NodeValue;

                if (maxSum < sum)
                {
                    maxSum = sum;
                    maxLevel = level;
                }

                level++;
            }

            return maxLevel;
        }
    }

    // Задание 5. Восстановление оригинального дерева из результатов префиксного и инфиксного обхода.
    // Нельзя однозначно построить дерево по инфиксному или префиксному подхоходу в отдельности,
    // т.к.каждый обход в отдельности дает неполную для построения дерева информацию.
    // Префиксный обход дает информацию о порядке узлов, начиная с корня снизу вверх,
    // т.е.часть информации о вертикальной иерархии.Но по префиксному в отдельности
    // невозможно понять, какие узлы находятся с какой стороны от рассматриваемого.
    // Инфиксный обход дает информацию о том, какие узлы находятся слева и справа от данного.
    // Но не позволяет понять, находятся они выше или ниже в иерархии.
    // Вместе эта информация дает возможность понять: где корень или текущий узел (по префиксному);
    // какие узлы находятся слева и справа от текущего, анализируемого (по инфиксному и префиксному);
    // какой следующий узел для анализа(по префиксному).
    public static class TreeExtensions
    {
        public static BSTNode RecoverTree(int[] preorder, int[] inorder)
        {
            if (preorder.Length != inorder.Length)
                return null;

            int count = preorder.Length;

            if (count == 0)
                return null;

            if (preorder[count - 1] != inorder[count - 1])
                return null;

            int preLeft = 0;
            return RecoverTree(ref preLeft, 0, inorder.Length, preorder, inorder);
        }

        private static BSTNode RecoverTree(ref int preLeft, int inLeft, int inRight, int[] preorder, int[] inorder)
        {
            if (inLeft >= inRight)
                return null;

            int root = preorder[preLeft];
            int inRootIndex = -1;

            for (int i = inLeft; i < inRight; i++)
            {
                if (inorder[i] == root)
                {
                    inRootIndex = i;
                    break;
                }
            }

            preLeft++;

            BSTNode node = new BSTNode { NodeKey = root };

            BSTNode leftNode = RecoverTree(ref preLeft, inLeft, inRootIndex, preorder, inorder);
            if (leftNode != null)
            {
                leftNode.Parent = node;
                node.LeftChild = leftNode;
            }

            BSTNode rightNode = RecoverTree(ref preLeft, inRootIndex + 1, inRight, preorder, inorder);
            if (rightNode != null)
            {
                rightNode.Parent = node;
                node.RightChild = rightNode;
            }

            return node;
        }
    }

    // ========================================================================

    // Рефлексия по уроку 1.

    // Задание 3.
    // Метод проверки симметричности дерева относительно корня. 
    // При реализации допустил ошибку, которая была пропущена тестами
    // (дублирующиеся обходы дерева, плохо учитывал случаи со средней
    // и нечётной дочерней ветвью). Были добавлены соответсвующие тест
    // кейсы, исправления. Сделал вывод, что необходимо при тестировнии
    // лучше продумывать тест кейсы и крайние случаи в особенности.
}
