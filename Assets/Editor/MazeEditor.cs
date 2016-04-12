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
        if (_mazeGenarator.mazeCode == "")
        {
            return;
        }

        if (_mazeGenarator.mazeID > 0)
        {
            return;
        }

        FileMode fileMode;


        if (File.Exists(Application.persistentDataPath + "/levels.dat"))
        {
            fileMode = FileMode.Open;
        }
        else
        {
            fileMode = FileMode.Create;
            _mazeGenarator.levels = new Levels();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", fileMode);

        LevelData level;

        if (_mazeGenarator.levels.levels.Count == _mazeGenarator.mazeID)
        {
            level = _mazeGenarator.levels.levels[_mazeGenarator.mazeID - 1];
            if (level == null)
            {
                level = new LevelData();
                _mazeGenarator.levels.levels.Add(level);
            }
        }
        else
        {
            level = new LevelData();
            _mazeGenarator.levels.levels.Add(level);
        }

        level.data = _mazeGenarator.mazeCode;
        level.level = _mazeGenarator.mazeID.ToString();
        level.width = _mazeGenarator.mazeSize.x.ToString();
        level.height = _mazeGenarator.mazeSize.y.ToString();

        bf.Serialize(file, _mazeGenarator.levels);
        file.Close();
    }
}
