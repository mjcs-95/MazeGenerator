using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

public class ExportToOBj<T> where T : IComparable<T>
{
    private enum Face { Floor, North, South, East, West, Ceiling }
    private readonly string outputPath;
    private readonly StringBuilder sb;

    public ExportToOBj(string path)
    {
        outputPath = Path.Combine(path, "Resources", "objeto1.obj");
        sb = new StringBuilder();
    }

    public void GenerateObj(MazeGraph<T> graph, bool ceil = false)
    {
        if(graph is null)
            throw new ArgumentNullException(nameof(graph));

        string directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using (StreamWriter file = new StreamWriter(outputPath))
        {
            for (int i = 0; i < graph.Rows; ++i)
            {
                for (int j = 0; j < graph.Cols; ++j)
                {
                    sb.Clear();
                    Vector3[] vertices = new Vector3[8]{
                        new(i,     j,     0),
                        new(i,     j + 1, 0),
                        new(i + 1, j,     0),
                        new(i + 1, j + 1, 0),
                        new(i,     j,     1),
                        new(i,     j + 1, 1),
                        new(i + 1, j,     1),
                        new(i + 1, j + 1, 1)
                    };

                    var currentNode = graph.GetNode(i, j);

                    WriteWall(vertices, Face.Floor);
                    if (i == graph.Rows - 1 || !graph.HasEdge(currentNode, graph.GetNode(i + 1, j)))
                        WriteWall(vertices, Face.North);
                    
                    if (i == 0              || !graph.HasEdge(currentNode, graph.GetNode(i - 1, j)))
                        WriteWall(vertices, Face.South);
                    
                    if (j == graph.Cols - 1 || !graph.HasEdge(currentNode, graph.GetNode(i, j + 1)))
                        WriteWall(vertices, Face.East);
                    
                    if (j == 0              || !graph.HasEdge(currentNode, graph.GetNode(i, j - 1)))
                        WriteWall(vertices, Face.West);
                    
                    if (ceil)
                        WriteWall(vertices, Face.Ceiling);

                    file.Write(sb.ToString());
                }
            }
        }
    }

    private static Vector3 CalculateTriangleNormal(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex)
    {
        Vector3 firstEdge = secondVertex - firstVertex;
        Vector3 secondEdge = thirdVertex - firstVertex;
        Vector3 normal = Vector3.Cross(firstEdge, secondEdge);

        if (normal.LengthSquared() <= 1e-12f)
            throw new ArgumentException("Degenerated triangle.");

        return Vector3.Normalize(normal);
    }

    private void AppendNormalToObj(Vector3 normal) => sb.AppendFormat(CultureInfo.InvariantCulture, "vn {0} {1} {2}\n", normal.X, normal.Z, normal.Y);

    private const string v_String = "v {0} {2} {1} \n";
    private const string vtString = "vt {0} {1}  \n";
    private void AppendVertexAndUV(Vector3 vertex, Face face)
    {
        sb.AppendFormat(CultureInfo.InvariantCulture, v_String, vertex.X, vertex.Y, vertex.Z);

        if (face == Face.Floor || face == Face.Ceiling)
            sb.AppendFormat(CultureInfo.InvariantCulture, vtString, vertex.X, vertex.Y);
        else if (face == Face.North || face == Face.South)
            sb.AppendFormat(CultureInfo.InvariantCulture, vtString, vertex.Y, vertex.Z);
        else
            sb.AppendFormat(CultureInfo.InvariantCulture, vtString, vertex.X, vertex.Z);
    }

    private const string ObjFrontFirstTriangle = "f -4/-4/-1 -3/-3/-1 -2/-2/-1  \n";
    private const string ObjFrontSecondTriangle = "f -3/-3/-1 -1/-1/-1 -2/-2/-1  \n";
    private const string ObjBackFirstTriangle = "f -4/-4/-1 -2/-2/-1 -3/-3/-1  \n";
    private const string ObjBackSecondTriangle = "f -3/-3/-1 -2/-2/-1 -1/-1/-1  \n";

    private static readonly byte[,] FaceVertexIndices =
    {
        { 0, 1, 2, 3 },     //0 - Floor
        { 2, 3, 6, 7 },     //1 - North
        { 0, 1, 4, 5 },     //2 - South
        { 1, 3, 5, 7 },     //3 - East
        { 0, 2, 4, 6 },     //4 - West
        { 4, 5, 6, 7 }      //5 - Ceil
    };

    private void WriteWall(Vector3[] vertices, Face face)
    {
        int f = (int)face;

        var v0 = vertices[FaceVertexIndices[f, 0]];
        var v1 = vertices[FaceVertexIndices[f, 1]];
        var v2 = vertices[FaceVertexIndices[f, 2]];
        var v3 = vertices[FaceVertexIndices[f, 3]];

        AppendVertexAndUV(v0, face);
        AppendVertexAndUV(v1, face);
        AppendVertexAndUV(v2, face);
        AppendVertexAndUV(v3, face);

        Vector3 n = CalculateTriangleNormal(v0, v1, v2);
        AppendNormalToObj(n);  
        sb.Append(ObjFrontFirstTriangle);
        AppendNormalToObj(-n); 
        sb.Append(ObjBackFirstTriangle);

        n = CalculateTriangleNormal(v1, v3, v2);
        AppendNormalToObj(n);  
        sb.Append(ObjFrontSecondTriangle);
        AppendNormalToObj(-n); 
        sb.Append(ObjBackSecondTriangle);
    }
}