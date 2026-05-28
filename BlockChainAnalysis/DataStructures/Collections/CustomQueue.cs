using System;

namespace BlockChainAnalysis.DataStructures.Collections
{
    public class CustomQueue<T>
    {
        private class Node { public T Data; public Node Next; public Node(T data) { Data = data; Next = null; } }
        private Node _head;
        private Node _tail;
        private int _count;

        public CustomQueue() { _head = null; _tail = null; _count = 0; }

        public void Enqueue(T item)
        {
            Node newNode = new Node(item);
            if (_tail == null) { _head = newNode; _tail = newNode; }
            else { _tail.Next = newNode; _tail = newNode; }
            _count++;
        }

        public T Dequeue()
        {
            if (IsEmpty()) throw new InvalidOperationException("Kuyruk bos.");
            T data = _head.Data;
            _head = _head.Next;
            if (_head == null) _tail = null;
            _count--;
            return data;
        }

        public T Peek()
        {
            if (IsEmpty()) throw new InvalidOperationException("Kuyruk bos.");
            return _head.Data;
        }

        public bool IsEmpty() => _count == 0;
        public int Count => _count;
    }
}
