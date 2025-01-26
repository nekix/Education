using System.Collections.Generic;
using System.Linq;

namespace AlgorithmsDataStructures2
{
    public class BSTInt : BST<int>
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

        public int GetLevelWithMaxValueSum()
        {
            if (Root == null)
                return -1;

            Queue<BSTNode<int>> nodesQueue = new Queue<BSTNode<int>>();

            nodesQueue.Enqueue(Root);

            int sum = 0;
            int level = 0;

            int maxSum = Root.NodeValue;
            int maxLevel = 0;

            int nodesLevelCount = 1;

            while (nodesQueue.Count != 0)
            {
                BSTNode<int> currentNode = nodesQueue.Dequeue();

                nodesLevelCount--;
                sum += currentNode.NodeValue;

                if (currentNode.LeftChild != null)
                    nodesQueue.Enqueue(currentNode.LeftChild);

                if (currentNode.RightChild != null)
                    nodesQueue.Enqueue(currentNode.RightChild);

                if (nodesLevelCount == 0)
                {
                    if (maxSum < sum)
                    {
                        maxSum = sum;
                        maxLevel = level;
                    }

                    sum = 0;
                    level++;
                    nodesLevelCount = nodesQueue.Count;
                }
            }

            return maxLevel;
        }
    }
}
