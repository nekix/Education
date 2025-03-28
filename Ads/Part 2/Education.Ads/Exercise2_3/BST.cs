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

    public partial class BST<T>
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

        public List<BSTNode> WideAllNodes()
        {
            List<BSTNode<T>> nodesResult = new List<BSTNode<T>>();

            if (Root == null)
                return new List<BSTNode>();

            foreach (BSTNode<T> node in WideAllNodes(Root))
                nodesResult.Add(node);

            return ConvertToBstNode(nodesResult);
        }

        public List<BSTNode> DeepAllNodesIterative(int order)
        {
            switch (order)
            {
                case 0:
                    return DeepAllNodesInOrderIterative();
                case 1:
                    return DeepAllNodesPostOrderIterative();
                case 2:
                    return DeepAllNodesPreOrderIterative();
            }

            return new List<BSTNode>(0);
        }

        public List<BSTNode> DeepAllNodesRecursive(int order)
        {
            if (Root == null)
                return new List<BSTNode>(0);

            if (order < 0 || order > 2)
                return new List<BSTNode>(0);

            List<BSTNode<T>> nodes = new List<BSTNode<T>>();

            DeepAllNodesRecursive(order, Root, nodes);

            return ConvertToBstNode(nodes);
        }

        protected IEnumerable<BSTNode<T>> WideAllNodes(BSTNode<T> startNode)
        {
            Queue<BSTNode<T>> nodesQueue = new Queue<BSTNode<T>>();

            nodesQueue.Enqueue(Root);

            while (nodesQueue.Count != 0)
                foreach (BSTNode<T> node in WideCurrentItems(nodesQueue))
                    yield return node;
        }

        /// <summary>
        /// Traverse through the width of only those elements that are in the queue at the time the method is called.
        /// </summary>
        /// <param name="nodesQueue"></param>
        /// <returns></returns>
        protected IEnumerable<BSTNode<T>> WideCurrentItems(Queue<BSTNode<T>> nodesQueue)
        {
            for (int i = nodesQueue.Count; i > 0; i--)
                yield return WideNextNode(nodesQueue);
        }

        protected static BSTNode<T> WideNextNode(Queue<BSTNode<T>> nodesQueue)
        {
            BSTNode<T> currentNode = nodesQueue.Dequeue();

            if (currentNode.LeftChild != null)
                nodesQueue.Enqueue(currentNode.LeftChild);

            if (currentNode.RightChild != null)
                nodesQueue.Enqueue(currentNode.RightChild);

            return currentNode;
        }

        private List<BSTNode> DeepAllNodesInOrderIterative()
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

        private List<BSTNode> DeepAllNodesPostOrderIterative()
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

        private List<BSTNode> DeepAllNodesPreOrderIterative()
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

        private void DeepAllNodesRecursive(int order, BSTNode<T> node, List<BSTNode<T>> nodes)
        {
            if (node == null)
                return;

            if (order == 2)
                nodes.Add(node);

            DeepAllNodesRecursive(order, node.LeftChild, nodes);

            if (order == 0)
                nodes.Add(node);

            DeepAllNodesRecursive(order, node.RightChild, nodes);

            if (order == 1)
                nodes.Add(node);
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