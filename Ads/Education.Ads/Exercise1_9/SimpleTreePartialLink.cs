using Ads.Exercise6;

namespace AlgorithmsDataStructures2
{
    public partial class SimpleTree<T>
    {
        public bool IsSymmetricallyIterative()
        {
            if (Root == null)
                return true;

            // Т.к. по умолчанию в C# нет реализации деки, я использовал свою из первой части курса АСД
            ImprovedDeque<SimpleTreeNode<T>> nodesDeque = new ImprovedDeque<SimpleTreeNode<T>>();
            nodesDeque.AddFront(Root);

            while (nodesDeque.Size() > 0)
            {
                SimpleTreeNode<T> leftNode = nodesDeque.RemoveFront();

                if (nodesDeque.Size() != 0)
                {
                    SimpleTreeNode<T> rightNode = nodesDeque.RemoveTail();

                    if (!leftNode.NodeValue.Equals(rightNode.NodeValue))
                        return false;

                    if (leftNode.Children == null && leftNode.Children == null)
                        continue;

                    if (leftNode.Children.Count != rightNode.Children.Count)
                        return false;

                    for (int i = rightNode.Children.Count - 1; i >= 0; i--)
                        nodesDeque.AddTail(rightNode.Children[i]);
                }

                if (leftNode.Children != null)
                    foreach (SimpleTreeNode<T> child in leftNode.Children)
                        nodesDeque.AddFront(child);
            }

            return true;
        }
    }
}
