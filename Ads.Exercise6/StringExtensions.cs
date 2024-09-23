using AlgorithmsDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise6
{
    public static class StringExtensions
    {
        public static bool GetIsPalindrome(this string str)
        {
            if (str.Length == 0)
                throw new ArgumentException();

            var deque = new Deque<Char>();

            foreach (var ch in str)
                deque.AddFront(ch);

            while (deque.Size() > 1)
                if (deque.RemoveTail() != deque.RemoveFront())
                    return false;

            return true;
        }
    }
}
