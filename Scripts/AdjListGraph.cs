using System;                       //Icomparable
using System.Collections.Generic;   //List

public class AdjListGraph<T> where T: IComparable<T> {
    //data classes
    public class vertexCost : IComparable<vertexCost> {
        public int vertex;
        public T cost;
        public vertexCost(int v, T c) {
            vertex = v;
            cost = c;
        }
        public int CompareTo(vertexCost other) {
            if (other == null) return 1;
            return cost.CompareTo(other.cost);
        }
    };        

    //members
    protected List<vertexCost>[] adj;
    protected int N;
    public static T infinite = (T) typeof(T).GetField("MaxValue").GetValue(null);
        
    //methods
    public AdjListGraph(int n) {
        N = n;
        adj = new List<vertexCost>[n];
        for (int i = 0; i < n; ++i) {
            adj[i] = new List<vertexCost>();
        }
    }

    public AdjListGraph(AdjListGraph<T> G) {
        N = G.numVert();
        adj = new List<vertexCost>[N];
        for (int i = 0; i < N; ++i) {
            adj[i] = new List<vertexCost>(G.Adjacents(i).GetRange(0, G.Adjacents(i).Count));
        }
    }

    public List<vertexCost> this[int i] {
        get { return adj[i]; }
    }

    public int numVert() {
        return N;
    }

    public List<vertexCost> Adjacents(int i) {
        return new List<vertexCost>(adj[i]);
    }

    public bool addEdge(int i, int j, T c) { //O(n) , n=count
        if (i < 0 || i >= adj.Length) return false;
        for(int k = 0; k < adj[i].Count; ++k) {
            if (adj[i][k].vertex == j) {
                return false;
            }
        }
        adj[i].Add(new vertexCost(j, c));
        return true;
    }

    public bool removeEdge(int i, int j) { // O(n)
        if (i < 0 || i >= adj.Length) return false;
        for (int k = 0; k < adj[i].Count; ++k) {
            if (adj[i][k].vertex == j) {
                adj[i].RemoveAt(k);
                return true;
            }
        }
        return false;
    }

    public bool hasEdge(int i, int j) { //O(n)
        if (i < 0 || i >= adj.Length) return false;
        for (int k = 0; k < adj[i].Count; ++k) {
            if (adj[i][k].vertex == j) {
                return true;
            }
        }
        return false;
    }

    public List<vertexCost> outEdges(int i) { //O(1)
        if (i > 0 || i < adj.Length) {
            return adj[i];
        }
        return new List<vertexCost>();
    }

    public List<vertexCost> inEdges(int i) { //O(n^2)
        if (i > 0 || i < adj.Length) {
            List<vertexCost> inEdges = new List<vertexCost>();
            for (int k = 0; k < adj.Length; ++k) {
                for (int j = 0; j < adj[k].Count; ++j) {
                    if(adj[k][j].vertex == i) {
                        inEdges.Add(adj[k][j]);
                    }
                }
            }
            return inEdges;
        }
        return new List<vertexCost>();
    }
}

public class MazeGraph<T>: AdjListGraph<T> where T : IComparable<T> {
    public int rows { get; }
    public int cols { get; }

    public MazeGraph(int n, int rows = -1, int cols=-1):base(n) {
        this.rows = rows;
        this.cols = cols;
    }

    public MazeGraph(MazeGraph<T> G): base(G) {
        rows = G.rows;
        cols = G.cols;
    }
    
    public List<int> GetCoord(int n) {
        return new List<int>() { n / cols, n % cols };
    }

    public List<int> neighbors(int row, int col) {
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

    public List<int> UnconectedNeighbors(int row, int col) {
        List<int> neighbors = new List<int>();
        int n = GetNode(row, col);
        if (rows != -1 && cols != -1) {
            if (n != -1) {
                int neighbor;
                neighbor = GetNorth(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) neighbors.Add(neighbor);
                neighbor = GetSouth(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) neighbors.Add(neighbor);
                neighbor = GetEast(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) neighbors.Add(neighbor);
                neighbor = GetWest(row, col);
                if (neighbor != -1 && !hasEdge(n, neighbor)) neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    public List<int> ConectedNeighbors(int row, int col) {
        List<int> neighbors = new List<int>();
        if (rows != -1 && cols != -1) {
            int n = GetNode(row, col);
            if (n != -1) {
                if (hasEdge(n, GetNorth(row, col))) neighbors.Add(GetNorth(row, col));
                if (hasEdge(n, GetSouth(row, col))) neighbors.Add(GetSouth(row, col));
                if (hasEdge(n, GetEast(row, col))) neighbors.Add(GetEast(row, col));
                if (hasEdge(n, GetWest(row, col))) neighbors.Add(GetWest(row, col));
            }
        }
        return neighbors;
    }

    public int GetNode(int row,int col) {
        if ((0 <= row && row < rows && rows != -1) && (0 <= col && col < cols && cols != -1)) {
            return row * cols + col;
        } else {
            return -1;
        }
    }

    public int GetNorth(int row, int col) {
        return GetNode(row - 1, col);
    }

    public int GetSouth(int row, int col) {
        return GetNode(row + 1, col);
    }

    public int GetEast(int row, int col) {
        return GetNode(row, col + 1);
    }

    public int GetWest(int row, int col) {
        return GetNode(row, col - 1);
    }
}


