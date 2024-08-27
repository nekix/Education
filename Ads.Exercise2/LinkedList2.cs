using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures
{

    public class Node
    {
        public int value;
        public Node next, prev;

        public Node(int _value)
        {
            value = _value;
            next = null;
            prev = null;
        }
    }

    public class LinkedList2
    {
        public Node head;
        public Node tail;

        public LinkedList2()
        {
            head = null;
            tail = null;
        }

        public void AddInTail(Node _item)
        {
            if (head == null)
            {
                head = _item;
                head.next = null;
                head.prev = null;
            }
            else
            {
                tail.next = _item;
                _item.prev = tail;
            }
            tail = _item;
        }

        public Node Find(int _value)
        {
            Node node = head;

            while(node != null)
            {
                if (node.value == _value) return node;
                node = node.next;
            }

            return null;
        }

        public List<Node> FindAll(int _value)
        {
            List<Node> nodes = new List<Node>();

            Node node = head;
            while (node != null)
            {
                if (node.value == _value) nodes.Add(node);
                node = node.next;
            }

            return nodes;
        }

        public bool Remove(int _value)
        {
            var node = Find(_value);

            if (node == null) return false;

            var prefNode = node.prev;
            var nextNode = node.next;

            if (prefNode != null) prefNode.next = nextNode;
            else head = nextNode;

            if (nextNode != null) nextNode.prev = prefNode;
            else tail = prefNode;

            return true;
        }

        public void RemoveAll(int _value)
        {
            // здесь будет ваш код удаления всех узлов по заданному значению
        }

        public void Clear()
        {
            // здесь будет ваш код очистки всего списка
        }

        public int Count()
        {
            int count = 0;

            Node node = head;
            while (node != null)
            {
                count += 1;
                node = node.next;
            }

            return count;
        }

        public void InsertAfter(Node _nodeAfter, Node _nodeToInsert)
        {
            // здесь будет ваш код вставки узла после заданного узла

            // если _nodeAfter = null
            // добавьте новый элемент первым в списке 

        }

    }
}