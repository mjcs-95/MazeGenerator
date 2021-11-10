using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(NavMeshSurface))]
public class MazeCreator : MonoBehaviour
{
    /*Public Variables*/
    public int rows;
    public int cols;
    public Algorithms.AlgorithmEnum generationAlgorithm;
    public GameObject prefabFloor;
    public GameObject prefabWall;
    public Material mazeMaterial;
    public Vector3 mazePosition;
    public Vector2 XYMazeScale;
    public Color gradientColor;

    private Vector3 mazeScale;    
    private MazeGraph<int> G;
    private GameObject floor_;
    private GameObject wall_;
    private int[] mazeCosts;
    private int playerPosX;
    private int playerPosY;

    // Start is called before the first frame update
    private void Awake() 
    {
        GenerateMaze(); 
    }

    private void GenerateMaze()
    {
        mazeScale = new Vector3(XYMazeScale.x, XYMazeScale.y, XYMazeScale.x);
        G = MazeGraph<int>.CreateNoWallsGraph4(rows, cols);
        executeAlgorithm();                
        playerPosY = Random.Range(1, G.rows);
        playerPosX = Random.Range(1, G.cols);
        mazeCosts = Algorithms.Costs<int>.Dijkstra(G,G.GetNode(playerPosY,playerPosX));
        ScaledPrefabs();
        CreateMaze();
    }

    private void DestroyMaze() 
    {
        while (transform.childCount > 0) 
        {
            if (Application.isEditor) 
                DestroyImmediate(transform.GetChild(0).gameObject);
            else 
                Destroy(transform.GetChild(0).gameObject);
        }
        transform.GetComponentInParent<MeshFilter>().mesh = new Mesh();
    }

    private void executeAlgorithm() {
        switch (generationAlgorithm) 
        {
            case Algorithms.AlgorithmEnum.AldousBroder:
                G = Algorithms.AldousBroder<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.BinaryTree:
                G = Algorithms.BinaryTree<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.Ellers:
                G = Algorithms.Ellers<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.HuntAndKill:
                G = Algorithms.HuntAndKill<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.Kruskall:
                G = Algorithms.Kruskall<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.Prim:
                G = Algorithms.Prim<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.RecursiveDivision:
                G = Algorithms.RecursiveDivision<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.Sidewinder:
                G = Algorithms.Sidewinder<int>.Execute(G);
                break;
            case Algorithms.AlgorithmEnum.Wilson:
                G = Algorithms.Wilson<int>.Execute(G);
                break;
        }
    }

    private void ScaledPrefabs()
    {
        floor_ = Instantiate(prefabFloor,  mazePosition, new Quaternion());
        wall_ = Instantiate(prefabWall, mazePosition, new Quaternion());
        floor_.transform.localScale = Vector3.Scale(mazeScale, floor_.transform.localScale);
        wall_.transform.localScale = Vector3.Scale(mazeScale, wall_.transform.localScale);
    }    

    private void CreateMaze()
    {
        float offset_position = (floor_.transform.localScale.x - floor_.transform.localScale.y) / (2); //
        float offset_height = mazeScale.y * (floor_.transform.localScale.x + floor_.transform.localScale.y) / (4); //
        CombineInstance[] combine = new CombineInstance[G.NumVert];
        Mesh subMesh = floor_.GetComponent<MeshFilter>().sharedMesh;
        int aux = 0;        
        for (int i = 0; i < G.rows; ++i)
            for (int j = 0; j < G.cols; ++j, ++aux)
            {
                combine[aux].mesh = subMesh;
                floor_.transform.position = new Vector3(i * XYMazeScale.x, 0, j * XYMazeScale.x) + mazePosition;
                combine[aux].transform = floor_.transform.localToWorldMatrix;
                GenerateWalls(i, j, aux, offset_position, offset_height);
            }
        Destroy(floor_);
        Destroy(wall_);
        transform.GetComponentInParent<MeshFilter>().mesh = new Mesh();
        transform.GetComponentInParent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.GetComponentInParent<MeshFilter>().mesh.OptimizeIndexBuffers();
        transform.GetComponentInParent<MeshFilter>().mesh.OptimizeReorderVertexBuffer();
        transform.GetComponentInParent<MeshFilter>().mesh.Optimize();
        transform.GetComponentInParent<MeshRenderer>().material = mazeMaterial;
        transform.GetComponentInParent<MeshRenderer>().material.mainTextureScale = new Vector2(0.25f, 0.25f);

        transform.GetComponentInParent<NavMeshSurface>().BuildNavMesh();
        var Player = GameObject.FindGameObjectWithTag("Player");
        Player.transform.position = new Vector3(playerPosY*mazeScale.x, 0.4f, playerPosX * mazeScale.x) + mazePosition;
        var Enemy = GameObject.FindGameObjectWithTag("Enemy");
        Enemy.transform.position = new Vector3(Random.Range(1, G.rows), 0.2f, Random.Range(1, G.cols)) + mazePosition;
        Enemy.GetComponent<navmeshmove>().enabled = true;
        Debug.Log(Profiler.GetRuntimeMemorySizeLong(wall_) * 4*G.NumVert + "bytes");

    }

    private float DistFunction(int i, int j)
    {
        var c = mazeCosts[G.GetNode(i, j)] / (float) mazeCosts[G.NumVert];        
        return c;
    }

    private void GenerateWalls(int i, int j, int aux, float offset_position, float offset_height)
    {
        
        Color color = Color.Lerp(wall_.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor"), gradientColor, DistFunction(i, j));        
        if (!G.hasEdge(aux, aux + G.cols) || i == G.rows - 1)
        {
            GameObject wall = Instantiate(wall_);
            SetWallMembers(wall, new Vector3(XYMazeScale.x * i + offset_position, offset_height, XYMazeScale.x * j) + mazePosition, Quaternion.Euler(0, 0, 0), "NorthWall_" + i + "_" + j, transform);
            wall.GetComponent<MeshRenderer>().material = mazeMaterial;
            wall.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        }
        if (!G.hasEdge(aux, aux - G.cols) || i == 0)
        {
            GameObject wall = Instantiate(wall_);
            SetWallMembers(wall, new Vector3(XYMazeScale.x * i - offset_position, offset_height, XYMazeScale.x * j) + mazePosition, Quaternion.Euler(0, 180, 0), "SouthWall_" + i + "_" + j, transform);
            wall.GetComponent<MeshRenderer>().material = mazeMaterial;
            wall.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);

        }
        if (!G.hasEdge(aux, aux + 1) || j == G.cols - 1)
        {
            GameObject wall = Instantiate(wall_);
            SetWallMembers(wall, new Vector3(XYMazeScale.x * i, offset_height, XYMazeScale.x * j + offset_position) + mazePosition, Quaternion.Euler(0, 90, 0), "EastWall_" + i + "_" + j, transform);
            wall.GetComponent<MeshRenderer>().material = mazeMaterial;
            wall.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);

        }
        if (!G.hasEdge(aux, aux - 1) || j == 0)
        {
            GameObject wall = Instantiate(wall_);
            SetWallMembers(wall, new Vector3(XYMazeScale.x * i, offset_height, XYMazeScale.x * j - offset_position) + mazePosition, Quaternion.Euler(0, 270, 0), "WestWall_"+ i+"_"+j, transform);
            wall.GetComponent<MeshRenderer>().material = mazeMaterial;
            wall.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        }
    }

    private void SetWallMembers(GameObject wall, Vector3 pos, Quaternion rotation, string name, Transform parent, int layer = 8) 
    {
        wall.transform.position = pos;
        wall.transform.rotation = rotation;
        wall.name = name;
        wall.layer = layer;
        wall.tag = "Wall";
        wall.transform.SetParent(parent);        
    }
}
