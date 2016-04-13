using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{

    public Vector2 mazeSize = new Vector2(4, 6);
    public Vector2 mazeStart = new Vector2(0, 1);
    public float tileSize = 0.5f;
    public GameObject wall;
    public GameObject start;
    public GameObject end;
    public GameObject glowFloor;

    [HideInInspector]
    public List<GameObject> glowTiles;
    [HideInInspector]
    public GameObject player;

    public Levels levels;

    public string mazeCode;
    public int mazeID = 1;

    protected const char NORTH = 'N';
    protected const char SOUTH = 'S';
    protected const char EAST = 'E';
    protected const char WEST = 'W';

    protected int[,] _maze;
    protected List<int> _moves;
    protected int _width;
    protected int _height;

    // Use this for initialization
    void Start()
    {
        Load(mazeID);
    }

    protected void ResetMaze()
    {
        mazeCode = "";

        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        glowTiles = new List<GameObject>();
    }

    public void Generate()
    {
        ResetMaze();

        _width = (int)mazeSize.x * 2 + 1;
        _height = (int)mazeSize.y * 2 + 1;

        InitData();
        CreateData();
        DrawMaze();
    }

    void InitData()
    {
        _maze = new int[_width, _height];
        glowTiles = new List<GameObject>();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _maze[x, y] = 1;
            }
        }

        _maze[(int)mazeStart.x, (int)mazeStart.y] = 2;
        _maze[UnityEngine.Random.Range(1, _width - 1), _height - 1] = 3;
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

    protected void DrawMaze()
    {
        GameObject w = null;
        float tileWidth = 0;

        for (int y = 0; y < _height; y++)
        {
            w = null;
            tileWidth = 0;

            for (int x = 0; x < _width; x++)
            {
                mazeCode += _maze[x, y];

                GameObject t;
                if (_maze[x, y] == 1 && !w)
                {
                    w = (GameObject)Instantiate(wall, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
                    w.transform.parent = gameObject.transform;
                    tileWidth = tileSize;
                }
                else if (_maze[x, y] == 1 && w)
                {
                    tileWidth += tileSize;
                    w.transform.position = new Vector3(x * tileSize - (tileWidth - tileSize) / 2f, 0, y * tileSize);
                    w.transform.localScale = new Vector3(tileWidth, tileSize, tileSize);
                }
                else if (_maze[x, y] == 2)
                {
                    player = (GameObject)Instantiate(start, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
                    player.transform.parent = gameObject.transform;
                }
                else if (_maze[x, y] == 3)
                {
                    t = (GameObject)Instantiate(end, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                }

                if(_maze[x, y] != 1)
                {
                    w = null;

                    t = (GameObject)Instantiate(glowFloor, new Vector3(x * tileSize, -tileSize / 2f, y * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                    t.GetComponent<Renderer>().enabled = false;
                    glowTiles.Add(t);

                }
            }
        }
    }

    public void Load(int levelNumber)
    {
        if (File.Exists(Application.persistentDataPath + "/levels.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.Open);
            levels = (Levels)bf.Deserialize(file);
            file.Close();

            LevelData level = levels.levels[levelNumber - 1];
            if (level != null)
            {
                Parse(level);
            }
            else
            {
                Generate();
            }

        }
        else
        {
            Generate();
        }
    }

    void Parse(LevelData level)
    {
        ResetMaze();

        mazeSize = new Vector2(int.Parse(level.width), int.Parse(level.height));

        _width = (int)mazeSize.x * 2 + 1;
        _height = (int)mazeSize.y * 2 + 1;

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
