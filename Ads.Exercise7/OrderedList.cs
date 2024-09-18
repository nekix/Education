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

            // автоматическая вставка value 
            // в нужную позицию
        }

        public Node<T> Find(T val)
        {
            var node = head;

            while (node != null)
            {
                if (Compare(node.value, val) == 0) 
                    return node;

                node = node.next;
            }

            return null;
        }

        public void Delete(T val)
        {
            // здесь будет ваш код
        }

        public void Clear(bool asc)
        {
            _ascending = asc;
            // здесь будет ваш код
        }

        public int Count()
        {
            return 0; // здесь будет ваш код подсчёта количества элементов в списке
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
    }

}