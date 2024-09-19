using System;
using System.Collections.Generic;

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
            if (typeof(T) == typeof(String))
            {
                // версия для лексикографического сравнения строк
            }
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

        public void Clear(bool asc)
        {
            _ascending = asc;
            // здесь будет ваш код
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

        List<Node<T>> GetAll() // выдать все элементы упорядоченного 
            // списка в виде стандартного списка
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