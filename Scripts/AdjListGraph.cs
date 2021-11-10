using System;                       //Icomparable
using System.Collections.Generic;   //List

public class AdjListGraph<T> where T: IComparable<T> {
    //data classes
    public class VertexCost : IComparable<VertexCost> {
        public int vertex;
        public T cost;
        public VertexCost(int v, T c) 
        {
            vertex = v;
            cost = c;
        }
        public int CompareTo(VertexCost other) 
        {
            if (other == null) 
                return 1;
            return cost.CompareTo(other.cost);
        }
    };        

    //members
    protected List<VertexCost>[] adj;
    public static T infinite = (T) typeof(T).GetField("MaxValue").GetValue(null);
    public int NumVert { get; }

    //methods
    public AdjListGraph(int n) 
    {
        NumVert = n;
        adj = new List<VertexCost>[n];
        for (int i = 0; i < n; ++i)
            adj[i] = new List<VertexCost>();
    }

    public AdjListGraph(AdjListGraph<T> G) {
        NumVert = G.NumVert;
        adj = new List<VertexCost>[NumVert];
        for (int i = 0; i < NumVert; ++i)
            adj[i] = new List<VertexCost>(G.Adjacents(i).GetRange(0, G.Adjacents(i).Count));
    }

    public List<VertexCost> this[int i] {
        get { return adj[i]; }
    }

    

    public List<VertexCost> Adjacents(int i) 
    {
        return new List<VertexCost>(adj[i]);
    }

    public bool addEdge(int i, int j, T c) //O(n)
    { 
        if (i < 0 || i >= adj.Length) 
            return false;
        for(int k = 0; k < adj[i].Count; ++k)
            if (adj[i][k].vertex == j)
                return false;
        adj[i].Add(new VertexCost(j, c));
        return true;
    }

    public bool removeEdge(int i, int j) { // O(n)
        if (i < 0 || i >= adj.Length) 
            return false;
        for (int k = 0; k < adj[i].Count; ++k) 
            if (adj[i][k].vertex == j) 
            {
                adj[i].RemoveAt(k);
                return true;
            }
        return false;
    }

    public bool hasEdge(int i, int j) { //O(n)
        if (i < 0 || i >= adj.Length) 
            return false;
        for (int k = 0; k < adj[i].Count; ++k)
            if (adj[i][k].vertex == j)
                return true;
        return false;
    }

    public List<VertexCost> outEdges(int i) { //O(1)
        if (i > 0 || i < adj.Length)
            return adj[i];
        return new List<VertexCost>();
    }

    public List<VertexCost> inEdges(int i) { //O(n^2)
        if (i > 0 || i < adj.Length) {
            List<VertexCost> inEdges = new List<VertexCost>();
            for (int k = 0; k < adj.Length; ++k)
                for (int j = 0; j < adj[k].Count; ++j)
                    if(adj[k][j].vertex == i)
                        inEdges.Add(adj[k][j]);
            return inEdges;
        }
        return new List<VertexCost>();
    }
}

public class MazeGraph<T>: AdjListGraph<T> where T : IComparable<T> {
    //Members
    public int rows { get; }
    public int cols { get; }

    //Constructors
    public MazeGraph(int n, int rows = -1, int cols = -1) : base(n) 
    {
        this.rows = rows;
        this.cols = cols;
    }

    public MazeGraph(MazeGraph<T> G) : base(G) 
    {
        rows = G.rows;
        cols = G.cols;
    }

    //Methods
    
    public List<int> GetCoord(int n) 
    {
        return new List<int>() { n / cols, n % cols };
    }
    
    public int GetNode(int row, int col) 
    {
        if ((0 <= row && row < rows && rows != -1) && (0 <= col && col < cols && cols != -1))
            return row * cols + col;
        else
            return -1;
    }

    public int GetNorth(int row, int col) { return GetNode(row + 1, col); }
    public int GetSouth(int row, int col) { return GetNode(row - 1, col); }
    public int GetEast(int row, int col) { return GetNode(row, col + 1); }
    public int GetWest(int row, int col) { return GetNode(row, col - 1); }

    /**
     *  if you have 
     *      int n = GetNode(row, col);
     *  them 
     *      GetNorth() = n + cols ; GetSouth() = n - cols ;
     *      GetEast()  = n + 1    ; GetWest()  = n - 1
     * */

    public static MazeGraph<int> CreateNoWallsGraph4(int rows, int cols) {
        MazeGraph<int> G = new MazeGraph<int>(rows * cols, rows, cols);
        System.Random rand = new System.Random();
        int aux = 0;
        for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j, ++aux) {
                int cost = rand.Next(0,10);                
                if (j < cols - 1) // North
                {
                    G.addEdge(aux, aux + 1, cost);
                    G.addEdge(aux + 1, aux, cost);
                }
                if (i > 0)  // west
                {
                    G.addEdge(aux, aux - cols, cost);
                    G.addEdge(aux - cols, aux, cost);
                }
                if (j > 0) // South
                {
                    G.addEdge(aux, aux - 1, cost); 
                    G.addEdge(aux - 1, aux, cost); 
                }
                if (i < rows - 1)  // East
                {
                    G.addEdge(aux, aux + cols, cost);
                    G.addEdge(aux + cols, aux, cost);
                }
            }
        return G;
    }

    public List<int> Neighbors(int row, int col) {
        List<int> neighbors = new List<int>();
        if(rows != -1 && cols != -1) {
            int neighbor;
            neighbor = GetNorth(row, col);
            if (neighbor != -1) neighbors.Add(neighbor);
            neighbor = GetSouth(row, col);
            if (neighbor != -1) neighbors.Add(neighbor);
            neighbor = GetEast(row, col);
            if (neighbor != -1) neighbors.Add(neighbor);
            neighbor = GetWest(row, col);
            if (neighbor != -1) neighbors.Add(neighbor);

        }
        return neighbors;
    }

    public List<int> UnconnectedNeighbors(int row, int col) 
    {
        List<int> neighbors = new List<int>();
        int n = GetNode(row, col);
        if (rows != -1 && cols != -1) 
        {
            if (n != -1) 
            {
                int neighbor = GetNorth(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) { neighbors.Add(neighbor); }
                neighbor = GetSouth(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) { neighbors.Add(neighbor); }
                neighbor = GetEast(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) { neighbors.Add(neighbor); }
                neighbor = GetWest(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) { neighbors.Add(neighbor); }
            }
        }
        return neighbors;
    }

    public List<int> ConnectedNeighbors(int row, int col) 
    {
        List<int> neighbors = new List<int>();
        if (rows != -1 && cols != -1) 
        {
            int n = GetNode(row, col);
            if (n != -1) 
            {
                if (hasEdge(n, n + cols)) { neighbors.Add(n + cols); }
                if (hasEdge(n, n - cols)) { neighbors.Add(n - cols); }
                if (hasEdge(n, n + 1)) { neighbors.Add(n + 1); }
                if (hasEdge(n, n - 1)) { neighbors.Add(n - 1); }
            }
        }
        return neighbors;
    }

}


