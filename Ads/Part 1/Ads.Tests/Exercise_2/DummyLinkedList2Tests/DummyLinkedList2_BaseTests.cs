extern alias Exercise2;
using Exercise2.Ads.Exercise2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Tests.Exercise_2.DummyLinkedList2Tests
{
    public class DummyLinkedList2_BaseTests
    {
        protected static DummyLinkedList2 GetTestLinkedList(int[] nodValues)
        {
            var list = new DummyLinkedList2();

            for(int i = 0; i < nodValues.Length; i++)
            {
                list.InsertAfter(list.tail.prev, new Node(nodValues[i]));
            }

            return list;
        }
    }
}
