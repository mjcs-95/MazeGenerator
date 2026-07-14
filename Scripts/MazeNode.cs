using System;

public readonly struct GraphNode: IComparable<GraphNode>, IEquatable<GraphNode>
{
    public int Id { get; }
    public int Row { get; }
    public int Col { get; }

    public GraphNode(int id, int row, int col) 
    {
        Id = id;
        Row = row;
        Col = col;
    }
    #region IComparable<GraphNode> Members
    public int CompareTo(GraphNode other) => Id.CompareTo(other.Id);
    #endregion

    #region Iequatable<GraphNode> Members
    public bool Equals(GraphNode other) => Id == other.Id && Row == other.Row && Col == other.Col;
    public override bool Equals(object obj) => obj is GraphNode other && Equals(other);
    #endregion

    #region Utility Methods
    public override int GetHashCode() => HashCode.Combine(Id, Row, Col);
    public static bool operator ==(GraphNode left, GraphNode right) => left.Equals(right);
    public static bool operator !=(GraphNode left, GraphNode right) => !left.Equals(right);
    #endregion
}
