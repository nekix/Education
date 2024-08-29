using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Ads.Exercise2
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

    public class DummyLinkedList2
    {
        public Node head;
        public Node tail;

        public DummyLinkedList2()
        {
            head = new DummyNode();
            tail = new DummyNode();

            head.next = tail;
            tail.prev = head;
        }

        public Node Find(int _value)
        {
            for(var node = head.next; !node.GetType().Equals(typeof(DummyNode)); node = node.next)
            {
                if (node.value == _value) return node;
            }

            return null;
        }

        public List<Node> FindAll(int _value)
        {
            List<Node> nodes = new List<Node>();

            for (var node = head.next; !node.GetType().Equals(typeof(DummyNode)); node = node.next)
            {
                if (node.value == _value) nodes.Add(node);
            }

            return nodes;
        }

        public bool Remove(int _value)
        {
            var node = Find(_value);

            if (node == null || node.GetType().Equals(typeof(DummyNode)))
                return false;

            Remove(node);

            return true;
        }

        public void RemoveAll(int _value)
        {
            for (var node = head.next; !node.GetType().Equals(typeof(DummyNode)); node = node.next)
            {
                if (node.value == _value) Remove(node);
            }
        }

        public void Clear()
        {
            head.next = tail;
            tail.prev = head;
        }

        public int Count()
        {
            int count = 0;

            for (var node = head.next; !node.GetType().Equals(typeof(DummyNode)); node = node.next)
            {
                count++;
            }

            return count;
        }

        public void InsertAfter(Node _nodeAfter, Node _nodeToInsert)
        {
            if (_nodeAfter == null)
            {
                _nodeAfter = head;
            }

            if (_nodeAfter.next == null) return;

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