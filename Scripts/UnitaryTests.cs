using System;
using System.Collections.Generic;

static public class UnitaryTests<T> where T : IComparable<T> {
    static bool hasEastconnection(int i, int j, MazeGraph<T> G) {
        if (j == G.cols - 1) {
            return true;
        } else {
            return G.hasEdge(G.GetNode(i, j), G.GetNode(i, j + 1));
        }
    }

    static bool hasNorthconnection(int i, int j, MazeGraph<T> G) {
        if (i == G.rows-1) {
            return true;
        } else {
            return G.hasEdge(G.GetNode(i, j), G.GetNode(i + 1, j));
        }
    }

    static bool hasSouthconnection(int i, int j, MazeGraph<T> G) {
        if (i == 0) {
            return true;
        } else {
            return G.hasEdge(G.GetNode(i, j), G.GetNode(i-1, j));
        }
    }

    static bool hasWestconnection(int i, int j, MazeGraph<T> G) {
        if (j == 0) {
            return true;
        } else {
            return G.hasEdge(G.GetNode(i, j), G.GetNode(i, j - 1));
        }
    }

    static public bool TestConnectedGridGraph(MazeGraph<T> G) {
        for (int i = 0; i < G.rows; ++i) {
            for (int j = 0; j < G.cols; ++j) {
                if (!(  hasEastconnection(i, j, G) &&
                        hasNorthconnection(i, j, G) &&
                        hasSouthconnection(i, j, G) &&
                        hasWestconnection(i, j, G) )
                ){
                    return false;
                }
            }
        }
                return true;
    }


    static bool IsCycle(int current, bool[] visited, int parent, MazeGraph<T> G) {
        visited[current] =true;
        List<int> coord = G.GetCoord(current);
        foreach (int i in G.ConnectedNeighbors(coord[0], coord[1])) {
            if (visited[i]) {
                if (IsCycle(i, visited, current, G)) {
                    return true;
                } else if (i != parent) {
                    return true;
                }
            }            
        }
        return false;
    }


    static public bool TestGraphIsTree(MazeGraph<T> G) {
        bool[] visited = new bool[G.numVert()];
        if (IsCycle(0, visited, -1, G)) {
            return false;
        }
        for(int i = 0; i < visited.Length; ++i) {
            if (!visited[i]) {
                return true;
            }
        }
        return true;
    }


}
