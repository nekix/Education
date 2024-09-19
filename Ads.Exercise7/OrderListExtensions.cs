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
                    else if(secondNode != null)
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
