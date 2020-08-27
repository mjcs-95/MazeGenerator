using UnityEditor;
using UnityEngine;

public class MazeGenerator : MonoBehaviour 
{
    public int rows;
    public int cols;

    public enum Algorithm { AldousBroder, BinaryTree, Ellers, HuntAndKill,Kruskall, Prim, RecursiveDivision, Sidewinder, Wilson }
    public Algorithm generationAlgorithm;

    public void createOBJ() 
    {
        ExportToOBj<int> exporter = new ExportToOBj<int>();
        exporter.GenerateObj(G);
    }


    public Material mat;// = new Material(Shader.Find("Standard"));


    static private MazeGraph<int> G;
    private GameObject[] mazeCells;
    StatComparison<int> Test;


    public void Generate3dMaze() 
    {
        //Mesh model = Instantiate(Resources.Load("objeto1"), transform) as Mesh;        
        GameObject model = Instantiate(Resources.Load("objeto1"), transform) as GameObject;
        model.GetComponentInChildren<MeshRenderer>().material = mat;                
    }

    // Start is called before the first frame update
    void Start() 
    {
        GenerateMaze();
    }

    // Update is called once per frame
    void Update() {}

    public void DestroyMaze() 
    {
        while (transform.childCount > 0) 
            if (Application.isEditor)
                DestroyImmediate(transform.GetChild(0).gameObject);
            else
                Destroy(transform.GetChild(0).gameObject);
    }

    public void executeAlgorithm() 
    {
        switch (generationAlgorithm) 
        {
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

    public void GenerateMaze() 
    {
        DestroyMaze();
        G = MazeGraph<int>.CreateNoWallsGraph4(rows,cols);
        executeAlgorithm();
        createOBJ();
        AssetDatabase.Refresh();
        Generate3dMaze();
    }

    public void executeTimeAnalysis() 
    {        
        Test = new StatComparison<int>();
        Test.TimeComparison();
    }

    public void executeAnalysis() 
    {
        Test = new StatComparison<int>();
        Test.executeCharacteristicsAnalysis();
    }

}
