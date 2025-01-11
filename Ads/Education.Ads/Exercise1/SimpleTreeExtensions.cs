using AlgorithmsDataStructures2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Education.Ads.Exercise1
{
    public static class SimpleTreeExtensions
    {
        public static SimpleTree<T> UpdateNodesLevelsRecursive<T>(this SimpleTree<T> tree)
        {
            UpdateNodesLevelsRecursive(tree.Root, 0);
            return tree;
        }

        private static void UpdateNodesLevelsRecursive<T>(SimpleTreeNode<T> node, int level)
        {
            node.Level = level;

            if (node.Children == null)
                return;

            foreach (var child in node.Children)
            {
                UpdateNodesLevelsRecursive(child, level + 1);
            }
        }

        public static SimpleTree<T> UpdateNodesLevelsIterative<T>(this SimpleTree<T> tree)
        {
            Queue<SimpleTreeNode<T>> nodes = new Queue<SimpleTreeNode<T>>();

            nodes.Enqueue(tree.Root);

            int level = 0;
            int levelNodesCount = 1;

            int queueSize = 1;

            while (queueSize != 0)
            {
                SimpleTreeNode<T> currentNode = nodes.Dequeue();

                currentNode.Level = level;

                queueSize--;
                levelNodesCount--;

                if (currentNode.Children != null)
                {
                    foreach (SimpleTreeNode<T> child in currentNode.Children)
                    {
                        nodes.Enqueue(child);
                        queueSize++;
                    }
                }

                if (levelNodesCount == 0)
                {
                    levelNodesCount = queueSize;
                    level++;
                }
            }

            return tree;
        }
    }
}
