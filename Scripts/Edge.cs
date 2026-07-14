using System;

public readonly struct Edge<T>: IComparable<Edge<T>>, IEquatable<Edge<T>> where T : IComparable<T>   
{
    public int Origin { get; }
    public int Destination { get; }
    public T Cost { get; }

    public Edge(int origin, int destination, T cost)
    {
        Origin = origin;
        Destination = destination;
        Cost = cost;
    }

    #region IComparable<GraphNode> Members
    public int CompareTo(Edge<T> other) => Cost.CompareTo(other.Cost);
    #endregion

    #region Iequatable<GraphNode> Members
    public bool Equals(Edge<T> other) =>
        Origin == other.Origin &&
        Destination == other.Destination &&
        (Cost?.Equals(other.Cost) ?? other.Cost == null);

    public override bool Equals(object obj) => obj is Edge<T> other && Equals(other);
    #endregion

    #region Utility Methods
    public override int GetHashCode() => HashCode.Combine(Origin, Destination, Cost);
    public static bool operator ==(Edge<T> left, Edge<T> right) => left.Equals(right);
    public static bool operator !=(Edge<T> left, Edge<T> right) => !left.Equals(right);
    #endregion
}
