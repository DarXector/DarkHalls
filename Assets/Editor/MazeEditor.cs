using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

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
            _mazeGenarator.Load(_mazeGenarator.mazeID);
        }

        // add a custom button to the Inspector component
        if (GUILayout.Button("Save Maze"))
        {
            // if button pressed, then call function in script
            Save();
        }
        EditorGUILayout.EndHorizontal();
    }

    public void Save()
    {
        Debug.Log("Save _mazeGenarator.mazeID " + _mazeGenarator.mazeID);
        Debug.Log("Save _mazeGenarator.mazeCode " + _mazeGenarator.mazeCode);

        if (_mazeGenarator.mazeCode == "")
        {
            return;
        }

        if (_mazeGenarator.mazeID <= 0)
        {
            return;
        }

        if (_mazeGenarator.levels == null)
        { 
            _mazeGenarator.levels = new Levels();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.Create);
        Levels levels = _mazeGenarator.levels;
        LevelData level;

        Debug.Log("Save levels.levels.Count " + levels.levels.Count);

        if (levels.levels.Count > _mazeGenarator.mazeID)
        {
            level = levels.levels[_mazeGenarator.mazeID - 1];
            Debug.Log("Save level exists " + level);
        }
        else
        {
            level = new LevelData();
            levels.levels.Add(level);
            Debug.Log("Save level does not exist " + level);
        }

        level.data = _mazeGenarator.mazeCode;
        level.level = _mazeGenarator.mazeID.ToString();
        level.width = _mazeGenarator.mazeSize.x.ToString();
        level.height = _mazeGenarator.mazeSize.y.ToString();

        Debug.Log("Save level " + levels.levels[_mazeGenarator.mazeID - 1].data);

        bf.Serialize(file, _mazeGenarator.levels);
        file.Close();
    }
}
