extern alias Exercise2;
using Exercise2.Ads.Exercise2.CircleDummyLinkedLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_2.CircularLinkedList2Tests
{
    public class CircularLinkedList2_BaseTests
    {
        protected static CircularLinkedList2 GetTestLinkedList(int[] nodValues)
        {
            var list = new CircularLinkedList2();

            for (int i = 0; i < nodValues.Length; i++)
            {
                list.InsertAfter(list.head.prev, new Node(nodValues[i]));
            }

            return list;
        }
    }
}
