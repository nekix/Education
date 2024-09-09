namespace AlgorithmsDataStructures
{
    public static class QueueExtensions
    {
        public static Queue<T> Rotate<T>(this Queue<T> queue, int rotateCount)
        {
            for (int i = 0; i < rotateCount; i++)
                queue.Enqueue(queue.Dequeue());

            return queue;
        }
    }
}