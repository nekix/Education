using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise7
{
    public static class OrderListExtensions
    {
        public static OrderedList<T> Merge<T>(this OrderedList<T> first, OrderedList<T> second, bool asc)
        {
            var list = new OrderedList<T>(asc);

            if (first.head == null && second.head == null)
                return list;

            int multiplier = asc ? 1 : -1;

            
            var firstAsc = GetAsceding(first);
            var firstNode = firstAsc ? first.head : first.tail;

            var secondAsc = GetAsceding(second);
            var secondNode = secondAsc ? second.head : second.tail;

            while (firstNode != null || secondNode != null)
            {
                if(firstNode != null)
                {
                    if(secondNode == null || list.Compare(firstNode.value, secondNode.value) * multiplier <= 0)
                    {
                        list.Add(firstNode.value);
                        firstNode = firstAsc
                            ? firstNode.next
                            : firstNode.prev;
                    }
                    else
                    {
                        list.Add(secondNode.value);
                        secondNode = secondAsc
                            ? secondNode.next
                            : secondNode.prev;
                    }
                }
            }

            return list;
        }

        public static bool Contains<T>(this OrderedList<T> firstList, OrderedList<T> secondList)
        {
            if (secondList.head == null || firstList.head == null)
                return false;

            var secondCurrentNode = secondList.head;
            var firstStartNode = firstList.Find(secondCurrentNode.value);

            if (firstStartNode == null) return false;

            secondCurrentNode = secondCurrentNode.next;
            var firstCurrentNode = firstStartNode.next;

            while (secondCurrentNode != null)
            {
                if(firstCurrentNode == null)
                    return false;

                if(firstList.Compare(firstCurrentNode.value, secondCurrentNode.value) != 0)
                {
                    // Take next node in first list and compare with second list
                    firstStartNode = firstStartNode.next;

                    firstCurrentNode = firstStartNode;
                    secondCurrentNode = secondList.head;
                    
                    if(firstCurrentNode.next == null
                        || firstList.Compare(firstCurrentNode.value, secondCurrentNode.value) != 0)
                        return false;
                }

                firstCurrentNode = firstCurrentNode.next;
                secondCurrentNode = secondCurrentNode.next;
            }

            return true;
        }

        public static T GetMaxCommonValue<T>(this OrderedList<T> list)
        {
            if (list.head == null)
                return default;

            T maxCommonValue = default;
            var maxCommonCount = 0;
            
            var currentValue = list.head.value;
            var currentCount = 1;

            for (var currentNode = list.head.next; currentNode != null; currentNode = currentNode.next)
            {
                if (list.Compare(currentValue, currentNode.value) != 0)
                {
                    if (currentCount > maxCommonCount)
                    {
                        maxCommonValue = currentValue;
                        maxCommonCount = currentCount;
                    }

                    currentValue = currentNode.value;
                    currentCount = 0;
                }
                else
                {
                    currentCount++;
                }
            }

            if (currentCount > maxCommonCount)
            {
                maxCommonValue = currentValue;
                maxCommonCount = currentCount;
            }

            return maxCommonValue;
        }

        private static bool GetAsceding<T>(OrderedList<T> list)
        {
            if (list.head == null)
                return true;

            return list.Compare(list.head.value, list.tail.value) <= 0
                ? true
                : false;
        }
    }
}
