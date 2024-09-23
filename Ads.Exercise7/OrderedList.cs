using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AlgorithmsDataStructures
{
    public class Node<T>
    {
        public T value;
        public Node<T> next, prev;

        public Node(T _value)
        {
            value = _value;
            next = null;
            prev = null;
        }
    }

    public class OrderedList<T>
    {
        public Node<T> head, tail;
        private bool _ascending;
        private int _count;

        public OrderedList(bool asc)
        {
            head = null;
            tail = null;
            _ascending = asc;
            _count = 0;
        }

        public int Compare(T v1, T v2)
        {
            int result = 0;

            if (v1 is string strV1 && v2 is string strV2)
                result = string.Compare(strV1, strV2);
            else if (v1 is IComparable<T> compV1)
                result = compV1.CompareTo(v2);

            return result;
        }

        public void Add(T value)
        {
            var newNode = new Node<T>(value);

            if (head == null)
            {
                head = newNode;
                tail = head;
            }
            else
            {
                var multiplier = _ascending ? 1 : -1;

                var node = head;
                while (node != null)
                {
                    if (Compare(node.value, value) * multiplier > 0)
                        break;

                    node = node.next;
                }

                if (node == head)
                {
                    head.prev = newNode;
                    newNode.next = head;
                    head = newNode;
                }
                else if (node == null)
                {
                    tail.next = newNode;
                    newNode.prev = tail;
                    tail = newNode;
                }
                else
                {
                    node.prev.next = newNode;
                    newNode.prev = node.prev;
                    newNode.next = node;
                    node.prev = newNode;
                }
            }

            _count++;
        }

        public Node<T> Find(T val)
        {
            var multiplier = _ascending ? 1 : -1;

            if (head != null && Compare(head.value, val) * multiplier > 0)
                return null;

            if (tail != null && Compare(tail.value, val) * multiplier < 0)
                return null;

            var node = head;

            while (node != null)
            {
                var comparation = Compare(node.value, val);

                if (comparation == 0)
                    return node;

                if (comparation * multiplier > 0)
                    return null;

                node = node.next;
            }

            return null;
        }

        public int FindIndex(Node<T> node)
        {
            if(_count == 0) return -1;

            var multiplier = _ascending ? 1 : -1;

            // Check head
            var extremeCompareResult = Compare(head.value, node.value);
            if (extremeCompareResult == 0 && head == node)
                return 0;
            else if (extremeCompareResult * multiplier > 0)
                return -1;

            // Check tail
            extremeCompareResult = Compare(tail.value, node.value);
            if (extremeCompareResult == 0 && tail == node)
                return _count - 1;
            else if (extremeCompareResult * multiplier < 0)
                return -1;

            int leftIndex = 1;
            int rightIndex = _count - 2;

            int currentIndex = 0;
            var currentNode = head;

            while (leftIndex <= rightIndex)
            {
                var middle = leftIndex + (rightIndex - leftIndex >> 1);

                // Get currentNode by index faster
                // than comparing elements
                // so the O(1)
                var offset = middle - currentIndex;
                for (int i = 0; i < Math.Abs(offset); i++)
                    currentNode = offset > 0 
                        ? currentNode.next 
                        : currentNode.prev;

                currentIndex = middle;

                var compareResult = Compare(currentNode.value, node.value);

                if (compareResult == 0)
                {
                    if (currentNode == node)
                        return middle;

                    // Check nodes up to current
                    currentIndex--;
                    for (var finalNode = currentNode.prev;
                         Compare(finalNode.value, node.value) == 0;
                         finalNode = finalNode.prev, currentIndex--)
                    {
                        if(finalNode == node)
                            return currentIndex;
                    }

                    // Check nodes after current
                    currentIndex = middle + 1;
                    for (var finalNode = currentNode.next;
                         Compare(finalNode.value, node.value) == 0;
                         finalNode = finalNode.next, currentIndex++)
                    {
                        if (finalNode == node)
                            return currentIndex;
                    }

                    return -1;
                }
                if (compareResult * multiplier < 0)
                    leftIndex = middle + 1;
                else
                    rightIndex = middle - 1;
            }

            return -1;
        }

        public void Delete(T val)
        {
            var node = Find(val);

            if (node == null) return;

            Delete(node);
        }

        public void RemoveDuplicates()
        {
            if (head == null)
                return;

            var node = head;
            Node<T> firstEqualNode = null;

            while (node != null)
            {
                if (firstEqualNode == null)
                {
                    if (node.next != null && Compare(node.value, node.next.value) == 0)
                        firstEqualNode = node;
                }
                else if (node.next == null || Compare(firstEqualNode.value, node.next.value) != 0)
                {
                    Delete(firstEqualNode, node);

                    firstEqualNode = null;
                }

                node = node.next;
            }
        }

        public void Clear(bool asc)
        {
            _ascending = asc;
            head = null;
            tail = null;
        }

        public int Count()
            => _count;

        List<Node<T>> GetAll()
        {
            List<Node<T>> r = new List<Node<T>>();
            Node<T> node = head;
            while (node != null)
            {
                r.Add(node);
                node = node.next;
            }
            return r;
        }

        private void Delete(Node<T> firstNode, Node<T> lastNode)
        {
            for(var currentNode = firstNode;
                currentNode != lastNode;
                currentNode = currentNode.next)
                Delete(currentNode);
        }

        private void Delete(Node<T> node)
        {
            var prefNode = node.prev;
            var nextNode = node.next;

            if (prefNode != null) prefNode.next = nextNode;
            else head = nextNode;

            if (nextNode != null) nextNode.prev = prefNode;
            else tail = prefNode;

            _count--;
        }

        private int ReCountAll()
        {
            int count = 0;

            var node = head;
            while (node != null)
            {
                count++;
                node = node.next;
            }

            return _count = count;
        }
    }

}