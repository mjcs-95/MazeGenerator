﻿using System.Text;
using UnityEngine; //@Application

public class ExportToOBj<T> where T : System.IComparable<T> {
    // Start is called before the first frame update
    public enum Face { N, S, E, W, F, C }

    StringBuilder sb;
    System.Numerics.Vector3[] V;
    System.Numerics.Vector3 n;


    //Wall wdith != 0
    //float x_acum;
    //float y_acum;
    float xwidth; 
    float ywidth;
    float scale;
    /*
    //Multiple floor 
    float z_acum = 0;
    float z_height = 0;
    float x_width = 0;
    public ExportToOBj() {
        sb = new StringBuilder();
        V = new System.Numerics.Vector3[8];
        n = System.Numerics.Vector3.Zero;
        pos = new byte[6, 4] {
            { 0,1,2,3 },    //0 - Floor
            { 2,3,6,7 },    //1 - North
            { 0,1,4,5 },    //2 - South
            { 1,3,5,7 },    //3 - East
            { 0,2,4,6 },    //4 - West
            { 4,5,6,7 }     //5 - Ceil
        };
        x_acum = 0;
        y_acum = 0;
    }
*/

    byte[,] pos;
    float xsize, ysize;

    public ExportToOBj(float scale_ = 1, float xwidth_ = 0, float ywidth_ = 0) 
    {
        sb = new StringBuilder();
        V = new System.Numerics.Vector3[8];
        n = System.Numerics.Vector3.Zero;
        pos = new byte[6, 4] 
        {
            { 0,1,2,3 },    //0 - Floor
            { 2,3,6,7 },    //1 - North
            { 0,1,4,5 },    //2 - South
            { 1,3,5,7 },    //3 - East
            { 0,2,4,6 },    //4 - West
            { 4,5,6,7 }     //5 - Ceil
        };
        scale = scale_;
        xwidth = xwidth_;
        ywidth = ywidth_;
        //x_acum = 0;
        //y_acum = 0;
    }


    public void GenerateObj(MazeGraph<T> G, bool ceil = false) 
    {
        xsize = G.rows;
        ysize = G.cols;
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Application.dataPath+"/Resources/objeto1.obj")) {
            for (int i = 0; i < G.rows; ++i)
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

    public System.Numerics.Vector3 CalculateTriangleNormal(System.Numerics.Vector3 v1, System.Numerics.Vector3 v2, System.Numerics.Vector3 v3) 
    {
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

    private void WriteWall(System.Numerics.Vector3[] V, int f)
    {
        for (int k = 0; k < 4; ++k) 
        {
            sb.Append(string.Format(v_String, V[pos[f,k]].X, V[pos[f, k]].Y, V[pos[f, k]].Z));
            if (f == 0 || f == 5)
                sb.Append(string.Format(vtString, V[pos[f, k]].X, V[pos[f, k]].Y));
            else if (f == 1 || f == 2)
                sb.Append(string.Format(vtString, V[pos[f, k]].Y, V[pos[f, k]].Z));
            else if (f == 3 || f == 4)
                sb.Append(string.Format(vtString, V[pos[f, k]].X, V[pos[f, k]].Z));
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