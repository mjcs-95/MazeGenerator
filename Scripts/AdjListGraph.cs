using System;                       //Icomparable
using System.Collections.Generic;   //List

public class AdjListGraph<T> where T: IComparable<T> {
    public readonly struct VertexCost : IComparable<VertexCost>, IEquatable<VertexCost>
    {
        public int Vertex { get; }
        public T Cost { get; }

        public VertexCost(int vertex, T cost)
        {
            Vertex = vertex;
            Cost = cost;
        }

        public int CompareTo(VertexCost other) => Cost.CompareTo(other.Cost);
        public bool Equals(VertexCost other) => Vertex == other.Vertex && EqualityComparer<T>.Default.Equals(Cost, other.Cost);
        public override bool Equals(object obj) => obj is VertexCost other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Vertex, Cost);
    }

    //members
    protected readonly List<VertexCost>[] _adj;
    public int NumVert { get; }

    //methods
    public AdjListGraph(int totalVertices) 
    {
        if (totalVertices < 0) 
            throw new ArgumentOutOfRangeException(nameof(totalVertices), "Vertex count cannot be negative.");

        NumVert = totalVertices;
        _adj = new List<VertexCost>[totalVertices];
        for (int i = 0; i < totalVertices; ++i)
            _adj[i] = new List<VertexCost>();
    }

    public AdjListGraph(AdjListGraph<T> sourceGraph) {
        if (sourceGraph == null) 
            throw new ArgumentNullException(nameof(sourceGraph));

        NumVert = sourceGraph.NumVert;
        _adj = new List<VertexCost>[NumVert];
        for (int i = 0; i < NumVert; ++i)
            _adj[i] = new List<VertexCost>(sourceGraph._adj[i]);
    }

    public IReadOnlyList<VertexCost> this[int vertexIndex]
    {
        get
        {
            ValidateVertexIndex(vertexIndex);
            return _adj[vertexIndex];
        }
    }


    public IReadOnlyList<VertexCost> GetAdjacents(int vertexIndex)
    {
        ValidateVertexIndex(vertexIndex);
        return _adj[vertexIndex];
    }

    public bool AddEdge(int origin, int destination, T cost) //O(n)
    {
        if (!IsValidIndex(origin) || !IsValidIndex(destination))
            return false;

        var currentList = _adj[origin];
        for (int k = 0; k < currentList.Count; ++k)
        {
            if (currentList[k].Vertex == destination)
                return false;
        }

        currentList.Add(new VertexCost(destination, cost));
        return true;
    }

    public bool RemoveEdge(int origin, int destination) { // O(n)

        if (!IsValidIndex(origin) || !IsValidIndex(destination))
            return false;

        var currentList = _adj[origin];

        for (int k = 0; k < currentList.Count; ++k)
        {
            if (currentList[k].Vertex == destination)
            {
                currentList.RemoveAt(k);
                return true;
            }
        }
        return false;
    }

    public bool HasEdge(int origin, int destination) { //O(n)
        if (!IsValidIndex(origin) || !IsValidIndex(destination))
            return false;

        var currentList = _adj[origin];
        for (int k = 0; k < currentList.Count; ++k) {
            if (currentList[k].Vertex == destination)
                return true;
        }

        return false;
    }

    public IReadOnlyList<VertexCost> OutEdges(int vertexIndex) { //O(1)
        if (!IsValidIndex(vertexIndex))
            return Array.Empty<VertexCost>();
        return _adj[vertexIndex];
    }

    public IReadOnlyList<VertexCost> InEdges(int targetVertex) { //O(n^2)
        if (!IsValidIndex(targetVertex)) 
            return Array.Empty<VertexCost>();

        List<VertexCost> incomingEdges = new List<VertexCost>();
        for (int k = 0; k < _adj.Length; ++k)
        {
            var currentList = _adj[k];
            for (int j = 0; j < currentList.Count; ++j)
            {
                if (currentList[j].Vertex == targetVertex)
                    incomingEdges.Add(currentList[j]);
            }
        }
        return incomingEdges;
    }
    public IReadOnlyList<VertexCost> InEdgesUndirected(int targetVertex)
    {
        return OutEdges(targetVertex);
    }

    #region Helper Index Validations
    private bool IsValidIndex(int index) => index >= 0 && index < NumVert;
    private void ValidateVertexIndex(int index)
    {
        if (!IsValidIndex(index))
            throw new ArgumentOutOfRangeException(nameof(index), $"Vertex index {index} is outside bounds [0, {NumVert - 1}].");
    }
    #endregion
}

public class MazeGraph<T>: AdjListGraph<T> where T : IComparable<T> {
    //Members
    public int Rows { get; }
    public int Cols { get; }

    //Constructors
    public MazeGraph(int totalVertices, int rows, int cols) : base(totalVertices) 
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentException("Las dimensiones del laberinto deben ser mayores a cero.");

        Rows = rows;
        Cols = cols;
    }

    public MazeGraph(MazeGraph<T> sourceGraph) : base(sourceGraph) 
    {
        if (sourceGraph == null) 
            throw new ArgumentNullException(nameof(sourceGraph));
        
        Rows = sourceGraph.Rows;
        Cols = sourceGraph.Cols;
    }

    //Methods
    
    public (int Row, int Col) GetCoord(int n) 
    {
        return (n / Cols, n % Cols);
    }
    
    public int GetNode(int row, int col) 
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Cols)
            return (row * Cols) + col;        
        return -1;
    }

    public int GetNorth(int row, int col) => GetNode(row - 1, col); // Convención estándar: Norte disminuye fila (o aumenta según tu eje)
    public int GetSouth(int row, int col) => GetNode(row + 1, col);
    public int GetEast(int row, int col) => GetNode(row, col + 1);
    public int GetWest(int row, int col) => GetNode(row, col - 1);

    /// <summary>
    /// Creates a graph without walls, in which each node is connected with all its neighbors.
    /// </summary>
    public static MazeGraph<int> CreateNoWallsGraph4(int rows, int cols) {
        MazeGraph<int> graph = new MazeGraph<int>(rows * cols, rows, cols);
        Random rand = new Random();

        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                int currentNode = graph.GetNode(r, c);
                int cost = rand.Next(0, 10);

                int eastNeighbor = graph.GetEast(r, c);
                if (eastNeighbor != -1)
                {
                    graph.AddEdge(currentNode, eastNeighbor, cost);
                    graph.AddEdge(eastNeighbor, currentNode, cost);
                }

                int southNeighbor = graph.GetSouth(r, c);
                if (southNeighbor != -1)
                {
                    graph.AddEdge(currentNode, southNeighbor, cost);
                    graph.AddEdge(southNeighbor, currentNode, cost);
                }
            }
        }
        return graph;
    }

    private void AddIfValid(int nodeIdx, List<int> list)
    {
        if (nodeIdx != -1) 
            list.Add(nodeIdx);
    }

    public IReadOnlyList<int> GetNeighbors(int row, int col) {
        List<int> neighbors = new List<int>(4);
        AddIfValid(GetNorth(row, col), neighbors);
        AddIfValid(GetSouth(row, col), neighbors);
        AddIfValid(GetEast(row, col), neighbors);
        AddIfValid(GetWest(row, col), neighbors);
        return neighbors;
    }

    public List<int> GetUnconnectedNeighbors(int row, int col) 
    {
        List<int> neighbors = new List<int>(4);
        int currentNode = GetNode(row, col);
        
        if (currentNode == -1) 
            return neighbors;

        int neighbor = GetNorth(row, col);
        if (neighbor != -1 && !HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);
        
        neighbor = GetSouth(row, col);
        if (neighbor != -1 && !HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);
        
        neighbor = GetEast(row, col);
        if (neighbor != -1 && !HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);
        
        neighbor = GetWest(row, col);
        if (neighbor != -1 && !HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);

        return neighbors;
    }

    public List<int> ConnectedNeighbors(int row, int col) 
    {
        List<int> neighbors = new List<int>(4);
        int currentNode = GetNode(row, col);

        if (currentNode == -1)
            return neighbors;

        int neighbor = GetNorth(row, col);
        if (neighbor != -1 && HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);

        neighbor = GetSouth(row, col);
        if (neighbor != -1 && HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);

        neighbor = GetEast(row, col);
        if (neighbor != -1 && HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);

        neighbor = GetWest(row, col);
        if (neighbor != -1 && HasEdge(currentNode, neighbor)) neighbors.Add(neighbor);

        return neighbors;
    }
}