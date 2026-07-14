using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        MazeGenerator myGenerator = (MazeGenerator) target;

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Maze Actions", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("Generate Maze", GUILayout.Height(30)))
            {
                Undo.RecordObject(myGenerator, "Generate Maze");
                myGenerator.GenerateMaze();
            }

            GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
            if (GUILayout.Button("Destroy Maze", GUILayout.Height(30)))
            {
                Undo.RecordObject(myGenerator, "Destroy Maze");
                myGenerator.DestroyMaze();
            }
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Utilities & Export", EditorStyles.centeredGreyMiniLabel);
        
        if (GUILayout.Button("Export to OBJ"))
            myGenerator.createOBJ();

        if (GUILayout.Button("Run Quality Analysis", EditorStyles.miniButtonLeft))
            myGenerator.executeAnalysis();

        if (GUILayout.Button("Run Performance (Time) Analysis", EditorStyles.miniButtonRight))
            myGenerator.executeTimeAnalysis();
    }
}
