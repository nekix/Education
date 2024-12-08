using AlgorithmsDataStructures;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Exercise4
{
    public static class StackPostfixExpressionEvaluator
    {
        public static double Evaluate(string expression)
        {
            var expParts = expression.Split(' ');

            var expressionStack = new Stack<string>();
            var helpStack = new Stack<double>();

            for(int i = expParts.Length - 1; i >= 0; i--)
                expressionStack.Push(expParts[i]);

            while(expressionStack.Size() != 0)
            {
                var item = expressionStack.Pop();

                if (int.TryParse(item, out var value))
                {
                    helpStack.Push(value);
                    continue;
                }

                switch (item)
                {
                    case "+":
                        CalculateFromStack(helpStack, (x, y) => x + y);
                        break;
                    case "-":
                        CalculateFromStack(helpStack, (x, y) => x - y);
                        break;
                    case "*":
                        CalculateFromStack(helpStack, (x, y) => x * y);
                        break;
                    case "/":
                        CalculateFromStack(helpStack, (x, y) => x / y);
                        break;
                    case "=":
                        return helpStack.Pop();
                }
            }

            return default;
        }

        private static void CalculateFromStack(Stack<double> stack, Func<double, double, double> func)
        {
            var firstValue = stack.Pop();
            var secondValue = stack.Pop();

            stack.Push(func(firstValue, secondValue));
        }
    }
}
