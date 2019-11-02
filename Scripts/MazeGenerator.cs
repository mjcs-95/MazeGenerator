using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    public int width;
    public int height;
    public GameObject prefabFloor;
    public GameObject prefabWall;
    public Material wallmaterial;

    private MazeGraph<int> G;
    private GameObject[] mazeCells;



    public enum Algorithm { AldousBroder, BinaryTree, HuntAndKill,Kruskall, Prim, RecursiveDivision, Sidewinder, Wilson }
    public Algorithm generationAlgorithm;


    public void createGraph() {
        int n = width * height;
        mazeCells = new GameObject[n];

        /*Build the Full Graph*/
        G = new MazeGraph<int>(n,width, height);
        int aux = 0;
        for (int i = 0; i < width; ++i) {
            for (int j = 0; j < height; ++j, ++aux) {
                int cost = Random.Range(1, 10);
                //Random values to use with prim and kruskal but it could be 1; Still not undirected
                if (j < height - 1) {
                    G.addEdge(aux, aux + 1,     cost); //Random.Range(1, n)
                    G.addEdge(aux + 1, aux,     cost);
                }// North
                if (i > 0) {
                    G.addEdge(aux, aux - height, cost); // west
                    G.addEdge(aux - height, aux, cost); // west
                }
                if (j > 0) {
                    G.addEdge(aux, aux - 1,     cost); // South
                    G.addEdge(aux - 1, aux,     cost); // South
                }
                if (i < width - 1) {
                    G.addEdge(aux, aux + height, cost); // East
                    G.addEdge(aux + height, aux, cost); // East
                }
            }
        }
    }


    public void createMaze() {
        /*Maze creator*/
        int aux = 0;
        for (int i = 0; i < width; ++i) {
            for (int j = 0; j < height; ++j, ++aux) {
                GameObject cell = Instantiate(prefabFloor, new Vector3(i, 0, j), new Quaternion());
                cell.name = "cell_" + i + "_" + j + "_" + aux;
                cell.transform.SetParent(this.transform);
                float offset_position = (cell.transform.localScale.x - prefabFloor.transform.localScale.y) / 2;
                float offset_height = (cell.transform.localScale.x + prefabFloor.transform.localScale.y) / 2;
                //North Wall
                if (j == height - 1 || !(G.hasEdge(aux + 1, aux) && G.hasEdge(aux, aux + 1)) ) {
                    Vector3 position = new Vector3(i, offset_height, j + offset_position);
                    GameObject wall = Instantiate(prefabFloor, position, new Quaternion());
                    wall.transform.rotation = Quaternion.Euler(90, 0, 0);
                    wall.name = "NorthWall";
                    MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
                    meshRenderer.material = wallmaterial;
                    wall.transform.SetParent(cell.transform);
                }
                //West Wall
                if (i == 0 || !(G.hasEdge(aux - height, aux) && G.hasEdge(aux, aux - height))) {
                    Vector3 position = new Vector3(i - offset_position, offset_height, j);
                    GameObject wall = Instantiate(prefabFloor, position, new Quaternion());
                    wall.transform.rotation = Quaternion.Euler(90, 90, 0);
                    wall.name = "LeftWall";
                    MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
                    meshRenderer.material = wallmaterial;
                    wall.transform.SetParent(cell.transform);
                }
                //South Wall
                if (j == 0 || !(G.hasEdge(aux - 1, aux) && G.hasEdge(aux, aux - 1))) {
                    Vector3 position = new Vector3(i, offset_height, j - offset_position);
                    GameObject wall = Instantiate(prefabFloor, position, new Quaternion());
                    wall.transform.rotation = Quaternion.Euler(90, 180, 0);
                    wall.name = "SouthWall";
                    MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
                    meshRenderer.material = wallmaterial;
                    wall.transform.SetParent(cell.transform);
                }
                //East Wall
                if (i == width - 1 || !(G.hasEdge(aux + height, aux) && G.hasEdge(aux, aux + height)) ) {
                    Vector3 position = new Vector3(i + offset_position, offset_height, j);
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

    public void tests() {      
        while (transform.childCount > 0) {
            if (Application.isEditor) {
                DestroyImmediate(transform.GetChild(0).gameObject);
            } else {
                Destroy(transform.GetChild(0).gameObject);
            }
        }
        createGraph();
        switch (generationAlgorithm) {
            case Algorithm.AldousBroder:
                G = Algorithms.AldousBroder<int>.Execute(G);
                break;
            case Algorithm.BinaryTree:
                G = Algorithms.BinaryTree<int>.Execute(G);
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
        createMaze();
    }
}
