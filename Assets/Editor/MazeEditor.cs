using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(MazeGenerator))]
public class MazeEditor : Editor { // extend the Editor class

    MazeGenerator _mazeGenarator;

    // called when Unity Editor Inspector is updated
    public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

        _mazeGenarator = target as MazeGenerator;

        if (GUILayout.Button("Generate Maze"))
        {
            _mazeGenarator.Generate();
        }

        EditorGUILayout.BeginHorizontal();
        // add a custom button to the Inspector component
        if (GUILayout.Button("Load Maze"))
		{
            // if button pressed, then call function in script
            GameModel.Instance.OnLevelLoaded += _mazeGenarator.Parse;
            GameModel.Instance.Load(_mazeGenarator.mazeID);
        }

        // add a custom button to the Inspector component
        if (GUILayout.Button("Save Maze"))
        {
            // if button pressed, then call function in script
            GameModel.Instance.Save(_mazeGenarator.mazeID, _mazeGenarator.mazeCode, _mazeGenarator.mazeSize);
        }
        EditorGUILayout.EndHorizontal();
    }
}
