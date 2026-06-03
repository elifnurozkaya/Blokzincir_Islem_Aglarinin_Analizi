using System;

namespace BlockChainAnalysis.DataStructures.Collections
{
    public class CustomStack<T>
    {
        private class Node { public T Data; public Node Next; public Node(T data) { Data = data; Next = null; } }
        private Node _top;
        private int _count;

        public CustomStack() { _top = null; _count = 0; }

        public void Push(T item)
        {
            Node newNode = new Node(item) { Next = _top };
            _top = newNode;
            _count++;
        }

        public T Pop()
        {
            if (IsEmpty()) throw new InvalidOperationException("Yigit bos.");
            T data = _top.Data;
            _top = _top.Next;
            _count--;
            return data;
        }

        public T Peek()
        {
            if (IsEmpty()) throw new InvalidOperationException("Yigit bos.");
            return _top.Data;
        }

        public bool IsEmpty() => _count == 0;
        public int Count => _count;
    }
}
