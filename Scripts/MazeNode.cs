using System;

public struct GraphNode: IComparable<GraphNode>
{
    int id;
    int row;
    int col;

    public GraphNode(int i, int r, int c) {
        id = i;
        row = r;
        col = c;
    }

    public int CompareTo(GraphNode other) {
        return id.CompareTo(other.id);
    }
}
