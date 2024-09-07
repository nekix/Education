using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures
{

    public class Queue<T>
    {
        public Queue()
        {
            // инициализация внутреннего хранилища очереди
        }

        public void Enqueue(T item)
        {
            // вставка в хвост
        }

        public T Dequeue()
        {
            // выдача из головы
            return default(T); // если очередь пустая
        }

        public int Size()
        {
            return 0; // размер очереди
        }

    }
}