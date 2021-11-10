using System;
using UnityEngine; //Debug.log
using UnityEngine.Assertions;

public class PartialOrderedTree<T> where T : IComparable<T> {
    /*Private*/

    private T[] heap;
    private int capacity;
    private int last;

    private int left    (int i){ return (2 * i + 1); }
    private int right   (int i){ return (2 * i + 2); }
    private int parent  (int i){ return (i - 1) / 2; }

    private void waft(int i) 
    {
        T data = heap[i];
        while (i > 0 && data.CompareTo(heap[parent(i)]) == -1 ) 
        {
            heap[i] = heap[parent(i)];
            i = parent(i);
        }
        heap[i] = data;
    }

    private void sink(int i)
    {
        bool ended = false;
        T data = heap[i];
        while (left(i) <= last && !ended)
        {
            int minSon;
            if (left(i) < last && heap[right(i)].CompareTo( heap[left(i)] ) == -1)
                minSon = right(i);
            else
                minSon = left(i);
            if (heap[minSon].CompareTo(data) == -1) 
            {
                heap[i] = heap[minSon];
                i = minSon;
            } 
            else
                ended = true;
        }
        heap[i] = data;
    }

    /*Public*/
    public int Count() 
    {
        return last + 1;
    }


    public PartialOrderedTree(int cap) 
    {
        heap = new T[cap];
        capacity = cap;
        last = -1;
    }
    
    public void insert(T newData) 
    {
        Debug.Assert(last < capacity - 1, "Capacity exceeded");
        heap[++last] = newData;
        waft(last);
    }

    public void delete() 
    {
        Debug.Assert(last > -1,"Empty Tree");
        if (last > -1)
            if (--last > -1) 
            {
                heap[0] = heap[last + 1];
                if (last > 0)
                    sink(0);
            }
    }

    public T top() 
    {
        Debug.Assert(last > -1, "Empty Tree");
        return heap[0];
    }

    public bool empty() 
    {
        return last == -1;
    }
}
