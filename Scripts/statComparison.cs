using System;
using System.Collections.Generic;

public class StatComparison<T> where T : IComparable<T> 
{
    public StatComparison() {
    }

    //Deadends
    private int deadEnds(MazeGraph<T> g){
        int deadends = 0;
        for (int i = 0; i < g.rows; ++i) {
            for (int j = 0; j < g.cols - 1; ++j) {
                if (g.ConectedNeighbors(i, j).Count == 1) {
                    ++deadends;
                }
            }
        }        
        return deadends;
    }

    public float deadendsPercentage(MazeGraph<T> g) {
        return 100.0f * deadEnds(g) / (g.rows * g.cols);
    }

    //interSections
    private int interSections(MazeGraph<T> g) {
        int intersections = 0;
        for (int i = 0; i < g.rows; ++i) {
            for (int j = 0; j < g.cols - 1; ++j) {
                if (2 < g.ConectedNeighbors(i, j).Count) {
                    ++intersections;
                }
            }
        }
        return intersections;
    }

    public float interSectionsPercentage(MazeGraph<T> g) {
        return 100.0f * interSections(g) / (g.rows * g.cols);
    }

    //LongestPath
    public float LongestPath(MazeGraph<T> G) {
        bool[] visited;
        KeyValuePair<KeyValuePair<int, int>, int> LP = new KeyValuePair<KeyValuePair<int, int>, int>();
        List<KeyValuePair<int, int>> adycost = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < G.rows; ++i) {
            for (int j = 0; j < G.rows; ++j) {
                visited = new bool[G.numVert()];
                visited[G.GetNode(i, j)] = true;
                foreach (var n in G.ConectedNeighbors(i, j)) {
                    adycost.Add(new KeyValuePair<int, int>(n, 1));
                }
                while (adycost.Count != 0) {
                    KeyValuePair<int, int> c = adycost[0];
                    visited[c.Key] = true;
                    if (LP.Value < c.Value) {
                        LP = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(G.GetNode(i, j), c.Key), c.Value);
                    }
                    foreach (var n in G.ConectedNeighbors(G.GetCoord(c.Key)[0], G.GetCoord(c.Key)[1])) {
                        if (!visited[n]) {
                            adycost.Add(new KeyValuePair<int, int>(n, c.Value + 1));
                        }
                    }
                    adycost.RemoveAt(0);
                }
            }
        }
        return 100.0f * LP.Value / (G.rows * G.cols);
    }


    //Directness
    public float Directness(MazeGraph<T> g) {
        int direct = 0;
        for (int i = 0; i < g.rows; ++i) {
            for (int j = 0; j < g.rows; ++j) {
                bool n = g.hasEdge(g.GetNode(i, j), g.GetNode(i + 1, j));
                bool s = g.hasEdge(g.GetNode(i, j), g.GetNode(i - 1, j));
                bool e = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j + 1));
                bool w = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j - 1));
                if ( ((n && s) && !w && !e) || ((e && w) && !n && !s))  {
                    ++direct;
                }
            }
        }
        return 100.0f * direct / (g.rows * g.cols);
    }

    //Twistiness
    public float Twistiness(MazeGraph<T> g) {
        int twists = 0;
        for (int i = 0; i < g.rows; ++i) {
            for (int j = 0; j < g.rows; ++j) {
                bool n = g.hasEdge(g.GetNode(i, j), g.GetNode(i + 1, j));
                bool s = g.hasEdge(g.GetNode(i, j), g.GetNode(i - 1, j));
                bool e = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j + 1));
                bool w = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j - 1));
                if ((n & !s & (e || w)) || (!n & s & (e || w)) ||
                    (!w & e & (n || s)) || (w & !e & (n || s)) ){
                    ++twists;
                }
            }
        }
        return 100.0f * twists / (g.rows * g.cols);
    }
}
