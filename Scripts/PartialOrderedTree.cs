using System;
using UnityEngine;
using static UnityEditor.Progress;

public class PartialOrderedTree<T> where T : IComparable<T> {
    private T[] _heap;
    private int _count;

    public int Count => _count;
    public bool IsEmpty => _count == 0;
    public int Capacity => _heap.Length;


    private static int GetLeftChild(int i) => (i << 1) + 1;
    private static int GetRightChild(int i) => (i << 1) + 2;
    private static int GetParent(int i) => (i - 1) >> 1;

    public PartialOrderedTree(int initialCapacity = 16)
    {
        if (initialCapacity <= 0) initialCapacity = 16;
        _heap = new T[initialCapacity];
        _count = 0;
    }

    public T Top()
    {
        if (IsEmpty)
            throw new InvalidOperationException("The tree is empty.");
        return _heap[0];
    }

    private void Resize(int newCapacity)
    {
        T[] newHeap = new T[newCapacity];
        Array.Copy(_heap, newHeap, _count);
        _heap = newHeap;
    }

    public void Clear()
    {
        Array.Clear(_heap, 0, _count);
        _count = 0;
    }

    private void Waft(int i) 
    {
        T data = _heap[i];
        while (i > 0) 
        {
            int parentIdx = GetParent(i);
            if (data.CompareTo(_heap[parentIdx]) >= 0)
                break;

            _heap[i] = _heap[parentIdx];
            i = parentIdx;
        }
        _heap[i] = data;
    }

    private void Sink(int i)
    {
        T data = _heap[i];
        int leftIdx;

        while ((leftIdx = GetLeftChild(i)) < _count)
        {
            int rightIdx = GetRightChild(i);
            int minSon = leftIdx;

            if (rightIdx < _count && _heap[rightIdx].CompareTo(_heap[leftIdx]) < 0)
            {
                minSon = rightIdx;
            }

            if (data.CompareTo(_heap[minSon]) <= 0)
                break;

            _heap[i] = _heap[minSon];
            i = minSon;
        }
        _heap[i] = data;
    }


    public void Insert(T item) 
    {
        if (_count == _heap.Length)
        {
            Resize(_heap.Length * 2);
        }
        _heap[_count] = item;
        Waft(_count);
        _count++;
    }

    public T Pop() 
    {
        if (IsEmpty)
            throw new InvalidOperationException("The tree is empty.");

        T root = _heap[0];
        _count--;

        if (_count > 0)
        {
            _heap[0] = _heap[_count];
            Sink(0);
        }
        _heap[_count] = default;

        return root;
    }

}
