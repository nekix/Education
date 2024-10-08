﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.AccessControl;
using System.Threading;
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

        public bool IsHacLoop()
        {
            if(head == null)
                return false;

            // From head to tail
            Node current, next;
            for(current = head.next, next = head.next.next;
                current != null && next?.next != null;
                current = current.next, next = next.next.next)
            {
                if(current == next)
                    return true;
            }

            // From tail to head
            for (current = tail.prev, next = tail.prev.prev;
                current != null && next?.prev != null;
                current = current.prev, next = next.prev.prev)
            {
                if (current == next)
                    return true;
            }

            return false;
        }

        public void Sort()
        {
            if (head == null)
                return;

            var newHeadNode = head;
            var newTailNode = head;

            Remove(head);
            newHeadNode.next = null;

            for (var currentNode = head; currentNode != null; currentNode = head)
            {
                if(currentNode.value <= newHeadNode.value)
                {
                    Remove(currentNode);

                    newHeadNode.prev = currentNode;
                    currentNode.next = newHeadNode;
                    currentNode.prev = null;
                    newHeadNode = currentNode;
                }
                else
                {
                    var currentNewNode = newHeadNode;
                    while (currentNewNode.next != null && currentNewNode.next.value < currentNode.value)
                        currentNewNode = currentNewNode.next;

                    Remove(currentNode);

                    currentNode.prev = currentNewNode;
                    currentNode.next = currentNewNode.next;

                    if (currentNewNode.next != null)
                        currentNewNode.next.prev = currentNode;
                    else
                        newTailNode = currentNode;

                    currentNewNode.next = currentNode;
                }
            }

            head = newHeadNode;
            tail = newTailNode;
        }

        public LinkedList2 SortMerge(LinkedList2 secondList)
        {
            Sort();
            secondList.Sort();

            var list = new LinkedList2();

            if (head == null && secondList.head == null)
                return list;

            var firstNode =  head;

            var secondNode = secondList.head;

            while (firstNode != null || secondNode != null)
            {
                if (firstNode != null && (secondNode == null || (firstNode.value <= secondNode.value)))
                {
                    list.AddInTail(new Node (firstNode.value));
                    firstNode = firstNode.next;
                }
                else
                {
                    list.AddInTail(new Node(secondNode.value));
                    secondNode = secondNode.next;
                }
            }

            return list;
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