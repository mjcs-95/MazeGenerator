using System;
using System.Collections.Generic;
using UnityEngine;

static public class MazeTester<T> where T : IComparable<T> {
    
    private static readonly (int rowOffset, int colOffset)[] Directions =
    {
        (0, 1),  // East
        (1, 0),  // North
        (-1, 0), // South
        (0, -1)  // West
    };

    /// <summary>
    /// Verifies that every internal cell is correctly connected to its structural grid neighbors,
    /// while safely treating external maze outer boundaries as automatically connected.
    /// </summary>
    static public bool VerifyNoWallsMaze(MazeGraph<T> graph) 
    {
        for (int r = 0; r < graph.Rows; ++r)
        {
            for (int c = 0; c < graph.Cols; ++c)
            {
                var currentNode = graph.GetNode(r, c);

                foreach (var (dr, dc) in Directions)
                {
                    int neighborRow = r + dr;
                    int neighborCol = c + dc;

                    if (neighborRow < 0 || neighborRow >= graph.Rows || neighborCol < 0 || neighborCol >= graph.Cols)
                        continue;

                    var neighborNode = graph.GetNode(neighborRow, neighborCol);
                    if (!graph.HasEdge(currentNode, neighborNode))
                    {
                        Debug.LogWarning($"Connectivity gap found at cell ({r}, {c}) looking toward ({neighborRow}, {neighborCol})");
                        return false;
                    }
                }
            }
        }
        return true;
    }


    static bool HasCycleDFS(int current, int parent, bool[] visited, MazeGraph<T> graph) 
    {
        visited[current] =true;
        var coord = graph.GetCoord(current);
        
        foreach (int neighbor in graph.ConnectedNeighbors(coord.Row, coord.Col))
        {
            if (!visited[neighbor])
            {
                if (HasCycleDFS(neighbor, current, visited, graph))
                    return true;
            }
            else if (neighbor != parent)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Validates that the generated maze is a true mathematical tree structure 
    /// (it contains absolutely zero loops/cycles AND is fully connected).
    /// </summary>
    static public bool TestGraphIsTree(MazeGraph<T> graph) 
    {
        if (graph.NumVert == 0) 
            return true;

        bool[] visited = new bool[graph.NumVert];

        if (HasCycleDFS(0, -1, visited, graph))
        {
            Debug.LogWarning("Maze validation failed: Graph contains a loop/cycle.");
            return false;
        }
        for (int i = 0; i < visited.Length; ++i)
        {
            if (!visited[i])
            {
                Debug.LogWarning($"Maze validation failed: Graph is disconnected. Node {i} is unreachable.");
                return false;
            }
        }
        return true;
    }

}
