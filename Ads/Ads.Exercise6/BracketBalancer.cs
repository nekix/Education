using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmsDataStructures;

namespace Ads.Exercise6
{
    public static class BracketBalancer
    {
        public static bool CheckIsBalanced(string input)
        {
            var deque = new Deque<char>();

            foreach (var ch in input)
            {
                if (ch == '(' || ch == '[' || ch == '{')
                    deque.AddFront(ch);
                else if (ch == ')' && deque.RemoveFront() != '(')
                    return false;
                else if (ch == ']' && deque.RemoveFront() != '[')
                    return false;
                else if (ch == '}' && deque.RemoveFront() != '{')
                    return false;
            }

            return deque.Size() == 0;
        }
    }
}
