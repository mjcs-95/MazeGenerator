using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public class StatComparison<T> where T : IComparable<T> 
{
    MazeGraph<int> G;
    public MazeGenerator.Algorithm generationAlgorithm;


    public StatComparison() {}

    //Deadends
    private int deadEnds(MazeGraph<T> g)
    {
        int deadends = 0;
        for (int i = 0; i < g.rows; ++i)
            for (int j = 0; j < g.cols - 1; ++j)
                if (g.ConnectedNeighbors(i, j).Count == 1)
                    ++deadends;
        return deadends;
    }

    public float deadendsPercentage(MazeGraph<T> g) 
    { 
        return 100.0f * deadEnds(g) / (g.rows * g.cols); 
    }

    //interSections
    private int interSections(MazeGraph<T> g) 
    {
        int intersections = 0;
        for (int i = 0; i < g.rows; ++i)
            for (int j = 0; j < g.cols - 1; ++j)
                if (2 < g.ConnectedNeighbors(i, j).Count)
                    ++intersections;
        return intersections;
    }

    public float interSectionsPercentage(MazeGraph<T> g) 
    {
        return 100.0f * interSections(g) / (g.rows * g.cols);
    }

    //LongestPath
    public float LongestPath(MazeGraph<T> G) 
    {
        bool[] visited;
        KeyValuePair<KeyValuePair<int, int>, int> LP = new KeyValuePair<KeyValuePair<int, int>, int>();
        List<KeyValuePair<int, int>> adycost = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < G.rows; ++i)
            for (int j = 0; j < G.rows; ++j) {
                visited = new bool[G.NumVert];
                visited[G.GetNode(i, j)] = true;
                foreach (var n in G.ConnectedNeighbors(i, j))
                    adycost.Add(new KeyValuePair<int, int>(n, 1));
                while (adycost.Count != 0) 
                {
                    KeyValuePair<int, int> c = adycost[0];
                    visited[c.Key] = true;
                    if (LP.Value < c.Value)
                        LP = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(G.GetNode(i, j), c.Key), c.Value);
                    foreach (var n in G.ConnectedNeighbors(G.GetCoord(c.Key)[0], G.GetCoord(c.Key)[1]))
                        if (!visited[n])
                            adycost.Add(new KeyValuePair<int, int>(n, c.Value + 1));
                    adycost.RemoveAt(0);
                }
            }
        return 100.0f * LP.Value / (G.rows * G.cols);
    }


    //Directness
    public float Directness(MazeGraph<T> g) {
        int direct = 0;
        for (int i = 0; i < g.rows; ++i) 
            for (int j = 0; j < g.rows; ++j) 
            {
                bool n = g.hasEdge(g.GetNode(i, j), g.GetNode(i + 1, j));
                bool s = g.hasEdge(g.GetNode(i, j), g.GetNode(i - 1, j));
                bool e = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j + 1));
                bool w = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j - 1));
                if ( ((n && s) && !w && !e) || ((e && w) && !n && !s))
                    ++direct;
            }
        return 100.0f * direct / (g.rows * g.cols);
    }

    //Twistiness
    public float Twistiness(MazeGraph<T> g) 
    {
        int twists = 0;
        for (int i = 0; i < g.rows; ++i)
            for (int j = 0; j < g.rows; ++j) {
                bool n = g.hasEdge(g.GetNode(i, j), g.GetNode(i + 1, j));
                bool s = g.hasEdge(g.GetNode(i, j), g.GetNode(i - 1, j));
                bool e = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j + 1));
                bool w = g.hasEdge(g.GetNode(i, j), g.GetNode(i, j - 1));
                if ((n & !s & (e || w)) || (!n & s & (e || w)) || (!w & e & (n || s)) || (w & !e & (n || s)) )
                    ++twists;
            }
        return 100.0f * twists / (g.rows * g.cols);
    }

    public void executeCharacteristicsAnalysis() {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Application.dataPath + "/analisis.csv")) 
        {
            StatComparison<int> Test = new StatComparison<int>();
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("Algorithm,DeadEnds,Intersection,LongestPath,Directness,Twistiness\n");
            file.Write(sb.ToString());
            int rows = 10;
            int cols = 10;
            for (int i = 0; i < 100; i++) 
            {
                TimeSpan stop;
                TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
                // codigo a medir
                foreach (MazeGenerator.Algorithm it in Enum.GetValues(typeof(MazeGenerator.Algorithm))) 
                {
                    sb.Clear();
                    G = MazeGraph<int>.CreateNoWallsGraph4(rows, cols);
                    var generationAlgorithm = it;
                    executeAlgorithm();
                    sb.Append(it + ",");
                    sb.Append(Test.deadendsPercentage(G).ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(Test.interSectionsPercentage(G).ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(Test.LongestPath(G).ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(Test.Directness(G).ToString("00.000", CultureInfo.InvariantCulture) + ",");
                    sb.Append(Test.Twistiness(G).ToString("00.000", CultureInfo.InvariantCulture) + "\n");
                    file.Write(sb.ToString());

                }
                stop = new TimeSpan(DateTime.Now.Ticks);
                Debug.Log("Iteracion " + i + " , Tiempo(s) : " + stop.Subtract(start).TotalMilliseconds / 1000.0f);
            }
        }
    }

    public void TimeComparison() 
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Application.dataPath + "/Tiempos.csv")) 
        {
            StatComparison<int> Test = new StatComparison<int>();
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("Algorithm,Size,Time(ms)\n");
            file.Write(sb.ToString());
            int size = 50;
            int maxsize = 100;
            int inc = 10;
            //int rows = size;
            //int cols = size;
            TimeSpan stop;
            TimeSpan start;
            while (size <= maxsize) 
            {
                G = MazeGraph<int>.CreateNoWallsGraph4(size, size);
                for (int i = 0; i < 10; i++)
                    foreach (MazeGenerator.Algorithm it in Enum.GetValues(typeof(MazeGenerator.Algorithm))) 
                    {
                        sb.Clear();                        
                        generationAlgorithm = it;

                        start = new TimeSpan(DateTime.Now.Ticks);
                        executeAlgorithm();
                        stop = new TimeSpan(DateTime.Now.Ticks);

                        sb.Append(it + "," + (size+"x"+size) + "," + stop.Subtract(start).TotalMilliseconds.ToString("00.000", CultureInfo.InvariantCulture) + "\n");
                        file.Write(sb.ToString());
                    }
                size = size + inc;
            }
        }

    }



    public void executeAlgorithm() 
    {        
        switch (generationAlgorithm) 
        {
            case MazeGenerator.Algorithm.AldousBroder:
                //G = 
                Algorithms.AldousBroder<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.BinaryTree:
                //G = 
                Algorithms.BinaryTree<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.Ellers:
                //G = 
                Algorithms.Ellers<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.HuntAndKill:
                //G = 
                Algorithms.HuntAndKill<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.Kruskall:
                //G = 
                Algorithms.Kruskall<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.Prim:
                //G = 
                Algorithms.Prim<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.RecursiveDivision:
                //G = 
                Algorithms.RecursiveDivision<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.Sidewinder:
                //G = 
                Algorithms.Sidewinder<int>.Execute(G);
                break;
            case MazeGenerator.Algorithm.Wilson:
                //G = 
                Algorithms.Wilson<int>.Execute(G);
                break;           
        }
    }

}
