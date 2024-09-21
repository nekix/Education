using System;
using System.Collections.Generic;
using System.Xml.Linq;

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

            Remove(node);

            return true;
        }

        public void RemoveAll(int _value)
        {
            Node node = head;

            while (node != null)
            {
                if (node.value == _value) Remove(node);
                node = node.next;
            }
        }

        public void Clear()
        {
            head = null;
            tail = null;
        }

        public int Count()
        {
            int count = 0;

            Node node = head;
            while (node != null)
            {
                count++;
                node = node.next;
            }

            return count;
        }

        public void InsertAfter(Node _nodeAfter, Node _nodeToInsert)
        {
            if (_nodeAfter == null)
            {
                _nodeToInsert.next = head;

                if (head == null)
                {
                    tail = _nodeToInsert;
                }
                else
                {
                    _nodeToInsert.prev = null;
                    head.prev = _nodeToInsert;
                }

                head = _nodeToInsert;
            }
            else
            {
                var nextNode = _nodeAfter.next;

                _nodeAfter.next = _nodeToInsert;
                _nodeToInsert.prev = _nodeAfter;

                _nodeToInsert.next = nextNode;

                if (nextNode == null) tail = _nodeToInsert;
                else nextNode.prev = _nodeToInsert;
            }
        }

        public void Reverse()
        {
            for (var node = head; node != null; node = node.prev)
                (node.prev, node.next) = (node.next, node.prev);

            (head, tail) = (tail, head);
        }

        private void Remove(Node node)
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