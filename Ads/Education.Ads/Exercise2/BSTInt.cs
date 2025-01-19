using AlgorithmsDataStructures2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsDataStructures2
{
    public class BSTInt : BST<int>
    {
        public BSTInt(BSTNode<int> node) : base(node)
        {

        }

        public List<List<BSTNode<int>>> GetMaxValuePaths()
        {
            List<List<BSTNode<int>>> leafPaths = new List<List<BSTNode<int>>>();

            if (Root == null)
                return leafPaths;

            if (Root.LeftChild == null && Root.RightChild == null)
                return leafPaths;

            Stack<BSTNode<int>> nodes = new Stack<BSTNode<int>>();
            Stack<BSTNode<int>> currentPath = new Stack<BSTNode<int>>();

            nodes.Push(Root);

            int maxSum = 0;
            int currentSum = 0;

            while (nodes.Count != 0)
            {
                BSTNode<int> currentNode = nodes.Pop();

                if (currentPath.Count != 0 && currentPath.Peek() != currentNode.Parent)
                    while (currentPath.Peek() != currentNode.Parent)
                        currentSum -= currentPath.Pop().NodeValue;

                currentPath.Push(currentNode);
                currentSum += currentNode.NodeValue;

                if (currentNode.RightChild == null && currentNode.LeftChild == null)
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
    }
}
