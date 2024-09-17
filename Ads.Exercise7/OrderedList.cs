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
            if (head == null)
            {
                head = new Node<T>(value);
                tail = head;
            }
            else
            {
                if (_ascending)
                {
                    var node = head;
                    while (node != null)
                    {
                        if (Compare(node.value, value) > 0)
                            break;

                        node = node.next;
                    }

                    if (node == null)
                    {
                        tail.next = new Node<T>(value);
                        tail.next.prev = tail;
                        tail = tail.next;
                    }
                    else
                    {
                        var next = node.next;
                        node.next = new Node<T>(value);
                        node.next.prev = node;
                        node.next.next = next;
                    }
                }
                else
                {
                    var node = head;
                    while (node != null)
                    {
                        if (Compare(node.value, value) < 0)
                            break;

                        node = node.next;
                    }

                    if (node == null)
                    {
                        tail.next = new Node<T>(value);
                        tail.next.prev = tail;
                        tail = tail.next;
                    }
                    else
                    {
                        var prev = node.prev;
                        node.prev = new Node<T>(value);
                        node.prev.next = node;
                        node.prev.prev = prev;
                    }
                }
            }

            // автоматическая вставка value 
            // в нужную позицию
        }

        public Node<T> Find(T val)
        {
            return null; // здесь будет ваш код
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