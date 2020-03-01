//using System.Numerics;
using System.Text;
using UnityEngine; //@Application

public class ExportToOBj<T> where T : System.IComparable<T> {
    // Start is called before the first frame update
    public enum Face { N, S, E, W, F, C }

    StringBuilder sb;
    System.Numerics.Vector3[] V;
    System.Numerics.Vector3 n;
    byte[,] pos;

    public ExportToOBj() {
        sb = new StringBuilder();
        V = new System.Numerics.Vector3[8];
        n = System.Numerics.Vector3.Zero;
        pos = new byte[6, 4] {
            { 0,1,2,3 },    //F
            { 2,3,6,7 },    //N
            { 0,1,4,5 },    //S
            { 1,3,5,7 },    //E
            { 0,2,4,6 },    //W
            { 4,5,6,7 }     //C
        };
    }

    public void GenerateObj(MazeGraph<T> G, bool ceil = false) {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Application.dataPath+"/Resources/objeto1.obj")) {
            for (int i = 0; i < G.rows; ++i) {
                for (int j = 0; j < G.cols; ++j) {
                    sb.Clear();
                    V = new System.Numerics.Vector3[]{
                        new System.Numerics.Vector3(i  ,j  , 0),
                        new System.Numerics.Vector3(i  ,j+1, 0),
                        new System.Numerics.Vector3(i+1,j  , 0),
                        new System.Numerics.Vector3(i+1,j+1, 0),
                        new System.Numerics.Vector3(i,  j  , 1),
                        new System.Numerics.Vector3(i,  j+1, 1),
                        new System.Numerics.Vector3(i+1,j  , 1),
                        new System.Numerics.Vector3(i+1,j+1, 1)
                    };
                    WriteWall(V, 0);
                    if (!G.hasEdge(G.GetNode(i, j), G.GetNode(i + 1, j)) || i == G.rows - 1 ) { WriteWall(V, 1); }
                    if (!G.hasEdge(G.GetNode(i, j), G.GetNode(i - 1, j)) || i == 0          ) { WriteWall(V, 2); }
                    if (!G.hasEdge(G.GetNode(i, j), G.GetNode(i, j + 1)) || j == G.cols - 1 ) { WriteWall(V, 3); }
                    if (!G.hasEdge(G.GetNode(i, j), G.GetNode(i, j - 1)) || j == 0          ) { WriteWall(V, 4); }
                    if (ceil) { WriteWall(V, 5); }
                    file.Write(sb.ToString());
                }
            }

        }
    }

    public System.Numerics.Vector3 CalculateTriangleNormal(System.Numerics.Vector3 v1, System.Numerics.Vector3 v2, System.Numerics.Vector3 v3) {
        System.Numerics.Vector3 a = v2 - v1;
        System.Numerics.Vector3 b = v3 - v1;
        a = System.Numerics.Vector3.Cross(a, b);
        return System.Numerics.Vector3.Normalize(a);
    }

    private const string v_String = "v {0} {2} {1} \n";
    private const string vtString = "vt {0} {1}  \n";
    private const string vnString = "vn {0} {2} {1} \n";
    private const string fString1 = "f -4/-4/-1 -3/-3/-1 -2/-2/-1  \n";
    private const string fString2 = "f -3/-3/-1 -1/-1/-1 -2/-2/-1  \n";
    private const string fString12 ="f -4/-4/-1 -2/-2/-1 -3/-3/-1  \n";
    private const string fString22 ="f -3/-3/-1 -2/-2/-1 -1/-1/-1  \n";



    private void WriteWall(System.Numerics.Vector3[] V, int f) {
        for (int k = 0; k < 4; ++k) {
            sb.Append(string.Format(v_String, V[pos[f,k]].X, V[pos[f, k]].Y, V[pos[f, k]].Z));
            sb.Append(string.Format(vtString, V[pos[f, k]].X, V[pos[f, k]].Y));
        }
        n = CalculateTriangleNormal(V[pos[f,0]], V[pos[f,1]], V[pos[f,2]]);
        sb.Append(string.Format(vnString, n.X, n.Y, n.Z));
        sb.Append(fString1);
        n = -n;
        sb.Append(string.Format(vnString, n.X, n.Y, n.Z));
        sb.Append(fString12);

        n = CalculateTriangleNormal(V[pos[f,1]], V[pos[f,3]], V[pos[f,2]]);
        sb.Append(string.Format(vnString, n.X, n.Y, n.Z));
        sb.Append(fString2);
        n = -n;
        sb.Append(string.Format(vnString, n.X, n.Y, n.Z));
        sb.Append(fString22);
    }
}