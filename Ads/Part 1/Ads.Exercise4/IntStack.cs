using System.Collections.Generic;

namespace AlgorithmsDataStructures
{
    public class IntStack : Stack<int>
    {
        private Stack<int> _minItemsStack;
        private double _sum;

        public IntStack()
        {
            _minItemsStack = new Stack<int>();
        }

        public override int Pop()
        {
            var size = Size();

            var value = base.Pop();

            if(size != 0)
            {
                if(value == _minItemsStack.Peek())
                    _minItemsStack.Pop();
                
                _sum -= value;
            }

            return value;
        }

        public override void Push(int val)
        {
            if (Size() == 0 || _minItemsStack.Peek() >= val)
                _minItemsStack.Push(val);

            _sum += val;

            base.Push(val);
        }

        public int PeekMin()
        {
            if (Size() == 0)
                return default;

            return _minItemsStack.Pop();
        }

        public double GetMiddle()
        {
            if(Size() == 0) return default;

            return _sum / Size();
        }
    }

}