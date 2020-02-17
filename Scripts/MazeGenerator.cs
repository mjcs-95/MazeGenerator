using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    public int rows;
    public int cols;
    public GameObject prefabFloor;
    public GameObject prefabWall;
    public Material wallmaterial;

    static private MazeGraph<int> G;
    private GameObject[] mazeCells;



    public enum Algorithm { AldousBroder, BinaryTree, Ellers, HuntAndKill,Kruskall, Prim, RecursiveDivision, Sidewinder, Wilson }
    public Algorithm generationAlgorithm;


    public void createGraph() {
        int n = rows * cols;
        mazeCells = new GameObject[n];

        /*Build the Full Graph*/
        G = new MazeGraph<int>(n,rows, cols);
        int aux = 0;
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j, ++aux) {
                int cost = UnityEngine.Random.Range(1, 10);
                //Random values to use with prim and kruskal but it could be 1; Still not undirected
                if (j < cols - 1) {
                    G.addEdge(aux, aux + 1,     cost); //Random.Range(1, n)
                    G.addEdge(aux + 1, aux,     cost);
                }// North
                if (i > 0) {
                    G.addEdge(aux, aux - cols, cost); // west
                    G.addEdge(aux - cols, aux, cost); // west
                }
                if (j > 0) {
                    G.addEdge(aux, aux - 1,     cost); // South
                    G.addEdge(aux - 1, aux,     cost); // South
                }
                if (i < rows - 1) {
                    G.addEdge(aux, aux + cols, cost); // East
                    G.addEdge(aux + cols, aux, cost); // East
                }
            }
        }
    }


    public void createOBJ() {
        ExportToOBj<int> exporter = new ExportToOBj<int>();
        exporter.GenerateObj(G);
    }


    public void Generate3dMaze() {
        /*Maze creator*/
        int aux = 0;
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j, ++aux) {
                GameObject cell = Instantiate(prefabFloor, new Vector3(i, 0, j), new Quaternion());
                cell.name = string.Format("cell_{0}_{1}_{2}", i, j, aux);
                cell.transform.SetParent(this.transform);
                float offset_position = (cell.transform.localScale.x - prefabFloor.transform.localScale.y) / 2;
                float offset_cols = (cell.transform.localScale.x + prefabFloor.transform.localScale.y) / 2;
                //North Wall
                if (j == cols - 1 || !(G.hasEdge(aux + 1, aux) && G.hasEdge(aux, aux + 1))) {
                    Vector3 position = new Vector3(i, offset_cols, j + offset_position);
                    GameObject wall = Instantiate(prefabFloor, position, new Quaternion());
                    wall.transform.rotation = Quaternion.Euler(90, 0, 0);
                    wall.name = "NorthWall";
                    MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
                    meshRenderer.material = wallmaterial;
                    wall.transform.SetParent(cell.transform);
                }
                //West Wall
                if (i == 0 || !(G.hasEdge(aux - cols, aux) && G.hasEdge(aux, aux - cols))) {
                    Vector3 position = new Vector3(i - offset_position, offset_cols, j);
                    GameObject wall = Instantiate(prefabFloor, position, new Quaternion());
                    wall.transform.rotation = Quaternion.Euler(90, 90, 0);
                    wall.name = "LeftWall";
                    MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
                    meshRenderer.material = wallmaterial;
                    wall.transform.SetParent(cell.transform);
                }
                //South Wall
                if (j == 0 || !(G.hasEdge(aux - 1, aux) && G.hasEdge(aux, aux - 1))) {
                    Vector3 position = new Vector3(i, offset_cols, j - offset_position);
                    GameObject wall = Instantiate(prefabFloor, position, new Quaternion());
                    wall.transform.rotation = Quaternion.Euler(90, 180, 0);
                    wall.name = "SouthWall";
                    MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
                    meshRenderer.material = wallmaterial;
                    wall.transform.SetParent(cell.transform);
                }
                //East Wall
                if (i == rows - 1 || !(G.hasEdge(aux + cols, aux) && G.hasEdge(aux, aux + cols))) {
                    Vector3 position = new Vector3(i + offset_position, offset_cols, j);
                    GameObject wall = Instantiate(prefabFloor, position, new Quaternion());
                    wall.transform.rotation = Quaternion.Euler(90, 270, 0);
                    wall.name = "EasthWall";
                    MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
                    meshRenderer.material = wallmaterial;
                    wall.transform.SetParent(cell.transform);
                }
            }
        }
    }



    // Start is called before the first frame update
    void Start() {
        tests();
    }

    // Update is called once per frame
    void Update() {
    }

    public void DestroyMaze() {
        while (transform.childCount > 0) {
            if (Application.isEditor) {
                DestroyImmediate(transform.GetChild(0).gameObject);
            } else {
                Destroy(transform.GetChild(0).gameObject);
            }
        }
    }

    public void executeAlgorithm() {
        switch (generationAlgorithm) {
            case Algorithm.AldousBroder:
                G = Algorithms.AldousBroder<int>.Execute(G);
                break;
            case Algorithm.BinaryTree:
                G = Algorithms.BinaryTree<int>.Execute(G);
                break;
            case Algorithm.Ellers:
                G = Algorithms.Ellers<int>.Execute(G);
                break;
            case Algorithm.HuntAndKill:
                G = Algorithms.HuntAndKill<int>.Execute(G);
                break;
            case Algorithm.Kruskall:
                G = Algorithms.Kruskall<int>.Execute(G);
                break;
            case Algorithm.Prim:
                G = Algorithms.Prim<int>.Execute(G);
                break;
            case Algorithm.RecursiveDivision:
                G = Algorithms.RecursiveDivision<int>.Execute(G);
                break;
            case Algorithm.Sidewinder:
                G = Algorithms.Sidewinder<int>.Execute(G);
                break;
            case Algorithm.Wilson:
                G = Algorithms.Wilson<int>.Execute(G);
                break;
        }
    }

    public void GenerateMaze() {
        DestroyMaze();
        createGraph();
        executeAlgorithm();
        Generate3dMaze();
    }


    public void executeAnalysis() {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Application.dataPath + "/analisis.csv")) {
            StatComparison<int> Test = new StatComparison<int>();
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("Algorithm,DeadEnds,Intersection,LongestPath,Directness,Twistiness\n");
            file.Write(sb.ToString());
            for (int i = 0; i < 100; i++) {
                TimeSpan stop;
                TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
                // codigo a medir
                foreach (Algorithm it in System.Enum.GetValues(typeof(Algorithm))) {
                    sb.Clear();
                    createGraph();
                    generationAlgorithm = it;
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
                Debug.Log("Iteracion "+i+" , Tiempo(s) : "+stop.Subtract(start).TotalMilliseconds/1000.0f);
            }
        }

    }

    public void tests() {
        DestroyMaze();
    }
}
