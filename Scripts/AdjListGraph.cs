using System;                       //Icomparable
using System.Collections.Generic;   //List

public class AdjListGraph<T> where T: IComparable<T> {
    //data classes
    public class VertexCost : IComparable<VertexCost> {
        public int vertex;
        public T cost;
        public VertexCost(int v, T c) {
            vertex = v;
            cost = c;
        }
        public int CompareTo(VertexCost other) {
            if (other == null) return 1;
            return cost.CompareTo(other.cost);
        }
    };        

    //members
    protected List<VertexCost>[] adj;
    protected int N;
    public static T infinite = (T) typeof(T).GetField("MaxValue").GetValue(null);
        
    //methods
    public AdjListGraph(int n) {
        N = n;
        adj = new List<VertexCost>[n];
        for (int i = 0; i < n; ++i) {
            adj[i] = new List<VertexCost>();
        }
    }

    public AdjListGraph(AdjListGraph<T> G) {
        N = G.numVert();
        adj = new List<VertexCost>[N];
        for (int i = 0; i < N; ++i) {
            adj[i] = new List<VertexCost>(G.Adjacents(i).GetRange(0, G.Adjacents(i).Count));
        }
    }

    public List<VertexCost> this[int i] {
        get { return adj[i]; }
    }

    public int numVert() {
        return N;
    }

    public List<VertexCost> Adjacents(int i) {
        return new List<VertexCost>(adj[i]);
    }

    public bool addEdge(int i, int j, T c) { //O(n) , n=count
        if (i < 0 || i >= adj.Length) return false;
        for(int k = 0; k < adj[i].Count; ++k) {
            if (adj[i][k].vertex == j) {
                return false;
            }
        }
        adj[i].Add(new VertexCost(j, c));
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

    public List<VertexCost> outEdges(int i) { //O(1)
        if (i > 0 || i < adj.Length) {
            return adj[i];
        }
        return new List<VertexCost>();
    }

    public List<VertexCost> inEdges(int i) { //O(n^2)
        if (i > 0 || i < adj.Length) {
            List<VertexCost> inEdges = new List<VertexCost>();
            for (int k = 0; k < adj.Length; ++k) {
                for (int j = 0; j < adj[k].Count; ++j) {
                    if(adj[k][j].vertex == i) {
                        inEdges.Add(adj[k][j]);
                    }
                }
            }
            return inEdges;
        }
        return new List<VertexCost>();
    }
}

public class MazeGraph<T>: AdjListGraph<T> where T : IComparable<T> {

    public static MazeGraph<int> createNoWallsGraph4(int rows, int cols) {
        int n = rows * cols;
        /*Build the Full Graph*/
        MazeGraph<int> G = new MazeGraph<int>(n, rows, cols);
        System.Random rand = new System.Random();
        int aux = 0;
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j, ++aux) {
                int cost = rand.Next(1, 10);
                //Random values to use with prim and kruskal but it could be 1; Still not undirected
                if (j < cols - 1) {
                    G.addEdge(aux, aux + 1, rand.Next(1, n) ); //Random.Range(1, n)
                    G.addEdge(aux + 1, aux, rand.Next(1, n));
                }// North
                if (i > 0) {
                    G.addEdge(aux, aux - cols, rand.Next(1, n)); // west
                    G.addEdge(aux - cols, aux, rand.Next(1, n)); // west
                }
                if (j > 0) {
                    G.addEdge(aux, aux - 1, rand.Next(1, n)); // South
                    G.addEdge(aux - 1, aux, rand.Next(1, n)); // South
                }
                if (i < rows - 1) {
                    G.addEdge(aux, aux + cols, rand.Next(1, n)); // East
                    G.addEdge(aux + cols, aux, rand.Next(1, n)); // East
                }
            }
        }
        return G;
    }

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

    public List<int> UnconnectedNeighbors(int row, int col) {
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

    public List<int> ConnectedNeighbors(int row, int col) {
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
        return GetNode(row + 1, col);
    }

    public int GetSouth(int row, int col) {
        return GetNode(row - 1, col);
    }

    public int GetEast(int row, int col) {
        return GetNode(row, col + 1);
    }

    public int GetWest(int row, int col) {
        return GetNode(row, col - 1);
    }
}


