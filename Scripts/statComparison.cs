using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class StatComparison<T> where T : IComparable<T> 
{
    MazeGraph<int> graph;

    private string DataPath;

    public readonly struct MazeStats
    {
        public float DeadEnds { get; }
        public float Intersections { get; }
        public float Directness { get; }
        public float Twistiness { get; }

        public MazeStats(float deadEnds, float intersections, float directness, float twistiness)
        {
            DeadEnds = deadEnds;
            Intersections = intersections;
            Directness = directness;
            Twistiness = twistiness;
        }
    }

    public StatComparison(string path) {
        DataPath = path;
    }

    public MazeGraph<int> executeAlgorithm(MazeGenerator.Algorithm generationAlgorithm, MazeGraph<int> inputGraph) => MazeGenerator.AlgorithmMap[generationAlgorithm](inputGraph);
    
    private float CalculatePercentage(int count, int total) => total > 0 ? 100.0f * count / total : 0.0f;

    private MazeStats AnalyzeMaze(MazeGraph<T> inputGraph)
    {
        int deadEndsCount = 0;
        int intersectionsCount = 0;
        int directCount = 0;
        int twistsCount = 0;

        int rows = inputGraph.Rows;
        int cols = inputGraph.Cols;
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                int current = inputGraph.GetNode(i, j);

                bool n = i < rows - 1 && inputGraph.HasEdge(current, current + cols);
                bool s = i > 0        && inputGraph.HasEdge(current, current - cols);
                bool e = j < cols - 1 && inputGraph.HasEdge(current, current + 1);
                bool w = j > 0        && inputGraph.HasEdge(current, current - 1);

                int connectedCount = (n ? 1 : 0) + (s ? 1 : 0) + (e ? 1 : 0) + (w ? 1 : 0);

                if (connectedCount == 1)
                    ++deadEndsCount;
                else if (connectedCount > 2)
                    ++intersectionsCount;

                if ((n == s) && (w == e) && (n != e))
                    ++directCount;

                if ((n || s) && (e || w) && !(n == s && e == w))
                    ++twistsCount;
            }
        }

        int totalVertices = inputGraph.NumVert;
        return new MazeStats(
            CalculatePercentage(deadEndsCount, totalVertices),
            CalculatePercentage(intersectionsCount, totalVertices),
            CalculatePercentage(directCount, totalVertices),
            CalculatePercentage(twistsCount, totalVertices)
        );
    }


    private readonly struct PathNode
    {
        public readonly int NodeId;
        public readonly int Cost;
        public PathNode(int nodeId, int cost)
        {
            NodeId = nodeId;
            Cost = cost;
        }
    }

    public float LongestPath(MazeGraph<T> inputGraph)
    {

        if (inputGraph == null) 
            return 0.0f;

        int numVert = inputGraph.NumVert;
        int rows = inputGraph.Rows;
        int cols = inputGraph.Cols;

        bool[] visited = new bool[numVert];
        int maxPathLength = 0;
        Queue<PathNode> queue = new Queue<PathNode>(numVert);

        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                Array.Clear(visited, 0, numVert);
                queue.Clear();

                int startNode = inputGraph.GetNode(i, j);
                visited[inputGraph.GetNode(i, j)] = true;

                foreach (var neighbor in inputGraph.ConnectedNeighbors(i, j))
                    queue.Enqueue(new PathNode(neighbor, 1));

                while (queue.Count != 0)
                {
                    PathNode current = queue.Dequeue() ;
                    visited[current.NodeId] = true;

                    if (maxPathLength < current.Cost)
                        maxPathLength = current.Cost;

                    var coord = inputGraph.GetCoord(current.NodeId);
                    foreach (var neighbor in inputGraph.ConnectedNeighbors(coord.Row, coord.Col))
                    {
                        if (!visited[neighbor])
                            queue.Enqueue(new PathNode(neighbor, current.Cost + 1));
                    }
                }
            }
        }
        return CalculatePercentage(maxPathLength, numVert);
    }

    public void executeCharacteristicsAnalysis() {
        string filePath = Path.Combine(DataPath, "analisis.csv");

        using (StreamWriter file = new StreamWriter(filePath, false, Encoding.UTF8)) 
        {
            StatComparison<int> Test = new StatComparison<int>(DataPath);
            Stopwatch stopwatch = new Stopwatch();
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("Algorithm,DeadEnds,Intersection,LongestPath,Directness,Twistiness\n");
            file.Write(sb.ToString());
            int rows = 30;
            int cols = 30;
            var algorithms = Enum.GetValues(typeof(MazeGenerator.Algorithm));
            for (int i = 0; i < 100; i++)
            {
                stopwatch.Restart();
                sb.Clear();
                foreach (MazeGenerator.Algorithm generationAlgorithm in algorithms) 
                {
                    graph = MazeGraph<int>.CreateNoWallsGraph4(rows, cols);
                    graph = executeAlgorithm(generationAlgorithm, graph);
                    sb.Append(generationAlgorithm + ",");
                    var mazeStats = Test.AnalyzeMaze(graph);
                    sb.Append(mazeStats.DeadEnds.ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(mazeStats.Intersections.ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(Test.LongestPath(graph).ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(mazeStats.Directness.ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(mazeStats.Twistiness.ToString("00.000", CultureInfo.InvariantCulture) + "\n");
                } 
                file.Write(sb.ToString());
                stopwatch.Stop();
                Debug.Log($"Iteracion {i} , Tiempo(s) : {stopwatch.Elapsed.TotalSeconds:F4}");
            }
        }
    }

    public void TimeComparison() 
    {
        string filePath = Path.Combine(DataPath, "Tiempos.csv");
        using (StreamWriter file = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            var algorithms = (MazeGenerator.Algorithm[])Enum.GetValues(typeof(MazeGenerator.Algorithm));
            StatComparison<int> Test = new StatComparison<int>(DataPath);
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("Algorithm,Size,Time(ms)\n");
            file.Write(sb.ToString());
            int size = 50;
            int maxsize = 100;
            int inc = 10;
            Stopwatch stopwatch = new Stopwatch();

            for (int i = size; i < maxsize; i += inc)
            {
                string sizeString = $"{i}x{i}";
                foreach (MazeGenerator.Algorithm generationAlgorithm in algorithms)
                {
                    sb.Clear();
                    graph = MazeGraph<int>.CreateNoWallsGraph4(size, size);
                    stopwatch.Restart();
                    executeAlgorithm(generationAlgorithm, graph);
                    stopwatch.Stop();

                    sb.Append(generationAlgorithm).Append(",")
                        .Append(sizeString).Append(",")
                        .Append(stopwatch.Elapsed.TotalMilliseconds.ToString("00.000", CultureInfo.InvariantCulture))
                        .Append("\n");
                    file.Write(sb.ToString());
                }
            }
        }
    }
}