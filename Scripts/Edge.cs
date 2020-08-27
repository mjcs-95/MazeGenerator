using System;       //Icomparable

public class Edge<T>: IComparable<Edge<T>>  
        where T : IComparable<T>   
{
    public int orig, dest;
    public T cost;
    public Edge(int v, int w, T c) 
    {
        orig = v;
        dest = w;
        cost = c;
    }

    public int CompareTo(Edge<T> other) 
    {
        if (other == null) 
            return 1;
        return cost.CompareTo(other.cost);
    }
}
