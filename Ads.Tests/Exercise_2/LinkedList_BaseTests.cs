extern alias Exercise2;

using Exercise2.AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_2
{
    public class LinkedList_BaseTests
    {
        protected static LinkedList2 GetTestLinkedList(int[] nodValues)
        {
            var list = new LinkedList2();

            foreach (var nodValue in nodValues)
            {
                list.AddInTail(new Node(nodValue));
            }

            return list;
        }
    }
}
