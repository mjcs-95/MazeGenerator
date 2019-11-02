using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        MazeGenerator myGenerator = (MazeGenerator) target;
        if (GUILayout.Button("Generate Maze")) {
            myGenerator.tests();
        }
        if (GUILayout.Button("Destroy Maze")) {
            myGenerator.DestroyMaze();
        }
    }
}
