using System;
using UnityEngine; //Debug.log

public class PartialOrderedTree<T> where T : IComparable<T> {
    /*Private*/

    private T[] heap;
    private int capacity;
    private int last;

    private int left    (int i){ return (2 * i + 1); }
    private int right   (int i){ return (2 * i + 2); }
    private int parent  (int i){ return (i - 1) / 2; }

    private void waft(int i) {
        T data = heap[i];
        while (i > 0 && data.CompareTo(heap[parent(i)]) == -1 ) {
            heap[i] = heap[parent(i)];
            i = parent(i);
        }
        heap[i] = data;
    }

    private void sink(int i){
        bool ended = false;
        T data = heap[i];
        while (left(i) <= last && !ended){
            int minSon;
            if (left(i) < last && heap[right(i)].CompareTo( heap[left(i)] ) == -1) {
                minSon = right(i);
            } else {
                minSon = left(i);
            }
            if (heap[minSon].CompareTo(data) == -1) {
                heap[i] = heap[minSon];
                i = minSon;
            } else {
                ended = true;
            }
        }
        heap[i] = data;
    }

    /*Public*/
    public int Count() {
        return last + 1;
    }


    public PartialOrderedTree(int cap) {
        heap = new T[cap];
        capacity = cap;
        last = -1;
    }
    
    public void insert(T newData) {
        if(last < capacity - 1) {
            heap[++last] = newData;
            waft(last);
        } else {
            Debug.Log("capacity exceeded");
        }
    }

    public void delete() {
        if (last > -1) {
            if (--last > -1) {
                heap[0] = heap[last + 1];
                if (last > 0)
                    sink(0);
            }
        } else {
            Debug.Log("No element to delete");
        }
    }

    public T top() {
        if (last > -1)   // Apo no vacío
            return heap[0];
        else
            return default(T);
    }

    public bool empty() {
        return (last == -1);
    }
}
