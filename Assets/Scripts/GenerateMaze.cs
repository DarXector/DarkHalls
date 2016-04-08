using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class GenerateMaze : MonoBehaviour
{

    public Vector2 mazeSize = new Vector2(6, 4);
    public Vector2 mazeStart = new Vector2(0, 1);
    public Vector2 mazeEnd = new Vector2(6, 3);
    public float tileSize = 0.5f;
    public GameObject wall;
    public GameObject start;
    public GameObject end;

    public InputField mazeCode;

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
        Generate();
    }

    public void Generate()
    {
        while (transform.childCount > 0)
        {
            GameObject.Destroy(transform.GetChild(0));
        }

        _width = (int)mazeSize.y * 2 + 1;
        _height = (int)mazeSize.x * 2 + 1;

        InitData();
        CreateData();
        DrawMaze();
    }

    void InitData()
    {
        _maze = new int[_height, _width];

        for (int x = 0; x < _height; x++)
        {
            for (int y = 0; y < _width; y++)
            {
                _maze[x, y] = 1;
            }
        }

        _maze[(int)mazeStart.x, (int)mazeStart.y] = 2;
        _maze[(int)mazeEnd.x, (int)mazeEnd.y] = 3;
    }

    void CreateData()
    {
        int back;
        int move;
        String possibleDirections;
        Vector2 pos = mazeStart;

        _moves = new List<int>();
        _moves.Add((int)(pos.y + (pos.x * _width)));

        while (_moves.Count > 0)
        {
            possibleDirections = "";

            //Debug.Log("pos: " + pos);


            if ((pos.x + 2 < _height) && (_maze[(int)pos.x + 2, (int)pos.y] == 1) && (pos.x + 2 != 0) && (pos.x + 2 != _height - 1))
            {
                possibleDirections += NORTH;
            }

            if ((pos.x - 2 >= 0) && (_maze[(int)pos.x - 2, (int)pos.y] == 1) && (pos.x - 2 != 0) && (pos.x - 2 != _height - 1))
            {
                possibleDirections += SOUTH;
            }

            if ((pos.y - 2 >= 0) && (_maze[(int)pos.x, (int)pos.y - 2] == 1) && (pos.y - 2 != 0) && (pos.y - 2 != _width - 1))
            {
                possibleDirections += WEST;
            }

            if ((pos.y + 2 < _width) && (_maze[(int)pos.x, (int)pos.y + 2] == 1) && (pos.y + 2 != 0) && (pos.y + 2 != _width - 1))
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
                        _maze[(int)pos.x - 2, (int)pos.y] = 0;
                        _maze[(int)pos.x - 1, (int)pos.y] = 0;
                        pos.x -= 2;
                        break;

                    case NORTH:
                        _maze[(int)pos.x + 2, (int)pos.y] = 0;
                        _maze[(int)pos.x + 1, (int)pos.y] = 0;
                        pos.x += 2;
                        break;

                    case WEST:
                        _maze[(int)pos.x, (int)pos.y - 2] = 0;
                        _maze[(int)pos.x, (int)pos.y - 1] = 0;
                        pos.y -= 2;
                        break;

                    case EAST:
                        _maze[(int)pos.x, (int)pos.y + 2] = 0;
                        _maze[(int)pos.x, (int)pos.y + 1] = 0;
                        pos.y += 2;
                        break;
                }

                _moves.Add((int)(pos.y + (pos.x * _width)));
            }
            else
            {
                back = _moves[_moves.Count - 1];
                pos.x = Mathf.Round(back / _width);
                pos.y = back % _width;

                _moves.Remove(back);
            }
        }
    }

    private void DrawMaze()
    {
        // Debug.Log("MAZE: " + _maze);

        // Instantiate(tile, new Vector3())
        mazeCode.text += mazeSize.x + "|" + mazeSize.y + "|";
        for (int x = 0; x < _height; x++)
        {
            for (int y = 0; y < _width; y++)
            {
                mazeCode.text += _maze[x, y];

                GameObject t;
                if(_maze[x, y] == 1)
                {
                    t = (GameObject)Instantiate(wall, new Vector3(y * tileSize, x * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                }
                else if (_maze[x, y] == 2)
                {
                    t = (GameObject)Instantiate(start, new Vector3(y * tileSize, x * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                }
                else if (_maze[x, y] == 3)
                {
                    t = (GameObject)Instantiate(end, new Vector3(y * tileSize, x * tileSize), Quaternion.identity);
                    t.transform.parent = gameObject.transform;
                }
                
            }
        }
    }
}
