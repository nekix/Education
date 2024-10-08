﻿using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace AlgorithmsDataStructures
{
    public class Node
    {
        public int value;
        public Node next;
        public Node(int _value) { value = _value; }
    }

    public class LinkedList
    {
        public Node head;
        public Node tail;

        public LinkedList()
        {
            head = null;
            tail = null;
        }

        public void AddInTail(Node _item)
        {
            if (head == null) head = _item;
            else tail.next = _item;
            tail = _item;
        }

        public Node Find(int _value)
        {
            Node node = head;
            while (node != null)
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
            Node node = head;
            Node previousNode = null;

            while (node != null)
            {
                if (node.value == _value)
                {
                    if (previousNode == null)
                    {
                        head = head.next;

                        if (head == null) tail = null;
                    }
                    else
                    {
                        previousNode.next = node.next;

                        if (previousNode.next == null)
                        {
                            tail = previousNode;
                        }
                    }

                    return true;
                }

                previousNode = node;
                node = node.next;
            }

            return false;
        }

        public void RemoveAll(int _value)
        {
            Node node = head;
            Node previousNode = null;

            while (node != null)
            {
                if (node.value == _value)
                {
                    if (previousNode == null)
                    {
                        head = node.next;

                        if (head == null) tail = null;
                    }
                    else
                    {
                        previousNode.next = node.next;

                        if (previousNode.next == null)
                        {
                            tail = previousNode;
                        }
                    }

                }
                else
                {
                    previousNode = node;
                }

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
                count += 1;
                node = node.next;
            }

            return count;
        }

        public void InsertAfter(Node _nodeAfter, Node _nodeToInsert)
        {
            if(_nodeAfter == null)
            {
                if(head == null)
                {
                    tail = _nodeToInsert;
                }
                else
                {
                    _nodeToInsert.next = head;
                }

                head = _nodeToInsert;
            }
            else
            {
                var nextNode = _nodeAfter.next;

                _nodeAfter.next = _nodeToInsert;

                if (nextNode == null)
                {
                    tail = _nodeToInsert;
                }
                else
                {
                    _nodeToInsert.next = nextNode;
                }
            }
        }

        public static LinkedList Sum(LinkedList firstList, LinkedList secondList)
        {
            if(firstList.Count() != secondList.Count())
            {
                return null;
            }

            var resultList = new LinkedList();

            var firstListNode = firstList.head;
            var secondListNode = secondList.head;

            while (firstListNode != null)
            {
                var node = new Node(firstListNode.value + secondListNode.value);

                resultList.AddInTail(node);

                firstListNode = firstListNode.next;
                secondListNode = secondListNode.next;
            }

            return resultList;
        }
    }
}



