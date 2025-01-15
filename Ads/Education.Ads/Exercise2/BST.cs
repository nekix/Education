using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

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
        BSTNode<T> Root; // корень дерева, или null

        public BST(BSTNode<T> node)
        {
            Root = node;
        }

        public BSTFind<T> FindNodeByKey(int key)
        {
            var node = Root;
            var result = new BSTFind<T>();

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
            var bstFind = FindNodeByKey(key);

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
            var node = FromNode;

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
            var bstFind = FindNodeByKey(key);

            if (!bstFind.NodeHasKey)
                return false;

            if (bstFind.Node == Root)
            {
                Root = null;
                return true;
            }

            var deletedNode = bstFind.Node;

            BSTNode<T> newChild = null;

            if (deletedNode.LeftChild == null && deletedNode.RightChild != null)
            {
                newChild = deletedNode.RightChild;

                deletedNode.RightChild = null;
            }
            else if (deletedNode.LeftChild != null && deletedNode.RightChild == null)
            {
                newChild = deletedNode.LeftChild;

                deletedNode.LeftChild = null;
            }
            else if (deletedNode.LeftChild != null && deletedNode.RightChild != null)
            {
                newChild = deletedNode.RightChild;

                while (newChild.LeftChild != null)
                    newChild = newChild.LeftChild;

                if (newChild.RightChild != null)
                {
                    if (newChild.Parent.RightChild == newChild)
                    {
                        newChild.Parent.RightChild = newChild.RightChild;
                        newChild.RightChild.Parent = newChild.Parent;
                    }
                    else
                    {
                        newChild.Parent.LeftChild = newChild.RightChild;
                        newChild.RightChild.Parent = newChild.Parent;
                    }
                }
                else
                {
                    if (newChild.Parent.RightChild == newChild)
                    {
                        newChild.Parent.RightChild = null;
                    }
                    else
                    {
                        newChild.Parent.LeftChild = null;
                    }
                }

                newChild.Parent = deletedNode.Parent;

                newChild.LeftChild = deletedNode.LeftChild;
                deletedNode.LeftChild.Parent = newChild;

                newChild.RightChild = deletedNode.RightChild;
                deletedNode.RightChild.Parent = newChild;
            }

            if (deletedNode.Parent.RightChild == deletedNode)
            {
                deletedNode.Parent.RightChild = newChild;
            }
            else
            {
                deletedNode.Parent.LeftChild = newChild;
            }

            deletedNode.Parent = null;

            return true;
        }

        public bool DeleteNodeByKeyV2(int key)
        {
            var bstFind = FindNodeByKey(key);

            if (!bstFind.NodeHasKey)
                return false;

            if (bstFind.Node == Root)
            {
                Root = null;
                return true;
            }

            var deletedNode = bstFind.Node;

            BSTNode<T> newChild = null;

            // Один правый потомок
            if (deletedNode.LeftChild == null && deletedNode.RightChild != null)
            {
                // Новый
                newChild = deletedNode.RightChild;

                // Родитель удаляемого
                var parent = deletedNode.Parent;
                if (parent != null)
                {
                    if (parent.RightChild == deletedNode)
                    {
                        parent.RightChild = newChild;
                    }
                    else
                    {
                        parent.LeftChild = newChild;
                    }
                }
                newChild.Parent = parent;

                // Удялемый
                deletedNode.Parent = null;
                deletedNode.RightChild = null;
            }
            // Один левый потомок
            else if (deletedNode.LeftChild != null && deletedNode.RightChild == null)
            {
                // Новый
                newChild = deletedNode.LeftChild;

                // Родитель удаляемого
                var parent = deletedNode.Parent;
                if (parent != null)
                {
                    if (parent.RightChild == deletedNode)
                    {
                        parent.RightChild = newChild;
                    }
                    else
                    {
                        parent.LeftChild = newChild;
                    }
                }
                newChild.Parent = parent;

                // Удялемый
                deletedNode.Parent = null;
                deletedNode.LeftChild = null;
            }
            // Два потомка
            else if (deletedNode.LeftChild != null && deletedNode.RightChild != null)
            {
                newChild = deletedNode.RightChild;

                while (newChild.LeftChild != null)
                    newChild = newChild.LeftChild;

                // Есть правый узел
                if (newChild.RightChild != null)
                {
                    // Правый ребенок приемника
                    var rightNewChild = newChild.RightChild;
                    rightNewChild.Parent = newChild.Parent;

                    // Родитель узла приемника
                    var newChildParent = newChild.Parent;
                    if (newChildParent.RightChild == newChild)
                    {
                        newChildParent.RightChild = rightNewChild;
                    }
                    else
                    {
                        newChildParent.LeftChild = rightNewChild;
                    }

                    // Родитель удаляемого
                    var parent = deletedNode.Parent;
                    if (parent != null)
                    {
                        if (parent.RightChild == deletedNode)
                        {
                            parent.RightChild = newChild;
                        }
                        else
                        {
                            parent.LeftChild = newChild;
                        }
                    }
                    newChild.Parent = parent;

                    // Левый потомок удаляемого
                    newChild.LeftChild = deletedNode.LeftChild;
                    newChild.LeftChild.Parent = newChild;

                    // Правый потомок удаляемого
                    newChild.RightChild = deletedNode.RightChild;
                    newChild.RightChild.Parent = newChild;

                    // Удаляемый
                    deletedNode.Parent = null;
                    deletedNode.RightChild = null;
                    deletedNode.LeftChild = null;
                }
                // Лист
                else
                {
                    // Родитель узла приемника
                    var newChildParent = newChild.Parent;
                    if (newChildParent.RightChild == newChild)
                    {
                        newChildParent.RightChild = null;
                    }
                    else
                    {
                        newChildParent.LeftChild = null;
                    }

                    // Родитель удаляемого
                    var parent = deletedNode.Parent;
                    if (parent != null)
                    {
                        if (parent.RightChild == deletedNode)
                        {
                            parent.RightChild = newChild;
                        }
                        else
                        {
                            parent.LeftChild = newChild;
                        }
                    }
                    newChild.Parent = parent;

                    // Левый потомок удаляемого
                    newChild.LeftChild = deletedNode.LeftChild;
                    newChild.LeftChild.Parent = newChild;

                    // Правый потомок удаляемого
                    newChild.RightChild = deletedNode.RightChild;
                    newChild.RightChild.Parent = newChild;

                    // Удаляемый
                    deletedNode.Parent = null;
                    deletedNode.RightChild = null;
                    deletedNode.LeftChild = null;
                }
            }
            // Удаление листа
            else
            {
                // Родитель удаляемого
                var parent = deletedNode.Parent;
                if (parent != null)
                {
                    if (parent.RightChild == deletedNode)
                    {
                        parent.RightChild = null;
                    }
                    else
                    {
                        parent.LeftChild = null;
                    }
                }

                // Удаляемый
                deletedNode.Parent = null;
            }

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
    }
}