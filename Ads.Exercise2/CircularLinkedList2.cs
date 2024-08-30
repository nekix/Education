using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Ads.Exercise2.CircleDummyLinkedLists
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

    public class DummyNode : Node
    {
        public DummyNode() : base(0)
        {
        }
    }

    public class CircularLinkedList2
    {
        public Node head;

        public CircularLinkedList2()
        {
            head = new DummyNode();

            head.next = head;
            head.prev = head;
        }

        public Node Find(int _value)
        {
            for (var node = head.next; !typeof(DummyNode).IsInstanceOfType(node); node = node.next)
            {
                if (node.value == _value) return node;
            }

            return null;
        }

        public List<Node> FindAll(int _value)
        {
            List<Node> nodes = new List<Node>();

            for (var node = head.next; !typeof(DummyNode).IsInstanceOfType(node); node = node.next)
            {
                if (node.value == _value) nodes.Add(node);
            }

            return nodes;
        }

        public bool Remove(int _value)
        {
            var node = Find(_value);

            if (node == null || typeof(DummyNode).IsInstanceOfType(node))
                return false;

            Remove(node);

            return true;
        }

        public void RemoveAll(int _value)
        {
            for (var node = head.next; !typeof(DummyNode).IsInstanceOfType(node); node = node.next)
            {
                if (node.value == _value) Remove(node);
            }
        }

        public void Clear()
        {
            head.next = head;
            head.prev = head;
        }

        public int Count()
        {
            int count = 0;

            for (var node = head.next; !typeof(DummyNode).IsInstanceOfType(node); node = node.next)
            {
                count++;
            }

            return count;
        }

        public void InsertAfter(Node _nodeAfter, Node _nodeToInsert)
        {
            _nodeAfter = _nodeAfter ?? head;

            _nodeToInsert.next = _nodeAfter.next;
            _nodeToInsert.prev = _nodeAfter;

            _nodeAfter.next = _nodeToInsert;
            _nodeToInsert.next.prev = _nodeToInsert;
        }

        private void Remove(Node node)
        {
            node.prev.next = node.next;
            node.next.prev = node.prev;
        }
    }
}