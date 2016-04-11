using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class MazeEditor : MonoBehaviour
{

    public Vector2 mazeSize = new Vector2(4, 6);
    public Vector2 mazeStart = new Vector2(0, 1);
    public float tileSize = 0.5f;
    public GameObject wall;
    public GameObject start;
    public GameObject end;

    private Levels levels;

    public InputField mazeCode;
    public InputField levelNumber;
    public Text editorMessaging;

    const char NORTH = 'N';
    const char SOUTH = 'S';
    const char EAST = 'E';
    const char WEST = 'W';

    int[,] _maze;
    List<int> _moves;
    int _width;
    int _height;

    // Use this for initialization
    void Start()
    {
        Load();
    }

    void ResetMaze()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        gameObject.transform.position = new Vector3(0, 0, 0);
    }

    public void Generate()
    {
        mazeCode.text = "";

        ResetMaze();

        editorMessaging.text = "Generated a new maze";

        _width = (int)mazeSize.x * 2 + 1;
        _height = (int)mazeSize.y * 2 + 1;

        InitData();
        CreateData();
        DrawMaze();
    }

    void InitData()
    {
        _maze = new int[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _maze[x, y] = 1;
            }
        }

        _maze[(int)mazeStart.x, (int)mazeStart.y] = 2;
        _maze[UnityEngine.Random.Range(1, _width-1), _height-1] = 3;
    }

    void CreateData()
    {
        int back;
        int move;
        String possibleDirections;
        Vector2 pos = mazeStart;

        _moves = new List<int>();
        _moves.Add((int)(pos.x + (pos.y * _width)));

        while (_moves.Count > 0)
        {
            possibleDirections = "";


            if ((pos.y + 2 < _height) && (_maze[(int)pos.x, (int)pos.y + 2] == 1) && (pos.y + 2 != 0) && (pos.y + 2 != _height - 1))
            {
                possibleDirections += NORTH;
            }

            if ((pos.y - 2 >= 0) && (_maze[(int)pos.x, (int)pos.y - 2] == 1) && (pos.y - 2 != 0) && (pos.y - 2 != _height - 1))
            {
                possibleDirections += SOUTH;
            }

            if ((pos.x - 2 >= 0) && (_maze[(int)pos.x - 2, (int)pos.y] == 1) && (pos.x - 2 != 0) && (pos.x - 2 != _width - 1))
            {
                possibleDirections += WEST;
            }

            if ((pos.x + 2 < _width) && (_maze[(int)pos.x + 2, (int)pos.y] == 1) && (pos.x + 2 != 0) && (pos.x + 2 != _width - 1))
            {
                possibleDirections += EAST;
            }

            //Debug.Log("possibleDirections: " + possibleDirections);

            if (possibleDirections.Length > 0)
            {
                move = UnityEngine.Random.Range(0, possibleDirections.Length);

                //Debug.Log("move: " + move);
                //Debug.Log("move: " + possibleDirections[move]);

                switch (possibleDirections[move])
                {
                    case SOUTH:
                        _maze[(int)pos.x, (int)pos.y - 2] = 0;
                        _maze[(int)pos.x, (int)pos.y - 1] = 0;
                        pos.y -= 2;
                        break;

                    case NORTH:
                        _maze[(int)pos.x, (int)pos.y + 2] = 0;
                        _maze[(int)pos.x, (int)pos.y + 1] = 0;
                        pos.y += 2;
                        break;

                    case WEST:
                        _maze[(int)pos.x - 2, (int)pos.y] = 0;
                        _maze[(int)pos.x - 1, (int)pos.y] = 0;
                        pos.x -= 2;
                        break;

                    case EAST:
                        _maze[(int)pos.x + 2, (int)pos.y] = 0;
                        _maze[(int)pos.x + 1, (int)pos.y] = 0;
                        pos.x += 2;
                        break;
                }

                _moves.Add((int)(pos.x + (pos.y * _width)));
            }
            else
            {
                back = _moves[_moves.Count - 1];
                pos.x = back % _width;
                pos.y = Mathf.Round(back / _width);

                _moves.Remove(back);
            }
        }
    }

    private void DrawMaze()
    {
        editorMessaging.text = "Drawing maze...";

        mazeCode.text = "";
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                mazeCode.text += _maze[x, y];

                GameObject t;
                if(_maze[x, y] == 1)
                {
                    t = (GameObject)Instantiate(wall, new Vector3(x * tileSize, y * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                }
                else if (_maze[x, y] == 2)
                {
                    t = (GameObject)Instantiate(start, new Vector3(x * tileSize, y * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                }
                else if (_maze[x, y] == 3)
                {
                    t = (GameObject)Instantiate(end, new Vector3(x * tileSize, y * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                }
                
            }
        }

        gameObject.transform.position = new Vector3(-_width * tileSize / 2, -_height * tileSize / 2);

        editorMessaging.text = "Maze drawn!";
    }

    public void Save()
    {
        if(mazeCode.text == "")
        {
            editorMessaging.text = "Maze code field empty";
            return;
        }

        if (levelNumber.text == "" || levelNumber.text == "0")
        {
            editorMessaging.text = "Level number field invalid";
            return;
        }

        FileMode fileMode;
        if (File.Exists(Application.persistentDataPath + "/levels.dat"))
        {
            fileMode = FileMode.Open;
            editorMessaging.text = "Opening levels.dat for saving";
        }
        else
        {
            fileMode = FileMode.Create;
            levels = new Levels();
            editorMessaging.text = "levels.dat file not created, creating one";
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", fileMode);

        LevelData level;

        if(levels.levels.Count == int.Parse(levelNumber.text))
        {
            level = levels.levels[int.Parse(levelNumber.text) - 1];
            if (level == null)
            {
                level = new LevelData();
                levels.levels.Add(level);
            }
        }
        else
        {
            level = new LevelData();
            levels.levels.Add(level);
        }

        level.data = mazeCode.text;
        level.level = levelNumber.text;
        level.width = mazeSize.x.ToString();
        level.height = mazeSize.y.ToString();

        Debug.Log("Saving: " + level.data + "  level.width " + level.width + " level.height " + level.height);

        bf.Serialize(file, levels);
        file.Close();

        editorMessaging.text = "levels.dat saved";
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/levels.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.Open);
            levels = (Levels)bf.Deserialize(file);
            file.Close();

            editorMessaging.text = "Opening levels.dat for loading level data";

            LevelData level = levels.levels[int.Parse(levelNumber.text) - 1];
            if(level != null)
            {
                editorMessaging.text = "Level exists, parsing level data";
                Parse(level);
            }
            else
            {
                editorMessaging.text = "Level does not exist, generate a new level";
                Generate();
            }
            
        }
        else
        {
            editorMessaging.text = "levels.dat file does not exist, generate a new level";
            Generate();
        }
    }

    void Parse(LevelData level)
    {
        mazeCode.text = level.data;
        levelNumber.text = level.level;

        ResetMaze();

        Debug.Log("Parsing: " + level.data);

        mazeSize = new Vector2(int.Parse(level.width), int.Parse(level.height));

        _width = (int)mazeSize.x * 2 + 1;
        _height = (int)mazeSize.y * 2 + 1;

        Debug.Log("Parsing: _width " +  _width + " _height " + _height);

        _maze = new int[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _maze[x, y] = int.Parse(level.data[y * _width + x].ToString());


                if (_maze[x, y] == 2)
                {
                    mazeStart = new Vector2(x, y);
                }
            }
        }

        DrawMaze();
    }

}

[Serializable]
public class LevelData
{
    public string data;
    public string level;
    public string width;
    public string height;
}

[Serializable]
public class Levels
{
    [SerializeField]
    public List<LevelData> levels = new List<LevelData>();
}
