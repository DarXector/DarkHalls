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

    private Levels levels;

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
        Load(1);
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

    private void DrawMaze()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {

                GameObject t;
                if (_maze[x, y] == 1)
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
