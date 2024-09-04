using System.Collections.Generic;

namespace AlgorithmsDataStructures
{
    public class IntStack : Stack<int>
    {
        private List<int> MinInnerList;
        private double _sum;

        public IntStack()
        {
            MinInnerList = new List<int>();
        }

        public override int Pop()
        {
            var size = Size();

            var value = base.Pop();

            if(size != 0)
            {
                if(value == MinInnerList[MinInnerList.Count - 1])
                    MinInnerList.RemoveAt(MinInnerList.Count - 1);
                
                _sum -= value;
            }

            return value;
        }

        public override void Push(int val)
        {
            if (Size() == 0 || MinInnerList[MinInnerList.Count - 1] >= val)
                MinInnerList.Add(val);

            _sum += val;

            base.Push(val);
        }

        public int PeekMin()
        {
            if (Size() == 0)
                return default;

            return MinInnerList[MinInnerList.Count - 1];
        }

        public double GetMiddle()
        {
            if(Size() == 0) return default;

            return _sum / Size();
        }
    }

}