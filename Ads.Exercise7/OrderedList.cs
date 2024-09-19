using System;
using System.Collections.Generic;
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

        public OrderedList(bool asc)
        {
            head = null;
            tail = null;
            _ascending = asc;
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
                    var prefNode = firstEqualNode.prev;
                    var nextNode = node.next;

                    if (prefNode != null) prefNode.next = nextNode;
                    else head = nextNode;

                    if (nextNode != null) nextNode.prev = prefNode;
                    else tail = prefNode;

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
        {
            int count = 0;

            var node = head;
            while (node != null)
            {
                count++;
                node = node.next;
            }

            return count;
        }

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

        private void Delete(Node<T> node)
        {
            var prefNode = node.prev;
            var nextNode = node.next;

            if (prefNode != null) prefNode.next = nextNode;
            else head = nextNode;

            if (nextNode != null) nextNode.prev = prefNode;
            else tail = prefNode;
        }
    }

}