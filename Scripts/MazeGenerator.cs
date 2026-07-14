using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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

    static public readonly Dictionary<Algorithm, Func<MazeGraph<int>, MazeGraph<int>>> AlgorithmMap = new()
    {
        { Algorithm.AldousBroder, Algorithms.AldousBroder<int>.Execute },
        { Algorithm.BinaryTree, Algorithms.BinaryTree<int>.Execute },
        { Algorithm.Ellers, Algorithms.Ellers<int>.Execute },
        { Algorithm.HuntAndKill, Algorithms.HuntAndKill<int>.Execute },
        { Algorithm.Kruskall, Algorithms.Kruskall<int>.Execute },
        { Algorithm.Prim, Algorithms.Prim<int>.Execute },
        { Algorithm.RecursiveDivision, Algorithms.RecursiveDivision<int>.Execute },
        { Algorithm.Sidewinder, Algorithms.Sidewinder<int>.Execute },
        { Algorithm.Wilson, Algorithms.Wilson<int>.Execute }
    };

    public void executeAlgorithm()
    {
        if (AlgorithmMap.TryGetValue(generationAlgorithm, out var executeFunc))
            G = executeFunc(G);
        else
            UnityEngine.Debug.LogError($"Algoritmo {generationAlgorithm} no está implementado en el diccionario.");
    }

    public void GenerateMaze() 
    {
        DestroyMaze();
        G = MazeGraph<int>.CreateNoWallsGraph4(rows,cols);
        Stopwatch stopwatch = Stopwatch.StartNew();
        executeAlgorithm();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Tiempo: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
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
