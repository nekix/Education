using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmsDataStructures2
{
    public class BSTNode
    {
        public int NodeKey;
        public BSTNode Parent;
        public BSTNode LeftChild;
        public BSTNode RightChild;
    }

    public class BSTNode<T>
    {
        public int NodeKey;
        public T NodeValue;
        public BSTNode<T> Parent;
        public BSTNode<T> LeftChild;
        public BSTNode<T> RightChild;

        public BSTNode(int key, T val, BSTNode<T> parent)
        {
            NodeKey = key;
            NodeValue = val;
            Parent = parent;
            LeftChild = null;
            RightChild = null;
        }
    }

    public class BSTFind<T>
    {
        public BSTNode<T> Node;

        public bool NodeHasKey;

        public bool ToLeft;

        public BSTFind() { Node = null; }
    }

    public class BST<T>
    {
        protected BSTNode<T> Root;

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

                newNodeParent.LeftChild = newNode.RightChild;
                if (newNodeParent.LeftChild != null)
                    newNodeParent.LeftChild.Parent = newNodeParent;

                newNode.LeftChild = delNode.LeftChild;
                newNode.LeftChild.Parent = newNode;

                newNode.RightChild = delNode.RightChild;
                newNode.RightChild.Parent = newNode;

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

        public List<BSTNode> WideAllNodes()
        {
            List<BSTNode<T>> nodesResult = new List<BSTNode<T>>();

            if (Root == null)
                return new List<BSTNode>();

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

            return ConvertToBstNode(nodesResult);
        }

        public List<BSTNode> DeepAllNodes(int order)
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

            return new List<BSTNode>(0);
        }

        public void InvertTree()
        {
            if (Root == null)
                return;

            Queue<BSTNode<T>> nodesQueue = new Queue<BSTNode<T>>();

            nodesQueue.Enqueue(Root);

            while (nodesQueue.Count != 0)
            {
                BSTNode<T> currentNode = nodesQueue.Dequeue();

                (currentNode.LeftChild, currentNode.RightChild) = (currentNode.RightChild, currentNode.LeftChild);

                if (currentNode.LeftChild != null)
                    nodesQueue.Enqueue(currentNode.LeftChild);

                if (currentNode.RightChild != null)
                    nodesQueue.Enqueue(currentNode.RightChild);
            }
        }

        protected bool CheckIsLeaf(BSTNode<T> node)
            => node.RightChild == null && node.LeftChild == null;

        private List<BSTNode> DeepAllNodesInOrder()
        {
            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            if (Root == null)
                return new List<BSTNode>(0);

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

            return ConvertToBstNode(nodes);
        }

        private List<BSTNode> DeepAllNodesPostOrder()
        {
            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            if (Root == null)
                return new List<BSTNode>(0);
            
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

            return ConvertToBstNode(nodes);
        }

        private List<BSTNode> DeepAllNodesPreOrder()
        {
            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            if (Root == null)
                return new List<BSTNode>(0);

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

            return ConvertToBstNode(nodes);
        }

        private List<BSTNode> ConvertToBstNode(List<BSTNode<T>> nodes)
        {
            List<BSTNode> newNodes = nodes
                .Select(x => new BSTNode { NodeKey = x.NodeKey })
                .ToList();

            for (int i = 0; i < newNodes.Count; i++)
            {
                BSTNode newNode = newNodes[i];
                BSTNode<T> oldNode = nodes[i];

                if (oldNode.Parent != null)
                {
                    newNode.Parent = newNodes
                        .First(n => n.NodeKey == oldNode.Parent.NodeKey);
                }

                if (oldNode.LeftChild != null)
                {
                    newNode.LeftChild = newNodes
                        .First(n => n.NodeKey == oldNode.LeftChild.NodeKey);
                }

                if (oldNode.RightChild != null)
                {
                    newNode.RightChild = newNodes
                        .First(n => n.NodeKey == oldNode.RightChild.NodeKey);
                }
            }

            return newNodes;
        }
    }
}