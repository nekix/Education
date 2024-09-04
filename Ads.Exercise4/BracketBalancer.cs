using AlgorithmsDataStructures;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise4
{
    public static class BracketBalancer
    {
        public static bool CheckIsBalanced(string input)
        {
            var stack = new Stack<char>();

            foreach (var ch in input)
            {
                switch (ch)
                {
                    case '(':
                        stack.Push(ch);
                        break;
                    case ')':
                        if(stack.Size() == 0) return false;
                        stack.Pop();
                        break;
                }
            }

            return stack.Size() == 0;
        }

        public static bool AdvancedCheckIsBalanced(string input)
        {
            var stack = new Stack<char>();

            foreach (var ch in input)
            {
                if (ch == '(' || ch == '[' || ch == '{')
                    stack.Push(ch);
                else if (stack.Size() == 0)
                    return false;
                else if (ch == ')' && stack.Pop() != '(')
                    return false;
                else if (ch == ']' && stack.Pop() != '[')
                    return false;
                else if (ch == '}' && stack.Pop() != '{')
                    return false;
            }

            return stack.Size() == 0;
        }
    }
}
