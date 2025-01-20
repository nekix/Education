using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO.Pipes;
using System.Linq;

namespace AlgorithmsDataStructures2
{
    public class BSTNode<T>
    {
        public int NodeKey; // ключ узла
        public T NodeValue; // значение в узле
        public BSTNode<T> Parent; // родитель или null для корня
        public BSTNode<T> LeftChild; // левый потомок
        public BSTNode<T> RightChild; // правый потомок

        public BSTNode(int key, T val, BSTNode<T> parent)
        {
            NodeKey = key;
            NodeValue = val;
            Parent = parent;
            LeftChild = null;
            RightChild = null;
        }
    }

    // промежуточный результат поиска
    public class BSTFind<T>
    {
        // null если в дереве вообще нету узлов
        public BSTNode<T> Node;

        // true если узел найден
        public bool NodeHasKey;

        // true, если родительскому узлу надо добавить новый левым
        public bool ToLeft;

        public BSTFind() { Node = null; }
    }

    public class BST<T>
    {
        protected BSTNode<T> Root; // корень дерева, или null

        public BST(BSTNode<T> node)
        {
            Root = node;
        }

        public BSTFind<T> FindNodeByKey(int key)
        {
            BSTNode<T> node = Root;
            BSTFind<T> result = new BSTFind<T>();

            while (node != null)
            {
                if (node.NodeKey == key)
                {
                    result.Node = node;
                    result.NodeHasKey = true;
                    return result;
                }
                else if (node.NodeKey > key)
                {
                    if (node.LeftChild == null)
                    {
                        result.Node = node;
                        result.ToLeft = true;
                        return result;
                    }

                    node = node.LeftChild;
                }
                else
                {
                    if (node.RightChild == null)
                    {
                        result.Node = node;
                        return result;
                    }

                    node = node.RightChild;
                }
            }

            return result;
        }

        public bool AddKeyValue(int key, T val)
        {
            BSTFind<T> bstFind = FindNodeByKey(key);

            if (bstFind.Node == null)
            {
                Root = new BSTNode<T>(key, val, null);
            }
            else if (bstFind.NodeHasKey)
            {
                return false;
            }
            else if (bstFind.ToLeft)
            {
                bstFind.Node.LeftChild = new BSTNode<T>(key, val, bstFind.Node);
            }
            else
            {
                bstFind.Node.RightChild = new BSTNode<T>(key, val, bstFind.Node);
            }

            return true;
        }

        public BSTNode<T> FinMinMax(BSTNode<T> FromNode, bool FindMax)
        {
            BSTNode<T> node = FromNode;

            if (FindMax)
            {
                while (node.RightChild != null)
                    node = node.RightChild;
            }
            else
            {
                while (node.LeftChild != null)
                    node = node.LeftChild;
            }

            return node;
        }

        public bool DeleteNodeByKey(int key)
        {
            BSTFind<T> bstFind = FindNodeByKey(key);

            if (!bstFind.NodeHasKey)
                return false;

            BSTNode<T> delNode = bstFind.Node;
            BSTNode<T> delParent = delNode.Parent;

            BSTNode<T> newNode;

            if (delNode.LeftChild == null || delNode.RightChild == null)
            {
                newNode = delNode.LeftChild ?? delNode.RightChild;

                if (delParent != null)
                {
                    if (delNode == delParent.LeftChild)
                        delParent.LeftChild = newNode;
                    else
                        delParent.RightChild = newNode;

                    if (newNode != null)
                        newNode.Parent = delParent;
                }
                else
                {
                    Root = newNode;
                }
            }
            else
            {
                newNode = delNode.RightChild;

                while (newNode.LeftChild != null)
                    newNode = newNode.LeftChild;

                BSTNode<T> newNodeParent = newNode.Parent;

                // Фиксация оставшегося родительского узла
                // ноды-приемника
                newNodeParent.LeftChild = newNode.RightChild;
                if (newNodeParent.LeftChild != null)
                    newNodeParent.LeftChild.Parent = newNodeParent;

                // Фиксация с левым узлом удаляемой узла
                newNode.LeftChild = delNode.LeftChild;
                newNode.LeftChild.Parent = newNode;

                // Фиксация с правым узлом удялемого узла
                newNode.RightChild = delNode.RightChild;
                newNode.RightChild.Parent = newNode;

                // Фиксация с родителем удаляемого узла
                newNode.Parent = delParent;
                if (delParent != null)
                {
                    if (delParent.LeftChild == delNode)
                        delParent.LeftChild = newNode;
                    else
                        delParent.RightChild = newNode;
                }
                else
                {
                    Root = newNode;
                }
            }

            // Очистка удаляемого узла
            delNode.RightChild = null;
            delNode.LeftChild = null;
            delNode.Parent = null;

            return true;
        }

        public int Count()
        {
            if (Root == null)
                return 0;

            Stack<BSTNode<T>> nodes = new Stack<BSTNode<T>>();

            nodes.Push(Root);
            int nodesCount = 1;

            while (nodes.Count != 0)
            {
                BSTNode<T> currentNode = nodes.Pop();

                if (currentNode.LeftChild != null)
                {
                    nodes.Push(currentNode.LeftChild);
                    nodesCount++;
                }
                
                if (currentNode.RightChild != null)
                {
                    nodes.Push(currentNode.RightChild);
                    nodesCount++;
                }
            }

            return nodesCount;
        }

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

        public List<List<BSTNode<T>>> GetLeafPaths(int pathLength)
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
                    if (currentNode.RightChild == null && currentNode.LeftChild == null)
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

        public List<BSTNode<T>> WideAllNodes()
        {
            List<BSTNode<T>> nodesResult = new List<BSTNode<T>>();

            if (Root == null)
                return nodesResult;

            Queue<BSTNode<T>> nodesQueue = new Queue<BSTNode<T>>();

            nodesQueue.Enqueue(Root);

            while (nodesQueue.Count != 0)
            {
                BSTNode<T> currentNode = nodesQueue.Dequeue();
                nodesResult.Add(currentNode);

                if (currentNode.LeftChild != null)
                    nodesQueue.Enqueue(currentNode.LeftChild);

                if (currentNode.RightChild != null)
                    nodesQueue.Enqueue(currentNode.RightChild);
            }

            return nodesResult;
        }

        public List<BSTNode<T>> DeepAllNodes(int order)
        {
            switch (order)
            {
                case 0:
                    return DeepAllNodesInOrder();
                case 1:
                    return DeepAllNodesPostOrder();
                case 2:
                    return DeepAllNodesPreOrder();
            }

            return new List<BSTNode<T>>(0);
        }

        private List<BSTNode<T>> DeepAllNodesInOrder()
        {
            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            if (Root == null)
                return nodes;

            Stack<BSTNode<T>> stackNodes = new Stack<BSTNode<T>>();

            BSTNode<T> node = Root;

            do
            {
                while (node != null)
                {
                    stackNodes.Push(node);
                    node = node.LeftChild;
                }

                node = stackNodes.Pop();
                nodes.Add(node);

                node = node.RightChild;
            } 
            while (node != null || stackNodes.Count != 0);

            return nodes;
        }

        private List<BSTNode<T>> DeepAllNodesPostOrder()
        {
            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            if (Root == null)
                return nodes;
            
            Stack<BSTNode<T>> nodesStack = new Stack<BSTNode<T>>();
            
            nodesStack.Push(Root);
            BSTNode<T> parentNode = null;

            while (nodesStack.Count != 0)
            {
                BSTNode<T> currentNode = nodesStack.Peek();

                if (parentNode == null || parentNode.LeftChild == currentNode || parentNode.RightChild == currentNode)
                {
                    if (currentNode.LeftChild != null)
                        nodesStack.Push(currentNode.LeftChild);
                    else if (currentNode.RightChild != null)
                        nodesStack.Push(currentNode.RightChild);
                    else
                    {
                        nodesStack.Pop();
                        nodes.Add(currentNode);
                    }
                }
                else if (currentNode.LeftChild == parentNode)
                {
                    if (currentNode.RightChild != null)
                        nodesStack.Push(currentNode.RightChild);
                    else
                    {
                        nodesStack.Pop();
                        nodes.Add(currentNode);
                    }
                }
                else if (currentNode.RightChild == parentNode)
                {
                    nodesStack.Pop();
                    nodes.Add(currentNode);
                }
               
                parentNode = currentNode;
            }

            return nodes;
        }

        private List<BSTNode<T>> DeepAllNodesPreOrder()
        {
            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            if (Root == null)
                return nodes;

            Stack<BSTNode<T>> stackNodes = new Stack<BSTNode<T>>();

            stackNodes.Push(Root);

            while (stackNodes.Count != 0)
            {
                BSTNode<T> currentNode = stackNodes.Pop();
                nodes.Add(currentNode);

                if (currentNode.RightChild != null)
                    stackNodes.Push(currentNode.RightChild);

                if (currentNode.LeftChild != null)
                    stackNodes.Push(currentNode.LeftChild);
            }

            return nodes;
        }
    }
}