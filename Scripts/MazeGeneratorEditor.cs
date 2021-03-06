﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        MazeGenerator myGenerator = (MazeGenerator) target;
        if (GUILayout.Button("Generate Maze"))          
            myGenerator.GenerateMaze();
        if (GUILayout.Button("Destroy Maze"))           
            myGenerator.DestroyMaze();
        if (GUILayout.Button("Generar OBJ"))            
            myGenerator.createOBJ(); 
        if (GUILayout.Button("Execute Analysis"))       
            myGenerator.executeAnalysis(); 
        if (GUILayout.Button("Execute Time Analysis"))  
            myGenerator.executeTimeAnalysis();    
    }
}
